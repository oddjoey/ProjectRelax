using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CarLogic : NetworkBehaviour
{
    GameLogic game;

    // Networking
    public NetworkObject networkObject;
    public NetworkObject currentDriver;
    
    // Vehicle Systems
    public SCC_Drivetrain drivetrain;
    public SCC_InputProcessor inputs;
    public new SCC_Audio audio;
    public SCC_Particles particles;
    public SCC_AntiRoll antiroll;
    public SCC_RigidStabilizer rigidStabilizer;
    public CarSuspensionLogic wheels;

    // Car Components
    private GameObject lights;
    public ParticleSystem leftExhaust;
    public ParticleSystem rightExhaust;
    [HideInInspector] public new GameObject camera;
    private new Rigidbody rigidbody;
    
    // Brake Lights
    private Light RLBrakeLight;
    private Light RRBrakeLight;

    // Variables
    public bool engineOn = false;
    public bool playerInside = false;

    // Internal Variables
    private bool engineHeld = false;

    // Layer Masks
    private int raycastEntranceMask;
    public bool IsLocalPlayerDriving()
    {
        return game != null && game.network.HasPlayerSpawned() && currentDriver == game.LocalPlayer.networkObject;
    }
    public override void OnStartClient()
    {
        game = GameLogic.instance;
        networkObject = GetComponent<NetworkObject>();
        rigidbody = GetComponent<Rigidbody>();
        drivetrain = GetComponent<SCC_Drivetrain>();
        inputs = GetComponent<SCC_InputProcessor>();
        audio = GetComponent<SCC_Audio>();
        particles = GetComponent<SCC_Particles>();
        antiroll = GetComponent<SCC_AntiRoll>();
        rigidStabilizer = GetComponent<SCC_RigidStabilizer>();

        lights = transform.Find("Lights").gameObject;
        RLBrakeLight = lights.transform.Find("RL Brake Light").GetComponent<Light>();
        RRBrakeLight = lights.transform.Find("RR Brake Light").GetComponent<Light>();

        var exhaustObj = transform.Find("Exhausts");
        leftExhaust = exhaustObj.Find("RL Exhaust Particles").GetComponent<ParticleSystem>();
        rightExhaust = exhaustObj.Find("RR Exhaust Particles").GetComponent<ParticleSystem>();

        camera = transform.Find("Vehicle Camera").gameObject;
        wheels = GetComponent<CarSuspensionLogic>();

        raycastEntranceMask = 1 << LayerMask.NameToLayer("Entrance");
    }

    void Update()
    {
        if (IsLocalPlayerDriving())
        {
            // Load Garage
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, raycastEntranceMask) && !game.teleporter.sceneChanging)
            {
                game.teleporter.LoadGarage();
            }           
            // Turn engine off and on
            if (game.inputs.engine && !engineHeld)
            {
                engineHeld = true;
                engineOn = !engineOn;
            }

            if (!game.inputs.engine)
                engineHeld = false;
        }

        ApplyCarLogic();
    }
    void ApplyCarLogic()
    {
        if (engineOn && IsLocalPlayerDriving())
            ApplyBrakeLights();

        // Should car be on or off
        audio.maximumVolume = engineOn == true ? 1 : 0;
        
        if (engineOn)
        {
            leftExhaust.Play(true);
            rightExhaust.Play(true);
        }
        else
        {
            leftExhaust.Stop(true);
            rightExhaust.Stop(true);
        }
        
        antiroll.enabled = engineOn;
        rigidStabilizer.enabled = engineOn;

        lights.SetActive(engineOn);
    }

    void ApplyBrakeLights()
    {
        bool brakesApplied = inputs.inputs.brakeInput > 0;

        RLBrakeLight.intensity = brakesApplied ? 90.0f : 50.5f;
        RRBrakeLight.intensity = brakesApplied ? 90.0f : 50.5f;
    }

    public void StopMoving()
    {
        foreach (var wheel in drivetrain.wheels)
        {
            wheel.wheelCollider.WheelCollider.motorTorque = 0;
            wheel.wheelCollider.WheelCollider.brakeTorque = 10000;
            wheel.wheelCollider.WheelCollider.transform.localEulerAngles = new Vector3(0, wheel.wheelCollider.WheelCollider.steerAngle, 0);
        }
        engineOn = false;
        drivetrain.speed = 0;
        drivetrain.currentEngineRPM = 0;
        rigidbody.linearVelocity = Vector3.zero;
    }
}
