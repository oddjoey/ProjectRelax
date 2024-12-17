using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    GameLogic game;

    // Networking
    public NetworkObject networkObject;
    // Systems
    public InventoryLogic inventory;

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
    
    // RPC Functions
    [ServerRpc(RequireOwnership = false)]
    public void RPCLoadGarageFromCity(NetworkConnection conn = null)
    {
        NetworkedPlayer player = conn.FirstObject.GetComponent<NetworkedPlayer>();

        SceneLoadData sld = new SceneLoadData("Garage");
        sld.ReplaceScenes = ReplaceOption.All;
        sld.MovedNetworkObjects = new NetworkObject[] { player.networkObject, player.currentVehicle.networkObject };
        Debug.Log("Loading garage from server~!");
        game.network.networkManager.SceneManager.LoadConnectionScenes(conn, sld);

        SceneUnloadData sud = new SceneUnloadData("City");
        game.network.networkManager.SceneManager.UnloadConnectionScenes(conn, sud);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RPCLoadCityFromGarage(NetworkConnection conn = null)
    {
        NetworkedPlayer player = conn.FirstObject.GetComponent<NetworkedPlayer>();

        SceneLoadData sld = new SceneLoadData("City");
        sld.ReplaceScenes = ReplaceOption.All;
        sld.MovedNetworkObjects = new NetworkObject[] { player.networkObject, player.currentVehicle.networkObject };
        Debug.Log("Loading city from server~!");
        game.network.networkManager.SceneManager.LoadConnectionScenes(conn, sld);

        SceneUnloadData sud = new SceneUnloadData("Garage");
        game.network.networkManager.SceneManager.UnloadConnectionScenes(conn, sud);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RPCSpawnCar(NetworkConnection conn = null)
    {
        GameObject car = Instantiate(game.carPrefab.gameObject);
        car.name = "Car";
        car.transform.position = new Vector3(28, 10, 35);
        car.transform.localEulerAngles = new Vector3(0, -90, 0);
        game.network.networkManager.ServerManager.Spawn(car, conn);
    }
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
       inventory.currentHotbarItem = inventory.hotbarGrid.cells[inventory.currentHotbarIndex, 0];

        // Hotbar item change
        if (inventory.lastHotbarItemIndex != inventory.currentHotbarIndex)
        {
            //if (game.inventory.currentHotbarItem != null)
                //game.sounds.ObjectSwitchSound(game.inventory.currentHotbarItem.type);

            inventory.lastHotbarItemIndex = inventory.currentHotbarIndex;
        }

        // No item being held, destory held item if there is one
        if (inventory.currentHotbarItem == null)
        {
            if (inventory.holdingItem != null)
            {
                Destroy(inventory.holdingItem);
                inventory.holdingItem = null;
            }            
            return;
        }

        // Holding item
        if (inventory.holdingItem != null)
        {
            if (inventory.lastHotbarItemType != inventory.currentHotbarItem.type)
            {
                Destroy(inventory.holdingItem);

                inventory.holdingItem = Instantiate(inventory.currentHotbarItem.heldObject);

                inventory.holdingItem.transform.parent = camera.transform;
                inventory.holdingItem.transform.localPosition = inventory.currentHotbarItem.localPosition;
                inventory.holdingItem.transform.localRotation = Quaternion.Euler(inventory.currentHotbarItem.localRotation);

                inventory.lastHotbarItemType = inventory.currentHotbarItem.type;
                inventory.hotbarItemLogic = inventory.holdingItem.GetComponent<HeldItemLogic>();
            }
        }
        else
        {
            inventory.holdingItem = Instantiate(inventory.currentHotbarItem.heldObject);

            inventory.holdingItem.transform.parent = camera.transform;
            inventory.holdingItem.transform.localPosition = inventory.currentHotbarItem.localPosition;
            inventory.holdingItem.transform.localRotation = Quaternion.Euler(inventory.currentHotbarItem.localRotation);

            inventory.lastHotbarItemType = inventory.currentHotbarItem.type;
            inventory.hotbarItemLogic =inventory.holdingItem.GetComponent<HeldItemLogic>();
        }
            if (inventory.currentHotbarItem == null)
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
        car.currentDriver = game.LocalPlayer.networkObject;
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
        currentVehicle.currentDriver = null;
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
    public override void OnStartClient()
    {      
        if (!base.IsOwner)
            return;

        game = GameLogic.instance;
        networkObject = GetComponent<NetworkObject>();

        rigidBody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        camera = transform.Find("Player Camera").gameObject;

        rayCastInteractableMask = 1 << LayerMask.NameToLayer("Interactable");

        inventory = GetComponent<InventoryLogic>();

        camera.GetComponent<Camera>().enabled = true;
        camera.GetComponent<AudioListener>().enabled = true;
        game.miniMap.minimapCamera.enabled = true;
    }
    void FixedUpdate()
    {
        if (!base.IsOwner)
            return;
            
        if (game.UI.isCursorLocked && !inVehicle)
            MovementLogic();
    }
    void Update()
    {
        if (!base.IsOwner)
            return;

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

