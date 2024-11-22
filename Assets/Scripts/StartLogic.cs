using UnityEngine;

public class StartLogic : MonoBehaviour
{
    GameLogic game;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game = GameLogic.instance;
        game.teleporter.LoadMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
