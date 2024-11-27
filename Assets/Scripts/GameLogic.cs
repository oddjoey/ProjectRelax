using System;
using FishNet.Managing;
using UnityEngine;

public class GameLogic : MonoBehaviour
{    
    public static GameLogic instance;
    // Prefabs
    [SerializeField] private GameObject localPlayerPrefab;
    public GameObject carPrefab;
    // Systems
    public InputManager inputs;
    public NetworkLogic network;
    public InventoryLogic inventory;
    public HotbarLogic hotbar;
    public SettingsLogic settings;
    public SoundLogic sounds;
    public UILogic UI;
    public TeleporterLogic teleporter;
    public MainMenuLogic mainMenu;
    public MiniMapLogic miniMap;
    public QuestLogic quests;

    public PlayerLogic LocalPlayer 
    {
        get {
            return null;
        }
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        inputs = GameObject.Find("Inputs").GetComponent<InputManager>();
        network = GameObject.Find("NetworkManager").GetComponent<NetworkLogic>();
        inventory = GameObject.Find("Inventory").GetComponent<InventoryLogic>();
        hotbar = GameObject.Find("Hotbar").GetComponent<HotbarLogic>();
        settings = GameObject.Find("Settings").GetComponent<SettingsLogic>();
        sounds = GameObject.Find("Sounds").GetComponent<SoundLogic>();
        UI = GameObject.Find("UI").GetComponent<UILogic>();
        teleporter = GameObject.Find("Teleporter").GetComponent<TeleporterLogic>();
        mainMenu = GameObject.Find("Main Menu").GetComponent<MainMenuLogic>();
        miniMap = GameObject.Find("Minimap").GetComponent<MiniMapLogic>();
        quests = GameObject.Find("Quests").GetComponent<QuestLogic>();
    }
}
