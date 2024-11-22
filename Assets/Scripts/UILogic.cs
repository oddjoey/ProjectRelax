
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class UILogic : MonoBehaviour
{
    GameLogic game;
    
    // Variables
    public bool isCursorLocked = false;
    public bool isInventoryMenuOpen = true;
    public bool isEscapeMenuOpen = true;
    public bool isHotbarOpen = true;

    // Internal Variables
    public bool inventoryToggle = false;
    private bool menuToggle = false;

    // Components/GameObjects
    private UIDocument escapeMenu;
    private GameObject inventoryObject;
    private GameObject hotbarObject;
    private GameObject crosshairObject;
    private GameObject tachometerObject;
    private GameObject RPMObject;
    private GameObject KMHObject;
    private GameObject minimapObject;
    private GameObject questNameObject;
    private GameObject questInfoObject;
    public void SetMinimapVisibility(bool visible)
    {
        minimapObject.SetActive(visible);
    }
    public void SetCrosshairVisibility(bool visbile)
    {
        crosshairObject.SetActive(visbile);
    }
    public void SetQuestVisiblity(bool visible)
    {
        questNameObject.GetComponent<TMP_Text>().alpha = visible ? 1 : 0;
        questInfoObject.GetComponent<TMP_Text>().alpha = visible ? 1 : 0;
    }
    void RPMTachoLogic()
    {
        tachometerObject.SetActive(game.localPlayer.inVehicle && !game.localPlayer.inGarage);

        if (game.localPlayer.inVehicle)
        {
            RPMObject.GetComponent<UnityEngine.UI.Image>().color = game.localPlayer.currentVehicle.engineOn ? Color.red : Color.white;
            KMHObject.GetComponent<UnityEngine.UI.Image>().color = game.localPlayer.currentVehicle.engineOn ? Color.red : Color.white;
        }
    }
    public void ToggleCursor()
    {
        isCursorLocked = !isCursorLocked;

        UnityEngine.Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        UnityEngine.Cursor.visible = !isCursorLocked;
    }
    public void ToggleInventory()
    {
        isInventoryMenuOpen = !isInventoryMenuOpen;

        foreach (var item in game.inventory.inventoryGrid.cells)
            if (item != null)
                item.uiObject.SetActive(isInventoryMenuOpen);

        inventoryObject.SetActive(isInventoryMenuOpen);
    }
    public void ToggleMenu()
    {
        isEscapeMenuOpen = !isEscapeMenuOpen;

        escapeMenu.rootVisualElement.style.display = isEscapeMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
        Time.timeScale = isEscapeMenuOpen ? 0 : 1;
    }
    public void ToggleHotbar()
    {
        isHotbarOpen = !isHotbarOpen;

        foreach (var item in game.inventory.hotbarGrid.cells)
           if (item != null)
               item.uiObject.SetActive(isHotbarOpen);
        
        hotbarObject.SetActive(isHotbarOpen);
    }
    void Start()
    {
        game = GameLogic.instance;
        escapeMenu = GameObject.Find("Escape Menu").GetComponent<UIDocument>();

        inventoryObject = GameObject.Find("Inventory");
        crosshairObject = GameObject.Find("Crosshair");
        hotbarObject = GameObject.Find("Hotbar");

        tachometerObject = GameObject.Find("SCC_Canvas");

        RPMObject = GameObject.Find("RPM");
        KMHObject = GameObject.Find("KMH");

        minimapObject = GameObject.Find("Minimap");

        questNameObject = game.quests.transform.Find("QuestName").gameObject;
        questInfoObject = game.quests.transform.Find("QuestInfo").gameObject;
    }
    void Update()
    {
        RPMTachoLogic();

        if (game.inputs.inventory && !inventoryToggle && !isEscapeMenuOpen)
        {
            inventoryToggle = true;
            ToggleCursor();
            ToggleInventory();
            SetCrosshairVisibility(isCursorLocked && !game.localPlayer.inVehicle);
        }

        if (game.inputs.settings && !menuToggle && !isInventoryMenuOpen)
        {
            menuToggle = true;
            ToggleCursor();
            ToggleMenu();
            SetCrosshairVisibility(isCursorLocked && !game.localPlayer.inVehicle);
        }
        
        if (!game.inputs.inventory)
            inventoryToggle = false;
        if (!game.inputs.settings)
            menuToggle = false;

    }
}
