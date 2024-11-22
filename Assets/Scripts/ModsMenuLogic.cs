using UnityEngine;
using UnityEngine.UIElements;

public class ModsMenuLogic : MonoBehaviour
{
    GameLogic game;
    public UIDocument      uIDocument;
    private Slider          engineHPSlider;
    private Slider          engineMaxRPMSlider;
    private Slider          brakePowerSlider;
    private Slider          susDropSlider;
    private Slider          susCamberSlider;
    private Slider          susOffsetSlider;
    private DropdownField   drivetrainDropdown;
    private Slider          transFinalDriveRatioSlider;
    private DropdownField   transGearboxDropdown;
    private Slider          ecuMaxSpeed;
    private CarLogic        garageCar;
    public bool isModsMenuOpen = true;
    private bool modsMenuHeld = false;
    void Start()
    {
        game = GameLogic.instance;
        uIDocument = GetComponent<UIDocument>();

        engineHPSlider = uIDocument.rootVisualElement.Q<Slider>("EngineHPSlider");
        engineMaxRPMSlider = uIDocument.rootVisualElement.Q<Slider>("EngineMaxRPMSlider");
        brakePowerSlider = uIDocument.rootVisualElement.Q<Slider>("BrakePowerSlider");
        susDropSlider = uIDocument.rootVisualElement.Q<Slider>("SusDropSlider");
        susCamberSlider = uIDocument.rootVisualElement.Q<Slider>("SusCamberSlider");
        susOffsetSlider = uIDocument.rootVisualElement.Q<Slider>("SusOffsetSlider");
        drivetrainDropdown = uIDocument.rootVisualElement.Q<DropdownField>("DrivetrainDropdown");
        transFinalDriveRatioSlider = uIDocument.rootVisualElement.Q<Slider>("TransFinalDriveRatioSlider");
        transGearboxDropdown = uIDocument.rootVisualElement.Q<DropdownField>("TransGearboxDropdown");
        ecuMaxSpeed = uIDocument.rootVisualElement.Q<Slider>("ECUMaxSpeed");

        garageCar = GameObject.Find("Car").GetComponent<CarLogic>();

        ToggleModsMenu();
    }
    void Update()
    {
        if (game.inputs.inventory && !modsMenuHeld)
        {
            modsMenuHeld = true;
            ToggleModsMenu();
        }

        if (isModsMenuOpen)
            SetModValues();

        if (!game.inputs.inventory)
            modsMenuHeld = false;
    }
    void SetModValues()
    {
        garageCar.drivetrain.engineTorque = engineHPSlider.value * 10;
        garageCar.drivetrain.maximumEngineRPM = engineMaxRPMSlider.value;
        garageCar.drivetrain.brakeTorque = brakePowerSlider.value * 100;
        garageCar.wheels.suspensionDrop = susDropSlider.value;
        garageCar.wheels.susCamber = susCamberSlider.value * -1;
        garageCar.wheels.susOffset = susOffsetSlider.value;
        // = drivetrainDropdown.index;
        garageCar.drivetrain.finalDriveRatio = transFinalDriveRatioSlider.value;
        // = transGearboxDropdown.index;
        garageCar.drivetrain.maximumSpeed = ecuMaxSpeed.value;
    }
    void UpdateMenuValues()
    {
        engineHPSlider.value = garageCar.drivetrain.engineTorque / 10;
        engineMaxRPMSlider.value = garageCar.drivetrain.maximumEngineRPM;
        brakePowerSlider.value = garageCar.drivetrain.brakeTorque / 100;
        susDropSlider.value = garageCar.wheels.suspensionDrop;
        susCamberSlider.value = garageCar.wheels.susCamber * -1;
        susOffsetSlider.value = garageCar.wheels.susOffset;
        drivetrainDropdown.index = 0;
        transFinalDriveRatioSlider.value = garageCar.drivetrain.finalDriveRatio;
        transGearboxDropdown.index = 0;
        ecuMaxSpeed.value = garageCar.drivetrain.maximumSpeed;
    }
    public void ToggleModsMenu()
    {
        isModsMenuOpen = !isModsMenuOpen;

        uIDocument.rootVisualElement.style.display = isModsMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;

        if (isModsMenuOpen)
            UpdateMenuValues();

        game.UI.SetQuestVisiblity(!isModsMenuOpen);
    }
}
