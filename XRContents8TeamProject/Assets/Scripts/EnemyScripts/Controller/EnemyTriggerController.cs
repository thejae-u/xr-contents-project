using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerController : MonoBehaviour
{
    [SerializeField] private int myNumber;

    private EnemySpawner enemySpawner;
    

    private void Start()
    {
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            LogPrintSystem.SystemLogPrint(transform, "Player Entered", ELogType.EnemyAI);
            enemySpawner.Spawn(myNumber);
        }
    }
}
