using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopper : MonoBehaviour
{
    private CameraController cameraController;

    private void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
        if (GameManager.Inst.CheckEnemyCount() == 0 && cameraController.IsCameraStop)
            cameraController.IsCameraStop = false;
    }

    public void CameraStop()
    {
        cameraController.IsCameraStop = true;
    }
}
