using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class NetworkLogic : MonoBehaviour
{
    GameLogic game;
    public NetworkManager networkManager;
    public void OnStartServer()
    {
        Debug.Log("Server: Started!");

        // Load our scene "globally" into the server.
        // Global scenes are loaded for any current and future clients.
        // https://fish-networking.gitbook.io/docs/manual/guides/scene-management/loading-scenes#global-scenes
        SceneLoadData sld = new SceneLoadData("City");

        // Unload all current scenes that the client had loaded and replace with ours.
        // https://fish-networking.gitbook.io/docs/manual/guides/scene-management/loading-scenes#replace-all
        sld.ReplaceScenes = ReplaceOption.All;
        networkManager.SceneManager.LoadGlobalScenes(sld);
    }
    // When a client loads the start scene.
    private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        // Who is running this function ?
        if (asServer)
        {
            Debug.Log("Server: Client[" + conn.ClientId + "] loaded start scene");

            // Spawn our client into the scene.
            // Since our ObserverManager component inside our NetworkManager has the SceneCondition, any client
            // loaded into the same scene as the server will be an Observer of all the objects in the scene.
            // https://fish-networking.gitbook.io/docs/manual/components/network-observer#conditions

            // Instead of creating and destorying objects constantly, it is much more performance efficient to
            // instead grab an object from a pool of objects, set its properties, and enable it. When finished,
            // unenable and return to the pool to be used again another time. This is called obeject pooling.
            // https://fish-networking.gitbook.io/docs/manual/guides/spawning/object-pooling
            NetworkObject nObj = networkManager.GetPooledInstantiated(game.playerPrefab, true);
            networkManager.ServerManager.Spawn(nObj, conn);
        }
        else
        {
            Debug.Log("Client[" + conn.ClientId + "]: loaded start scene");

            game.mainMenu.gameObject.SetActive(false);
            game.UI.SetNetworkMenuVisibility(false);
            game.UI.SetInventoryVisibility(true);
            game.UI.SetHotbarVisibility(true);
            game.UI.SetMinimapVisibility(true);
            game.UI.SetCrosshairVisibility(true);
            game.UI.SetQuestVisiblity(true);
            game.UI.SetCursorVisibility(false);
        }
    }
    void Start()
    {
        game = GameLogic.instance;
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
    }
    void Update()
    {
        
    }
}
