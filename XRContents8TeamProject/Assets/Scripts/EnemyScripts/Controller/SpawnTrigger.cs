using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnTrigger : MonoBehaviour
{
    public int stageCount;
    public int SectorCount { get; private set; }
    
    private CameraStopper stopper;

    private void Start()
    {
        stopper = GameObject.Find("Camera_Stop").GetComponent<CameraStopper>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            GameManager.Inst.EnemySpawn(stageCount, SectorCount);
            Destroy(gameObject);
        }
    }
}
