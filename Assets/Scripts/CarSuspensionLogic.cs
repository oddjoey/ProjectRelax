using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSuspensionLogic : MonoBehaviour
{
    GameLogic game;
    public float suspensionHeight = 0.2f;
    public float suspensionDrop = 0.0f;
    public float susCamber = 0.0f;
    public float susOffset = 0.0f;
    WheelCollider FR, FL, RR, RL;
    void Start()
    {
        game = GameLogic.instance;

        var wheelObjects = transform.Find("Wheel Colliders");
        
        FL = wheelObjects.transform.Find("FLC").GetComponent<WheelCollider>();
        RR = wheelObjects.transform.Find("RRC").GetComponent<WheelCollider>();
        FR = wheelObjects.transform.Find("FRC").GetComponent<WheelCollider>();
        RL = wheelObjects.transform.Find("RLC").GetComponent<WheelCollider>();
    }

    void Update()
    {
        FR.suspensionDistance = suspensionHeight;
        FL.suspensionDistance = suspensionHeight;
        RR.suspensionDistance = suspensionHeight;
        RL.suspensionDistance = suspensionHeight;

        FR.transform.localRotation = Quaternion.AngleAxis(susCamber, Vector3.forward);
        FL.transform.localRotation = Quaternion.AngleAxis(susCamber, Vector3.back);
        RR.transform.localRotation = Quaternion.AngleAxis(susCamber, Vector3.forward);
        RL.transform.localRotation = Quaternion.AngleAxis(susCamber, Vector3.back);

        FR.center = new Vector3(susOffset, suspensionDrop, 0);
        FL.center = new Vector3(-susOffset, suspensionDrop, 0);
        RR.center = new Vector3(susOffset, suspensionDrop, 0);
        RL.center = new Vector3(-susOffset, suspensionDrop, 0);
    }
}
