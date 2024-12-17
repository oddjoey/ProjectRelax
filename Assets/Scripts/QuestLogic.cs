using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestLogic : MonoBehaviour
{
    GameLogic game;
    private TMP_Text questName;
    private TMP_Text questInfo;
    public int questNumber;
    void Start()
    {
        game = GameLogic.instance;
        
        questName = GameObject.Find("QuestName").GetComponent<TMP_Text>();
        questInfo = GameObject.Find("QuestInfo").GetComponent<TMP_Text>();
    }
    void Update()
    {
    }
    IEnumerator FirstQuest()
    {
        questNumber = 1;

        game.LocalPlayer.RPCSpawnCar();

        while (!GameObject.Find("Car"))
            yield return null;

        GameObject car = GameObject.Find("Car");

        questName.text = "Customize Car";
        questInfo.text = "Get Into Car";
        game.miniMap.waypointObject = car.transform.Find("MinimapMarker").gameObject;

        while (!game.LocalPlayer.inVehicle)
            yield return null;

        questInfo.text = "Turn on your ride! (E)";
        while (!game.LocalPlayer.currentVehicle.engineOn)
            yield return null;

        questInfo.text = "Go to friend's garage to fix up your ride!";
        game.miniMap.waypointObject = GameObject.Find("GarageEntrance").transform.Find("MinimapIcon").gameObject;
        
        while (!game.LocalPlayer.inGarage)
            yield return null;

        game.miniMap.waypointObject = null;
        questInfo.text = "Customize your ride (TAB)";

        while (!game.UI.isInventoryMenuOpen)
            yield return null;

        questInfo.text = "Lets go back out! (F on Garage Door)";

        while(game.LocalPlayer.inGarage)
            yield return null;

        
        StartSecondQuest();

        yield return null;
    }
    IEnumerator SecondQuest()
    {
        questNumber = 2;
        questName.text = "Freeroam!";
        questInfo.text = "Explore the world!";

        yield return null;
    }
    public void StartFirstQuest()
    {
        StartCoroutine(FirstQuest());
    }
    public void StartSecondQuest()
    {
        StartCoroutine(SecondQuest());
    }
}
