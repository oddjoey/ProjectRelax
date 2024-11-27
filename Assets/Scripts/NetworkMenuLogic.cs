using FishNet.Transporting;
using UnityEditor.UIElements;
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
    private LocalConnectionState _clientState = LocalConnectionState.Stopped;
    private LocalConnectionState _serverState = LocalConnectionState.Stopped;
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        _clientState = obj.ConnectionState;
    }
    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        _serverState = obj.ConnectionState;
    }

    void OnServer(ClickEvent clickEvent)
    {
        Debug.Log("server");
        game.network.networkManager.ServerManager.StartConnection();
    }
    void OnClient(ClickEvent clickEvent)
    {
        game.network.networkManager.ClientManager.StartConnection();
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
