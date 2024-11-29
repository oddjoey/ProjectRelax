using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkMenuLogic : MonoBehaviour
{
    GameLogic game;
    private UIDocument uIDocument;
    private Toggle debugInfoToggle;
    private Button serverButton;
    private Button clientButton;
    private Button returnButton;
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

        debugInfoToggle = uIDocument.rootVisualElement.Q<Toggle>("Debug Toggle");
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
        
    }
}
