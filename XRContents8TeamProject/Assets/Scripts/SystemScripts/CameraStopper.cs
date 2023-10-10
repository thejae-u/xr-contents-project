using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopper : MonoBehaviour
{
    public List<GameObject> stopSections;

    private CameraController cameraController;
    
    private int curCount;
    private int curSection;

    private void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
        if (cameraController.IsCameraStop)
        {
            CheckEnemy();
        }
    }
    
    

    public void StopCamera(int section)
    {
        cameraController.IsCameraStop = true;
        curSection = section;
    }

    private void CheckEnemy()
    {
        if (stopSections[curSection].transform.childCount == 0)
        {
            cameraController.IsCameraStop = false;
        }
    }
}
