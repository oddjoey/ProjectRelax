
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
    public bool isNetworkMenuOpen = true;

    // Internal Variables
    public bool inventoryToggle = false;
    private bool menuToggle = false;

    // Components/GameObjects
    private UIDocument escapeMenu;
    private UIDocument networkMenu;
    private GameObject inventoryObject;
    private GameObject hotbarObject;
    private GameObject hotbarSelectedObject;
    private GameObject crosshairObject;
    private GameObject tachoSpeedoObject;
    private GameObject RPMObject;
    private GameObject KMHObject;
    private GameObject minimapObject;
    private GameObject questNameObject;
    private GameObject questInfoObject;
    public void SetMinimapVisibility(bool visible)
    {
        minimapObject.SetActive(visible);
    }
    public void SetCrosshairVisibility(bool visible)
    {
        crosshairObject.SetActive(visible);
    }
    public void SetQuestVisiblity(bool visible)
    {
        questNameObject.GetComponent<TMP_Text>().alpha = visible ? 1 : 0;
        questInfoObject.GetComponent<TMP_Text>().alpha = visible ? 1 : 0;
    }
    public void SetTachoSpeedoVisiblity(bool visible)
    {
        tachoSpeedoObject.SetActive(visible);
    }
    void RPMTachoLogic()
    {
        if (!game.LocalPlayer)
            return;

        SetTachoSpeedoVisiblity(game.LocalPlayer.inVehicle && !game.LocalPlayer.inGarage);

        if (game.LocalPlayer.inVehicle)
        {
            RPMObject.GetComponent<UnityEngine.UI.Image>().color = game.LocalPlayer.currentVehicle.engineOn ? Color.red : Color.white;
            KMHObject.GetComponent<UnityEngine.UI.Image>().color = game.LocalPlayer.currentVehicle.engineOn ? Color.red : Color.white;
        }
    }
    public void SetCursorVisibility(bool visible)
    {
        isCursorLocked = !visible;

        UnityEngine.Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        UnityEngine.Cursor.visible = !isCursorLocked;
    }
    public void SetInventoryVisibility(bool visible)
    {
         isInventoryMenuOpen = visible;

        /*foreach (var item in game.inventory.inventoryGrid.cells)
            if (item != null)
                item.uiObject.SetActive(isInventoryMenuOpen);*/

        inventoryObject.SetActive(isInventoryMenuOpen);
    }
    public void SetNetworkMenuVisibility(bool visible)
    {
        isNetworkMenuOpen = visible;

        networkMenu.rootVisualElement.style.display = isNetworkMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
        Time.timeScale = isNetworkMenuOpen ? 0 : 1;
    }
    public void SetEscapeMenuVisibility(bool visible)
    {
        isEscapeMenuOpen = visible;

        escapeMenu.rootVisualElement.style.display = isEscapeMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
        Time.timeScale = isEscapeMenuOpen ? 0 : 1;
    }
    public void SetHotbarVisibility(bool visible)
    {
        isHotbarOpen = visible;

/*         foreach (var item in game.inventory.hotbarGrid.cells)
           if (item != null)
               item.uiObject.SetActive(isHotbarOpen); */
        
        hotbarObject.SetActive(isHotbarOpen);

        var color = hotbarSelectedObject.GetComponent<UnityEngine.UI.Image>().color;
        color.a = isHotbarOpen ? 0.2f : 0;
        hotbarSelectedObject.GetComponent<UnityEngine.UI.Image>().color = color;
    }
    void Start()
    {
        game = GameLogic.instance;
        escapeMenu = GameObject.Find("Escape Menu").GetComponent<UIDocument>();
        inventoryObject = GameObject.Find("Inventory");
        crosshairObject = GameObject.Find("Crosshair");
        hotbarObject = GameObject.Find("Hotbar");

        tachoSpeedoObject = GameObject.Find("SCC_Canvas");

        RPMObject = GameObject.Find("RPM");
        KMHObject = GameObject.Find("KMH");

        minimapObject = GameObject.Find("Minimap");

        questNameObject = GameObject.Find("QuestName").gameObject;
        questInfoObject = GameObject.Find("QuestInfo").gameObject;

        hotbarSelectedObject = GameObject.Find("Selected Hotbar Item");

        networkMenu = GameObject.Find("Network Menu").GetComponent<UIDocument>();

        SetTachoSpeedoVisiblity(false);
    }
    void Update()
    {
        RPMTachoLogic();

        // Toggle Inventory
        if (game.inputs.inventory && !inventoryToggle && !isEscapeMenuOpen)
        {
            inventoryToggle = true;
            SetCursorVisibility(!isInventoryMenuOpen);
            SetInventoryVisibility(!isInventoryMenuOpen);
            //SetCrosshairVisibility(isCursorLocked && !game.LocalPlayer.inVehicle);
        }

        // Toggle Escape Menu
        if (game.inputs.settings && !menuToggle && !isInventoryMenuOpen)
        {
            menuToggle = true;
            SetCursorVisibility(!isEscapeMenuOpen);
            SetEscapeMenuVisibility(!isEscapeMenuOpen);
            //SetCrosshairVisibility(isCursorLocked && !game.LocalPlayer.inVehicle);
        }
        
        if (!game.inputs.inventory)
            inventoryToggle = false;
        if (!game.inputs.settings)
            menuToggle = false;
    }
}
