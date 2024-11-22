using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterLogic : MonoBehaviour
{
    GameLogic game;
    // Internal Variables
    private Vector3 lastLocation;
    private Vector3 lastAngles;
    void Start()
    {
        game = GameLogic.instance;
    }

    void Update()
    {
        
    }
    public void LoadMainMenu()
    {
        StartCoroutine(LoadMainMenuScene());
    }
    public void LoadGarage()
    {
        StartCoroutine(LoadGarageScene());
    }
    public void LoadCityQuest1()
    {
        StartCoroutine(LoadCitySceneFromIntro());
    }
    public void LoadCityQuest2()
    {
        StartCoroutine(LoadCitySceneQuest2());
    }
    public void LoadCityFromGarage()
    {
        StartCoroutine(LoadCitySceneFromGarage());
    }
    public void LoadIntro()
    {
        StartCoroutine(LoadIntroCutScene());
    }
    IEnumerator LoadMainMenuScene()
    {
        game.mainMenu.gameObject.SetActive(false);
        var car = GameObject.Find("Car");
        if (car != null)
            Destroy(car);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.mainMenu.gameObject.SetActive(true);

        if (game.UI.isEscapeMenuOpen)
            game.UI.ToggleMenu();
        if (game.UI.isInventoryMenuOpen)
            game.UI.ToggleInventory();
        if (game.UI.isHotbarOpen)
            game.UI.ToggleHotbar();

        game.UI.SetMinimapVisibility(false);
        game.UI.SetCrosshairVisibility(false);
        game.UI.SetQuestVisiblity(false);

        game.mainMenu.inMainMenu = true;
        game.localPlayer.inGameWorld = false;
        game.localPlayer.inGarage = false;

        game.mainMenu.SetupUIElements();

        yield return null;
    }
    IEnumerator LoadIntroCutScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("IntroCutScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.mainMenu.gameObject.SetActive(false);
        yield return null;
    }
    IEnumerator LoadGarageScene()
    {
        if (game.localPlayer.inVehicle)
            DontDestroyOnLoad(game.localPlayer.currentVehicle.gameObject);

        lastLocation = game.localPlayer.inVehicle ? game.localPlayer.currentVehicle.transform.position : game.localPlayer.transform.position;
        lastAngles = game.localPlayer.inVehicle ? game.localPlayer.currentVehicle.transform.eulerAngles : game.localPlayer.transform.eulerAngles;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Garage");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.mainMenu.inMainMenu = false;
        game.localPlayer.inGameWorld = false;
        game.localPlayer.inGarage = true;
        game.UI.SetMinimapVisibility(false);

        if (game.localPlayer.inVehicle)
        {
            game.localPlayer.currentVehicle.StopMoving();
            game.localPlayer.currentVehicle.transform.position = new Vector3(1.2f, 1, 2.1f);
            game.localPlayer.currentVehicle.transform.eulerAngles = new Vector3(0, 0, 0);
            game.localPlayer.GetOutVehicle();        
        }
        yield return null;
    }
    IEnumerator LoadCitySceneFromGarage()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("City");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.localPlayer.inGameWorld = true;
        game.localPlayer.inGarage = false;
        game.mainMenu.inMainMenu = false;
        game.UI.SetMinimapVisibility(true);

        if (game.localPlayer.inVehicle)
        {
            var garageExit = GameObject.Find("GarageExit");
            game.localPlayer.currentVehicle.transform.position = garageExit.transform.position;
            game.localPlayer.currentVehicle.transform.localEulerAngles = new Vector3(0, 90, 0);
            game.localPlayer.currentVehicle.transform.SetParent(null);
        }
        yield return null;
    }
    IEnumerator LoadCitySceneFromIntro()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("City");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.localPlayer.inGameWorld = true;
        game.localPlayer.inGarage = false;
        game.mainMenu.inMainMenu = false;
        game.localPlayer.inVehicle = false;
        
        game.UI.ToggleCursor();
        game.UI.ToggleHotbar();
        game.mainMenu.gameObject.SetActive(false);
        game.UI.SetMinimapVisibility(true);
        game.UI.SetCrosshairVisibility(true);
        game.UI.SetQuestVisiblity(true);

        var spawnPointPosition = GameObject.Find("Spawn").transform;
        game.localPlayer.transform.position = spawnPointPosition.position + Vector3.up;
        game.localPlayer.lookAngles.x = 180.0f;

        game.quests.StartFirstQuest();
        yield return null;
    }
    IEnumerator LoadCitySceneQuest2()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("City");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.localPlayer.inGameWorld = true;
        game.localPlayer.inGarage = false;
        game.mainMenu.inMainMenu = false;
        game.localPlayer.inVehicle = false;
        
        game.UI.ToggleCursor();
        game.UI.ToggleHotbar();
        game.mainMenu.gameObject.SetActive(false);
        game.UI.SetMinimapVisibility(true);
        game.UI.SetCrosshairVisibility(true);
        game.UI.SetQuestVisiblity(true);

        var spawnPointPosition = GameObject.Find("Spawn").transform;
        game.localPlayer.transform.position = spawnPointPosition.position + Vector3.up;
        game.localPlayer.lookAngles.x = 180.0f;

        game.quests.StartSecondQuest();

        var car = Instantiate(game.carPrefab);
        car.name = "Car";
        car.transform.position = new Vector3(29, 2, 32);
        car.transform.localEulerAngles = new Vector3(0, -90, 0);

        while (car.GetComponent<CarLogic>().drivetrain == null)
            yield return null;

        game.settings.LoadPlayerData();

        yield return null;
    }
}
