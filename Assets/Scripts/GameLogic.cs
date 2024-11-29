using System;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class GameLogic : MonoBehaviour
{    
    public static GameLogic instance;
    // Prefabs
    [SerializeField] public NetworkObject playerPrefab;
    public GameObject carPrefab;
    // Systems
    public InputManager inputs;
    public NetworkLogic network;
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

        network = GetComponent<NetworkLogic>();
        sounds = GameObject.Find("Sounds").GetComponent<SoundLogic>();
        UI = GameObject.Find("UI").GetComponent<UILogic>();
        mainMenu = GameObject.Find("Main Menu").GetComponent<MainMenuLogic>();
        miniMap = GameObject.Find("Minimap").GetComponent<MiniMapLogic>();

        inputs = GetComponent<InputManager>();
        settings = GetComponent<SettingsLogic>();
        teleporter = GetComponent<TeleporterLogic>();
        quests = GetComponent<QuestLogic>();
    }
}
