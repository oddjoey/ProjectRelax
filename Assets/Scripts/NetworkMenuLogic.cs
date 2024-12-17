using FishNet.Transporting;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkMenuLogic : MonoBehaviour
{
    GameLogic game;
    private UIDocument uIDocument;
    private Toggle networkInfoToggle;
    private Button serverButton;
    private Button clientButton;
    private Button returnButton;
    private TMP_Text networkInfoText;
    private LocalConnectionState clientState = LocalConnectionState.Stopped;
    private LocalConnectionState serverState = LocalConnectionState.Stopped;
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        clientState = obj.ConnectionState;
        switch (clientState)
        {
            case LocalConnectionState.Stopping:
                clientButton.text = "Start Client";
            break;
            case LocalConnectionState.Started:
                clientButton.text = "Stop Client";
            break;
        }
        Debug.Log("Client connection: " + obj.ConnectionState);
    }
    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        serverState = obj.ConnectionState;
        switch (serverState)
        {
            case LocalConnectionState.Stopping:
                serverButton.text = "Start Server";
            break;
            case LocalConnectionState.Started:
                serverButton.text = "Stop Server";
                game.network.OnStartServer();
            break;
        }
        Debug.Log("Server connection: " + obj.ConnectionState);
    }

    void OnClient(ClickEvent clickEvent)
    {
        switch (clientState)
        {
            case LocalConnectionState.Stopped:
                game.network.networkManager.ClientManager.StartConnection();
            break;
            case LocalConnectionState.Started:
                game.network.networkManager.ClientManager.StopConnection();
            break;
        }
    }
    void OnServer(ClickEvent clickEvent)
    {
        switch (serverState)
        {
            case LocalConnectionState.Stopped:
                game.network.networkManager.ServerManager.StartConnection();
            break;
            case LocalConnectionState.Started:
                game.network.networkManager.ServerManager.StopConnection(true);
            break;
        }
        
    }
    void OnReturn(ClickEvent clickEvent)
    {
        game.UI.SetNetworkMenuVisibility(false);
        game.UI.SetEscapeMenuVisibility(true);
    }
    void Start()
    {
        game = GameLogic.instance;

        uIDocument = GameObject.Find("Network Menu").GetComponent<UIDocument>();
        networkInfoText = GameObject.Find("Network Info").GetComponent<TMP_Text>();

        networkInfoToggle = uIDocument.rootVisualElement.Q<Toggle>("NetworkInfoToggle");
        serverButton = uIDocument.rootVisualElement.Q<Button>("ServerButton");
        clientButton = uIDocument.rootVisualElement.Q<Button>("ClientButton");
        returnButton = uIDocument.rootVisualElement.Q<Button>("ReturnButton");

        serverButton.RegisterCallback<ClickEvent>(OnServer);
        clientButton.RegisterCallback<ClickEvent>(OnClient);
        returnButton.RegisterCallback<ClickEvent>(OnReturn);
        
        game.network.networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        game.network.networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
    }

    void Update()
    {
        networkInfoText.alpha = networkInfoToggle.value ? 1 : 0;
        
        if (networkInfoToggle.value)
        {
            networkInfoText.text = 
            "Steam Initialized: " + SteamManager.Initialized + '\n' +
            "Client Status: " + clientState + '\n' +
            "Server Status: " + serverState + '\n' +
            "Is Client Started: " + game.network.networkManager.IsClientStarted + '\n' +
            "Is Server Started: " + game.network.networkManager.IsServerStarted + '\n' +
            "Is Connection Active: " + game.network.networkManager.ClientManager.Connection.IsActive + '\n';

            networkInfoText.text += "Owned Objects: \n";
            foreach (var obj in game.network.networkManager.ClientManager.Connection.Objects)
                networkInfoText.text += obj.name + '\n';

            networkInfoText.text += "Connected Clients: \n";
            foreach (var client in game.network.networkManager.ClientManager.Clients)
                networkInfoText.text += "Client " + client.Key + " " + client.Value.GetAddress() + '\n';

            if (SteamManager.Initialized)
            {
                networkInfoText.text += "SteamID: " + SteamUser.GetSteamID() + '\n' +
                "Steam Friends: \n";
                
                int numOfFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
                for (int i = 0; i < numOfFriends; i++)
                {
                    CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                    string friendDisplayName = SteamFriends.GetFriendPersonaName(friendSteamID);
                    networkInfoText.text += friendDisplayName + ':' + friendSteamID + '\n';
                }
            }
        }   
    }
}
