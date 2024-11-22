using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapLogic : MonoBehaviour
{
    GameLogic game;
    Camera minimapCamera;
    public GameObject waypointObject;
    public GameObject pointerObject;
    void Start()
    {
        game = GameLogic.instance;
        minimapCamera = transform.Find("MinimapCam").GetComponent<Camera>();
        pointerObject = transform.Find("Pointer").gameObject;
    }
    void UpdateMarkers()
    {
        if (pointerObject == null)
            return;

        pointerObject.SetActive(waypointObject != null);

        if (waypointObject == null)
            return;

        var localPlayerMarkerPos = minimapCamera.WorldToScreenPoint(game.localPlayer.inVehicle ? game.localPlayer.currentVehicle.transform.position : game.localPlayer.transform.position);
        localPlayerMarkerPos.z = 0;
        var markerPos = minimapCamera.WorldToScreenPoint(waypointObject.transform.position);
        markerPos.z = 0;
        var distance = Vector3.Distance(localPlayerMarkerPos, markerPos);
        pointerObject.SetActive(distance >= 50);
        if (distance >= 50)
        {
            var pointToMarker = localPlayerMarkerPos - markerPos;
            var angleRads = Mathf.Atan2(pointToMarker.y, pointToMarker.x);
            var pointerPosition = new Vector3(Mathf.Cos(angleRads) * -1, Mathf.Sin(angleRads) * -1);
            pointerPosition *= 50;
            pointerObject.transform.localPosition = pointerPosition;
            pointerObject.transform.localRotation = Quaternion.Euler(0, 0, (angleRads * 180 / Mathf.PI) + 90);
        }
    }
    void MoveCamera()
    {
        Vector3 newCamPosition = !game.localPlayer.inVehicle ? game.localPlayer.transform.position : game.localPlayer.currentVehicle.transform.position;
        Vector3 newCamAngle = minimapCamera.transform.localEulerAngles;
        newCamPosition.y += 100;
        newCamAngle.x = 90;
        newCamAngle.y = game.localPlayer.inVehicle ? game.localPlayer.currentVehicle.transform.localEulerAngles.y : game.localPlayer.transform.localEulerAngles.y;
        minimapCamera.transform.position = newCamPosition;
        minimapCamera.transform.localEulerAngles = newCamAngle;
    }
    void Update()
    {
        MoveCamera();
        UpdateMarkers();
    }
}
