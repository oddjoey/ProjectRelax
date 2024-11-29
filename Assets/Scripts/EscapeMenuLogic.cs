using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EscapeMenuLogic : MonoBehaviour
{
    GameLogic game;
    private UIDocument uIDocument;
    [SerializeField] private VisualTreeAsset escapeMenuAsset;
    [SerializeField] private VisualTreeAsset settingsMenuAsset;
    private NetworkMenuLogic networkMenu;

    private Button resumeButton;
    private Button saveButton;
    private Button networkButton;
    private Button settingsButton;
    private Button quitButton;
    private Slider volumeSlider;
    private Slider sensitivitySlider;
    private Button returnButton;
    private void OnResume(ClickEvent clickEvent)
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
        game.UI.SetEscapeMenuVisibility(false);
        game.UI.SetCursorVisibility(false);
    }
    private void OnSave(ClickEvent clickEvent)
    {
        game.settings.SavePlayerData();
    }
    private void OnNetwork(ClickEvent clickEvent)
    {
        game.UI.SetEscapeMenuVisibility(false);
        game.UI.SetNetworkMenuVisibility(true);
    }
    private void OnSettings(ClickEvent clickEvent)
    {
        uIDocument.visualTreeAsset = settingsMenuAsset;

        returnButton = uIDocument.rootVisualElement.Q<Button>("Return");
        volumeSlider = uIDocument.rootVisualElement.Q<Slider>("VolumeSlider");
        sensitivitySlider = uIDocument.rootVisualElement.Q<Slider>("SensitivitySlider");

        volumeSlider.value = game.settings.volume;
        sensitivitySlider.value = game.settings.sensitivity;

        returnButton.RegisterCallback<ClickEvent>(OnReturn);
    }
    private void OnQuit(ClickEvent clickEvent)
    {
        /* if (game.LocalPlayer.inVehicle)
            game.LocalPlayer.GetOutVehicle(); */
            
        game.teleporter.LoadMainMenu();
    }
    private void OnReturn(ClickEvent clickEvent)
    {
        returnButton.UnregisterCallback<ClickEvent>(OnReturn);
        uIDocument.visualTreeAsset = escapeMenuAsset;
        SetupUI();
    }
    private void SetupUI()
    {
        resumeButton = uIDocument.rootVisualElement.Q<Button>("Resume");
        saveButton = uIDocument.rootVisualElement.Q<Button>("Save");
        networkButton = uIDocument.rootVisualElement.Q<Button>("Network");
        settingsButton = uIDocument.rootVisualElement.Q<Button>("Settings");
        quitButton = uIDocument.rootVisualElement.Q<Button>("Quit");

        resumeButton.RegisterCallback<ClickEvent>(OnResume);
        saveButton.RegisterCallback<ClickEvent>(OnSave);
        networkButton.RegisterCallback<ClickEvent>(OnNetwork);
        settingsButton.RegisterCallback<ClickEvent>(OnSettings);
        quitButton.RegisterCallback<ClickEvent>(OnQuit);
    }
    // Start is called before the first frame update
    void Start()
    {
        game = GameLogic.instance;

        uIDocument = GameObject.Find("Escape Menu").GetComponent<UIDocument>();

        SetupUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (uIDocument.visualTreeAsset == settingsMenuAsset)
        {
            game.settings.volume = volumeSlider.value;
            game.settings.sensitivity = sensitivitySlider.value;
        }
    }
}
