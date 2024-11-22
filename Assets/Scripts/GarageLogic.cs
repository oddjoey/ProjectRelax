using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GarageLogic : MonoBehaviour
{
    [SerializeField] GameLogic game;
    // Components
    CarLogic car;
    // Variables

    // Internal Variables
    private bool cityHeld = false;
    private int rayCastInteractableMask;

    void Start()
    {
        game = GameLogic.instance;

        var carObject = GameObject.Find("Car");

        if (carObject != null)
        {
            car = carObject.GetComponent<CarLogic>();
        }

        rayCastInteractableMask = 1 << LayerMask.NameToLayer("Interactable");
    }
    void Update()
    {
        InteractLogic();

        if (!game.inputs.use)
            cityHeld = false;
    }
    void InteractLogic()
    {
        if (game.localPlayer == null)
            return;
            
        RaycastHit hit;
        if (!Physics.Raycast(game.localPlayer.camera.transform.position, game.localPlayer.camera.transform.TransformDirection(Vector3.forward), out hit, game.localPlayer.interactDistance, rayCastInteractableMask))
            return;

        if (hit.transform.tag == "Exit")
        {
            if (game.inputs.use && !cityHeld)
            {
                game.localPlayer.GetInVehicle(GameObject.Find("Car").GetComponent<CarLogic>());
                game.teleporter.LoadCityFromGarage();
                cityHeld = true;
            }
        }
    }
}
