using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnTrigger : MonoBehaviour
{
    public int stageCount;
    private int sectorCount;
    private bool isRunning;

    private void Start()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning) return;
        if (GameManager.Inst.stages[stageCount].sectors[sectorCount].transform.childCount > 0) return;
        sectorCount++;
        GameManager.Inst.EnemySpawn(stageCount, sectorCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            CameraController.Inst.CameraTransition();
            GameManager.Inst.EnemySpawn(stageCount, sectorCount);
            isRunning = true;
        }
    }
}
