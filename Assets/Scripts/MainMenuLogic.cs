using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuLogic : MonoBehaviour
{
    GameLogic game;
    
    private UIDocument uIDocument;
    private Button mainMenuHostButton;
    private Button mainMenuJoinButton;
    private Button mainMenuGuideButton;
    private Button mainMenuSettingsButton;
    private Button mainMenuQuitButton;
    private Button menuBackButton;
    private Slider settingsVolumeSlider;
    private Slider settingsSensitivitySlider;
    private ScrollView friendsList;
    private Label friendNameLabel;
    private Label friendSteamIDLabel;
    private Button friendJoinButton;
    private CSteamID currentSelectedFriend;
    [SerializeField] private VisualTreeAsset mainMenuVisualTree;
    [SerializeField] private VisualTreeAsset guideVisualTree;
    [SerializeField] private VisualTreeAsset settingsVisualTree;
    [SerializeField] private VisualTreeAsset friendsVisualtree;
    public bool inMainMenu = true;

    // https://github.com/rlabrecque/Steamworks.NET-Test/blob/master/Assets/Scripts/SteamUtilsTest.cs
    private Texture2D GetSteamImageAsTexture2D(int iImage) {
    Texture2D ret = null;
    uint ImageWidth;
    uint ImageHeight;
    bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

    if (bIsValid) {
        byte[] Image = new byte[ImageWidth * ImageHeight * 4];

        bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
        if (bIsValid) {
            ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            ret.LoadRawTextureData(Image);
            ret.Apply();
        }
    }

    return ret;
}
    public void Hide()
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void Show()
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    /*
        Whenever we load a new scene, variables have new addresses so we have to find them again
        - K
    */
    public void SetupMainmenuUIElements()
    {
        mainMenuHostButton = uIDocument.rootVisualElement.Q<Button>("HostButton");
        mainMenuJoinButton = uIDocument.rootVisualElement.Q<Button>("JoinButton");
        mainMenuGuideButton = uIDocument.rootVisualElement.Q<Button>("GuideButton");
        mainMenuSettingsButton = uIDocument.rootVisualElement.Q<Button>("SettingsButton");
        mainMenuQuitButton = uIDocument.rootVisualElement.Q<Button>("QuitButton");

        mainMenuHostButton.RegisterCallback<ClickEvent>(OnHostButton);
        mainMenuJoinButton.RegisterCallback<ClickEvent>(OnJoinButton);
        mainMenuGuideButton.RegisterCallback<ClickEvent>(OnGuideButton);
        mainMenuSettingsButton.RegisterCallback<ClickEvent>(OnSettingsButton);
        mainMenuQuitButton.RegisterCallback<ClickEvent>(OnQuitButton);
    }
    void Start()
    {
        game = GameLogic.instance;
        uIDocument = GetComponent<UIDocument>();

        SetupMainmenuUIElements();
    }
    void Update()
    {
        if (uIDocument.visualTreeAsset == settingsVisualTree)
        {
            game.settings.volume = settingsVolumeSlider.value;
            game.settings.sensitivity = settingsSensitivitySlider.value;
        }
    }
    void OnHostButton(ClickEvent clickEvent)
    {
        //game.teleporter.LoadIntro();
        game.teleporter.LoadCityFromIntro();
    }
    void OnJoinFriendButton(ClickEvent clickEvent)
    {
        game.network.networkManager.ClientManager.StartConnection(currentSelectedFriend.ToString());
    }
    void OnJoinButton(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = friendsVisualtree;
        friendsList = uIDocument.rootVisualElement.Q<ScrollView>("FriendsList");
        menuBackButton = uIDocument.rootVisualElement.Q<Button>("BackButton");
        menuBackButton.RegisterCallback<ClickEvent>(OnBackButton);
        friendNameLabel = uIDocument.rootVisualElement.Q<Label>("FriendName");
        friendSteamIDLabel = uIDocument.rootVisualElement.Q<Label>("FriendSteamID");
        friendJoinButton = uIDocument.rootVisualElement.Q<Button>("FriendJoinButton");
        friendJoinButton.RegisterCallback<ClickEvent>(OnJoinFriendButton);
        currentSelectedFriend = CSteamID.Nil;
        friendNameLabel.text = "";
        friendSteamIDLabel.text = "";
        friendJoinButton.style.display = DisplayStyle.None;

        if (SteamManager.Initialized)
        {
            game.network.RefreshSteamFriends();
            foreach (var friendSteamID in game.network.steamFriends)
            {
                string friendDisplayName = SteamFriends.GetFriendPersonaName(friendSteamID);
                var friendAvatarTexture = GetSteamImageAsTexture2D(SteamFriends.GetSmallFriendAvatar(friendSteamID));

                Button friend = new Button();
                friend.style.flexDirection = FlexDirection.Row;
                friend.clicked += () => 
                {
                    currentSelectedFriend = friendSteamID;
                    friendNameLabel.text = "Name: " +  friendDisplayName;
                    friendSteamIDLabel.text = "SteamID: " + friendSteamID;
                    friendJoinButton.style.display = DisplayStyle.Flex; 
                };

                Image avatar = new Image();
                avatar.image = friendAvatarTexture;
                friend.Add(avatar);

                Label name = new Label(friendDisplayName);
                friend.Add(name);

                friendsList.Add(friend);
            }
        }
    }
    void OnBackButton(ClickEvent clickEvent)
    {
        menuBackButton.UnregisterCallback<ClickEvent>(OnBackButton);
        uIDocument.visualTreeAsset = mainMenuVisualTree;
        SetupMainmenuUIElements();
    }
    void OnGuideButton(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = guideVisualTree;
        menuBackButton = uIDocument.rootVisualElement.Q<Button>("BackButton");
        menuBackButton.RegisterCallback<ClickEvent>(OnBackButton);
    }
    void OnSettingsButton(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = settingsVisualTree;
        menuBackButton = uIDocument.rootVisualElement.Q<Button>("BackButton");
        menuBackButton.RegisterCallback<ClickEvent>(OnBackButton);

        settingsVolumeSlider = uIDocument.rootVisualElement.Q<Slider>("VolumeSlider");
        settingsSensitivitySlider = uIDocument.rootVisualElement.Q<Slider>("SensitivitySlider");

        settingsVolumeSlider.value = game.settings.volume;
        settingsSensitivitySlider.value = game.settings.sensitivity;
    }
    void OnQuitButton(ClickEvent clickEvent)
    {
        if (game.network.networkManager.IsClientStarted)
            game.network.networkManager.ClientManager.StopConnection();
        if (game.network.networkManager.IsServerStarted)
            game.network.networkManager.ServerManager.StopConnection(true);

        // https://stackoverflow.com/questions/70437401/cannot-finish-the-game-in-unity-using-application-quit
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
