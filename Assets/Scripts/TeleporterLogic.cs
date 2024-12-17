using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterLogic : MonoBehaviour
{
    GameLogic game;
    // Internal Variables
    private Vector3 lastLocation;
    private Vector3 lastAngles;
    public bool sceneChanging;

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
    public void LoadCityFromIntro()
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
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.mainMenu.gameObject.SetActive(true);

        game.UI.SetEscapeMenuVisibility(false);
        game.UI.SetInventoryVisibility(false);
        game.UI.SetHotbarVisibility(false);
        game.UI.SetMinimapVisibility(false);
        game.UI.SetCrosshairVisibility(false);
        game.UI.SetQuestVisiblity(false);
        game.UI.SetCursorVisibility(true);
        game.UI.SetNetworkMenuVisibility(false);

        game.mainMenu.SetupMainmenuUIElements();

        yield return null;
    }
    IEnumerator LoadIntroCutScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("IntroCutScene");

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
        lastLocation = game.LocalPlayer.inVehicle ? game.LocalPlayer.currentVehicle.transform.position : game.LocalPlayer.transform.position;
        lastAngles = game.LocalPlayer.inVehicle ? game.LocalPlayer.currentVehicle.transform.eulerAngles : game.LocalPlayer.transform.eulerAngles;

        sceneChanging = true;
        Debug.Log("Calling server's rpc for garage from client!");
        game.LocalPlayer.RPCLoadGarageFromCity();

        while (sceneChanging)
        {
            yield return null;
        }

        Debug.Log("Garage should be loaded!");
    
        game.LocalPlayer.currentVehicle.StopMoving();
        game.LocalPlayer.currentVehicle.transform.position = new Vector3(1.2f, 4, 2.1f);
        game.LocalPlayer.currentVehicle.transform.eulerAngles = new Vector3(0, 0, 0);
        game.LocalPlayer.GetOutVehicle();

        game.mainMenu.inMainMenu = false;
        game.LocalPlayer.inGameWorld = false;
        game.LocalPlayer.inGarage = true;
        game.UI.SetMinimapVisibility(false);

        yield return null;
    }
    IEnumerator LoadCitySceneFromGarage()
    {
        sceneChanging = true;
        Debug.Log("Calling server's rpc for city from client!");
        game.LocalPlayer.RPCLoadCityFromGarage();

        // Wait until the asynchronous scene fully loads
        while (sceneChanging)
        {
            yield return null;
        }

        game.LocalPlayer.inGameWorld = false;
        game.LocalPlayer.inGarage = true;
        
        game.UI.SetMinimapVisibility(false);
        game.UI.SetCrosshairVisibility(true);
        game.UI.SetQuestVisiblity(true);

        if (game.LocalPlayer.inVehicle)
        {
            var garageExit = GameObject.Find("GarageExit");
            game.LocalPlayer.currentVehicle.transform.position = garageExit.transform.position;
            game.LocalPlayer.currentVehicle.transform.localEulerAngles = new Vector3(0, 90, 0);
            game.LocalPlayer.currentVehicle.transform.SetParent(null);
        }
        yield return null;
    }
    IEnumerator LoadCitySceneFromIntro()
    {
    /*     AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("City");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.mainMenu.gameObject.SetActive(false);
        game.UI.SetCursorVisibility(false);
        game.mainMenu.inMainMenu = false; */

        game.network.networkManager.ServerManager.StartConnection();

        while (!game.network.networkManager.IsServerStarted)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);
        
        game.network.networkManager.ClientManager.StartConnection();

        yield return new WaitForSeconds(1);

        game.LocalPlayer.inGameWorld = true;
        game.LocalPlayer.inGarage = false;
        game.LocalPlayer.inVehicle = false;

        var spawnPointPosition = GameObject.Find("Spawn").transform;
        game.LocalPlayer.transform.position = spawnPointPosition.position + Vector3.up;
        game.LocalPlayer.lookAngles.x = 180.0f;

        game.quests.StartFirstQuest();
        yield return null;
    }
    IEnumerator LoadCitySceneQuest2()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("City");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        game.LocalPlayer.inGameWorld = true;
        game.LocalPlayer.inGarage = false;
        game.mainMenu.inMainMenu = false;
        game.LocalPlayer.inVehicle = false;
        
        game.UI.SetCursorVisibility(false);
        game.UI.SetHotbarVisibility(true);
        game.mainMenu.gameObject.SetActive(false);
        game.UI.SetMinimapVisibility(true);
        game.UI.SetCrosshairVisibility(true);
        game.UI.SetQuestVisiblity(true);

        var spawnPointPosition = GameObject.Find("Spawn").transform;
        game.LocalPlayer.transform.position = spawnPointPosition.position + Vector3.up;
        game.LocalPlayer.lookAngles.x = 180.0f;

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
