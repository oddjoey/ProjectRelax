using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapLogic : MonoBehaviour
{
    GameLogic game;
    public Camera minimapCamera;
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
        var localPlayerMarkerPos = minimapCamera.WorldToScreenPoint(game.LocalPlayer.inVehicle ? game.LocalPlayer.currentVehicle.transform.position : game.LocalPlayer.transform.position);
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
        Vector3 newCamPosition = !game.LocalPlayer.inVehicle ? game.LocalPlayer.transform.position : game.LocalPlayer.currentVehicle.transform.position;
        Vector3 newCamAngle = minimapCamera.transform.localEulerAngles;
        newCamPosition.y += 100;
        newCamAngle.x = 90;
        newCamAngle.y = game.LocalPlayer.inVehicle ? game.LocalPlayer.currentVehicle.transform.localEulerAngles.y : game.LocalPlayer.transform.localEulerAngles.y;
        minimapCamera.transform.position = newCamPosition;
        minimapCamera.transform.localEulerAngles = newCamAngle;
    }
    void Update()
    {
        if (!game.network.HasPlayerSpawned())
            return;

        MoveCamera();
        UpdateMarkers();
    }
}
