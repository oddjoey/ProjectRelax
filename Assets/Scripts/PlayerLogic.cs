using FishNet.Object;
using UnityEngine;

public class PlayerLogic : NetworkBehaviour
{
    GameLogic game;

    // Components
    private Rigidbody rigidBody;
    private MeshRenderer meshRenderer;

    // Variables
    public bool inVehicle = false;
    public bool inGameWorld = false;
    public bool inGarage = false;

    // GameObjects
    [HideInInspector] public new GameObject camera;
    [HideInInspector] public CarLogic currentVehicle;

    [Header("LocalPlayer Variables")]
    [SerializeField] private float maxSpeed = 6.0f;
    [SerializeField] private float accelerationSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 10.0f;
    [SerializeField] public float interactDistance = 2.5f;

    // Internal Variables
    public Vector2 lookAngles;
    private float currentSpeed = 0.0f;
    private bool jumpHeld = false;
    private bool useHeld = false;

    // Raycast Masks
    private int rayCastInteractableMask;
    private int rayCastGroundMask;

    // Functions
    void LookLogic()
    {
        // Controller
        if (Mathf.Abs(game.inputs.lookDelta.x) < 1 && Mathf.Abs(game.inputs.lookDelta.x) > 0)
            lookAngles += game.settings.sensitivity * Time.deltaTime * game.inputs.lookDelta * 5;

        // Mouse
        else if (Mathf.Abs(game.inputs.lookDelta.x) >= 1 && game.inputs.rightClick)
            lookAngles += game.settings.sensitivity * Time.deltaTime * game.inputs.lookDelta;

        lookAngles += game.settings.sensitivity * Time.deltaTime * game.inputs.lookDelta;
        lookAngles.y = Mathf.Clamp(lookAngles.y, -88, 88);
        transform.localRotation = Quaternion.AngleAxis(lookAngles.x, Vector3.up);
        camera.transform.localRotation = Quaternion.AngleAxis(lookAngles.y, Vector3.left);
    }
    void MovementLogic()
    {
        // Movement inspired by quake engine
        // https://github.com/id-Software/Quake/blob/bf4ac424ce754894ac8f1dae6a3981954bc9852d/QW/client/pmove.c#L390
        Vector3 wishDirection = transform.forward * game.inputs.moveDelta.y;
        wishDirection += transform.right * game.inputs.moveDelta.x;

        currentSpeed = Vector3.Dot(rigidBody.linearVelocity, wishDirection);

        float addSpeed = maxSpeed - currentSpeed;
        if (addSpeed <= 0)
            return;

        float acceleration = accelerationSpeed * /*Time.deltaTime **/ maxSpeed;
        if (acceleration > addSpeed)
            acceleration = addSpeed;

        rigidBody.linearVelocity += acceleration * wishDirection;

        if (!jumpHeld && game.inputs.jump)// && IsGrounded())
        {
            jumpHeld = true;
            rigidBody.linearVelocity += transform.up * jumpHeight;
        }
        else if (jumpHeld && !game.inputs.jump)
            jumpHeld = false;

    }
    void HoldingObjectLogic()
    {
        game.inventory.currentHotbarItem = game.inventory.hotbarGrid.cells[game.hotbar.currentHotbarIndex, 0];

        // Hotbar item change
        if (game.inventory.lastHotbarItemIndex != game.hotbar.currentHotbarIndex)
        {
            //if (game.inventory.currentHotbarItem != null)
                //game.sounds.ObjectSwitchSound(game.inventory.currentHotbarItem.type);

            game.inventory.lastHotbarItemIndex = game.hotbar.currentHotbarIndex;
        }

        // No item being held, destory held item if there is one
        if (game.inventory.currentHotbarItem == null)
        {
            if (game.inventory.holdingItem != null)
            {
                Destroy(game.inventory.holdingItem);
                game.inventory.holdingItem = null;
            }            
            return;
        }

        // Holding item
        if (game.inventory.holdingItem != null)
        {
            if (game.inventory.lastHotbarItemType != game.inventory.currentHotbarItem.type)
            {
                Destroy(game.inventory.holdingItem);

                game.inventory.holdingItem = Instantiate(game.inventory.currentHotbarItem.heldObject);

                game.inventory.holdingItem.transform.parent = camera.transform;
                game.inventory.holdingItem.transform.localPosition = game.inventory.currentHotbarItem.localPosition;
                game.inventory.holdingItem.transform.localRotation = Quaternion.Euler(game.inventory.currentHotbarItem.localRotation);

                game.inventory.lastHotbarItemType = game.inventory.currentHotbarItem.type;
                game.inventory.hotbarItemLogic = game.inventory.holdingItem.GetComponent<HeldItemLogic>();
            }
        }
        else
        {
            game.inventory.holdingItem = Instantiate(game.inventory.currentHotbarItem.heldObject);

            game.inventory.holdingItem.transform.parent = camera.transform;
            game.inventory.holdingItem.transform.localPosition = game.inventory.currentHotbarItem.localPosition;
            game.inventory.holdingItem.transform.localRotation = Quaternion.Euler(game.inventory.currentHotbarItem.localRotation);

            game.inventory.lastHotbarItemType = game.inventory.currentHotbarItem.type;
            game.inventory.hotbarItemLogic = game.inventory.holdingItem.GetComponent<HeldItemLogic>();
        }
            if (game.inventory.currentHotbarItem == null)
                return;

    }
    public void GetInVehicle(CarLogic car)
    {
        game.sounds.PlaySound(game.sounds.carDoorOpenClose);
        currentVehicle = car;
        car.playerInside = true;
        inVehicle = true;

        camera.GetComponent<Camera>().enabled = false;
        camera.GetComponent<AudioListener>().enabled = false;

        car.camera.GetComponent<Camera>().enabled = true;
        car.camera.GetComponent<AudioListener>().enabled = true;

        game.UI.SetCrosshairVisibility(false);

        meshRenderer.enabled = false;

        transform.position *= 100;
    }
    public void GetOutVehicle()
    {
        game.sounds.PlaySound(game.sounds.carDoorOpenClose);
        currentVehicle.camera.GetComponent<Camera>().enabled = false;
        currentVehicle.camera.GetComponent<AudioListener>().enabled = false;

        camera.GetComponent<Camera>().enabled = true;
        camera.GetComponent<AudioListener>().enabled = true;

        game.UI.SetCrosshairVisibility(true);

        meshRenderer.enabled = true;
        transform.position = currentVehicle.transform.position + currentVehicle.transform.TransformDirection(Vector3.left) * 3;

        inVehicle = false;
        currentVehicle.playerInside = false;
        currentVehicle = null;
    }
    void InteractLogic()
    {
        if (!game.inputs.use || useHeld)
            return;

        useHeld = true;

        RaycastHit hit;
        if (!Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, interactDistance, rayCastInteractableMask))
            return;

        if (hit.transform.tag == "Vehicle" && !inGarage)
            GetInVehicle(hit.transform.gameObject.GetComponent<CarLogic>());

    }
    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, rayCastGroundMask))
            return true;

        return false;
    }
    void Start()
    {        
        game = GameLogic.instance;

        rigidBody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        camera = GameObject.Find("PlayerCamera");

        rayCastInteractableMask = 1 << LayerMask.NameToLayer("Interactable");
    }
    void FixedUpdate()
    {
        if (game.UI.isCursorLocked && !inVehicle)
            MovementLogic();
    }
    void Update()
    {
        HoldingObjectLogic();

        if (game.UI.isCursorLocked && !inVehicle)
        {
            LookLogic();
            InteractLogic();
        }

        if (inVehicle && game.inputs.use && !useHeld)
        {
            useHeld = true;
            GetOutVehicle();
        }

        if (!game.inputs.use)
            useHeld = false;
    }
}

