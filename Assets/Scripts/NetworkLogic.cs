using FishNet.Managing;
using UnityEngine;

public class NetworkLogic : MonoBehaviour
{
    public NetworkManager networkManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
