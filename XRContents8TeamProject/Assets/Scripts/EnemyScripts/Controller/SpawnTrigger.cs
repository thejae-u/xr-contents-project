using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnTrigger : MonoBehaviour
{
    public int stageCount;

    public int SectorCount { get; private set; }
    private bool isRunning;
    private CameraStopper stopper;

    private void Start()
    {
        stopper = GameObject.Find("Camera_Stop").GetComponent<CameraStopper>();
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning) return;


        if (GameManager.Inst.stages[stageCount].sectors.Count > SectorCount)
        {
            if (GameManager.Inst.stages[stageCount].sectors[SectorCount].transform.childCount > 0)
                return;

            SectorCount++;
            GameManager.Inst.EnemySpawn(stageCount, SectorCount);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            GameManager.Inst.EnemySpawn(stageCount, SectorCount);
            isRunning = true;
        }
    }
}
