using System;
using UnityEngine;

public class GameLogic : MonoBehaviour
{    
    public static GameLogic instance;
    // Prefabs
    [SerializeField] private GameObject localPlayerPrefab;
    public GameObject carPrefab;
    // Systems
    [HideInInspector] public InputManager inputs;
    [HideInInspector] public InventoryLogic inventory;
    [HideInInspector] public HotbarLogic hotbar;
    [HideInInspector] public SettingsLogic settings;
    [HideInInspector] public SoundLogic sounds;
    [HideInInspector] public UILogic UI;
    [HideInInspector] public TeleporterLogic teleporter;
    [HideInInspector] public MainMenuLogic mainMenu;
    [HideInInspector] public LocalPlayerLogic localPlayer;
    [HideInInspector] public MiniMapLogic miniMap;
    [HideInInspector] public QuestLogic quests;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        inputs = GameObject.Find("Inputs").GetComponent<InputManager>();
        inventory = GameObject.Find("Inventory").GetComponent<InventoryLogic>();
        hotbar = GameObject.Find("Hotbar").GetComponent<HotbarLogic>();
        settings = GameObject.Find("Settings").GetComponent<SettingsLogic>();
        sounds = GameObject.Find("Sounds").GetComponent<SoundLogic>();
        UI = GameObject.Find("UI").GetComponent<UILogic>();
        teleporter = GameObject.Find("Teleporter").GetComponent<TeleporterLogic>();
        mainMenu = GameObject.Find("Main Menu").GetComponent<MainMenuLogic>();
        localPlayer = Instantiate(localPlayerPrefab).GetComponent<LocalPlayerLogic>();
        localPlayer.gameObject.name = "LocalPlayer";
        miniMap = GameObject.Find("Minimap").GetComponent<MiniMapLogic>();
        quests = GameObject.Find("Quests").GetComponent<QuestLogic>();
    }
}
