using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuLogic : MonoBehaviour
{
    GameLogic game;
    
    private UIDocument uIDocument;
    private Button newButton;
    private Button loadButton;
    private Button guideButton;
    private Button settingsButton;
    private Button quitButton;
    private Button backButton;
    private Slider volumeSlider;
    private Slider sensitivitySlider;
    [SerializeField] private VisualTreeAsset mainMenuVisualTree;
    [SerializeField] private VisualTreeAsset guideVisualTree;
    [SerializeField] private VisualTreeAsset settingsVisualTree;
    public bool inMainMenu = true;
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
    public void SetupUIElements()
    {
        newButton = uIDocument.rootVisualElement.Q<Button>("NewButton");
        loadButton = uIDocument.rootVisualElement.Q<Button>("LoadButton");
        guideButton = uIDocument.rootVisualElement.Q<Button>("GuideButton");
        settingsButton = uIDocument.rootVisualElement.Q<Button>("SettingsButton");
        quitButton = uIDocument.rootVisualElement.Q<Button>("QuitButton");

        newButton.RegisterCallback<ClickEvent>(OnNewButton);
        loadButton.RegisterCallback<ClickEvent>(OnLoadButton);
        guideButton.RegisterCallback<ClickEvent>(OnGuideButton);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsButton);
        quitButton.RegisterCallback<ClickEvent>(OnQuitButton);
    }
    void Start()
    {
        game = GameLogic.instance;
        uIDocument = GetComponent<UIDocument>();

        SetupUIElements();

        game.UI.ToggleMenu();
        game.UI.ToggleInventory();
        game.UI.ToggleHotbar();
        game.UI.SetMinimapVisibility(false);
        game.UI.SetCrosshairVisibility(false);
        game.UI.SetQuestVisiblity(false);
    }
    void Update()
    {
        if (uIDocument.visualTreeAsset == settingsVisualTree)
        {
            game.settings.volume = volumeSlider.value;
            game.settings.sensitivity = sensitivitySlider.value;
        }
    }
    void OnNewButton(ClickEvent clickEvent)
    {
        game.teleporter.LoadIntro();
    }
    void OnLoadButton(ClickEvent clickEvent)
    {
        if (!game.settings.DataExists())
            return;

        game.settings.LoadPlayerData();

        switch (game.quests.questNumber)
        {
            case 0:
                game.teleporter.LoadIntro();
            break;
            case 1:
                game.teleporter.LoadCityQuest1();
            break;
            case 2:
                game.teleporter.LoadCityQuest2();
            break;
        }
    }
    void OnBackButton(ClickEvent clickEvent)
    {
        backButton.UnregisterCallback<ClickEvent>(OnBackButton);
        uIDocument.visualTreeAsset = mainMenuVisualTree;
        SetupUIElements();
    }
    void OnGuideButton(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = guideVisualTree;
        backButton = uIDocument.rootVisualElement.Q<Button>("ReturnButton");
        backButton.RegisterCallback<ClickEvent>(OnBackButton);
    }
    void OnSettingsButton(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = settingsVisualTree;
        backButton = uIDocument.rootVisualElement.Q<Button>("ReturnButton");
        backButton.RegisterCallback<ClickEvent>(OnBackButton);

        volumeSlider = uIDocument.rootVisualElement.Q<Slider>("VolumeSlider");
        sensitivitySlider = uIDocument.rootVisualElement.Q<Slider>("SensitivitySlider");
        volumeSlider.value = game.settings.volume;
        sensitivitySlider.value = game.settings.sensitivity;
    }
    void OnQuitButton(ClickEvent clickEvent)
    {
        // https://stackoverflow.com/questions/70437401/cannot-finish-the-game-in-unity-using-application-quit
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
