
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneLogic : MonoBehaviour
{
    GameLogic game;
    [SerializeField] List<Camera> cutSceneCameras;
    [SerializeField] List<GameObject> cutSceneObjects;
    void Start()
    {
        game = GameLogic.instance;

        StartCoroutine(FirstCutScene());
    }
    void Update()
    {
        
    }
    void SwitchToCamera(int index)
    {
        for (int i = 0; i < cutSceneCameras.Count; i++)
            cutSceneCameras[i].enabled = i == index;
    }
    void TriggerAnimation(Animator animator, String name)
    {
        animator.SetTrigger(name);
    }
    void BoolAnimation(Animator animator, String name, bool value)
    {
       animator.SetBool(name, value);
    }
    IEnumerator FirstCutScene()
    {
        var aj = cutSceneObjects[0].GetComponent<Animator>();
        //var door = cutSceneObjects[1];
        var claire = cutSceneObjects[1].GetComponent<Animator>();

        SwitchToCamera(3);
        yield return new WaitForSecondsRealtime(3);
        var canvasObj = GameObject.Find("Canvas");
        canvasObj.SetActive(false);

        SwitchToCamera(0);
        yield return new WaitForSecondsRealtime(3);
        SwitchToCamera(1);
        yield return new WaitForSecondsRealtime(1);

        // open door
        /*Vector3 doorAngles = door.transform.localEulerAngles;
        while (doorAngles.y > -75)
        {
            doorAngles.y = Mathf.Lerp(doorAngles.y, -90, 1 - Mathf.Pow(0.2f, Time.deltaTime));
            door.transform.localEulerAngles = doorAngles;
            yield return null;
        }*/

        // mom walks in
        TriggerAnimation(claire, "MoveToFirstPosition");

        TriggerAnimation(aj, "Situp");
        aj.transform.position = new Vector3(60.2f,3,14.9f);

        yield return new WaitForSecondsRealtime(1);

        SwitchToCamera(2);

        yield return new WaitForSecondsRealtime(1);

        TriggerAnimation(claire, "GiveKeys");

        yield return new WaitForSecondsRealtime(2);

        game.teleporter.LoadCityQuest1();

        yield return null;
    }
}
