using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopper : MonoBehaviour
{
    [SerializeField] private List<GameObject> cameraTriggers;
    private PlayerManager playerManager;
    private CameraController cameraController;

    private int curStage;
    private int curSection;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
        if (!cameraController.IsCameraStop) return;

        if (GameManager.Inst.stages[curStage].sectors.Count > curSection)
        {
            if (GameManager.Inst.stages[curStage].sectors[curSection].transform.childCount == 0)
            {
                cameraController.IsCameraStop = false;
            }
        }
    }

    public void CameraStop(int stageCount, int sectorCount)
    {
        cameraController.IsCameraStop = true;
        curStage = stageCount;
        curSection = sectorCount;
    }
}
