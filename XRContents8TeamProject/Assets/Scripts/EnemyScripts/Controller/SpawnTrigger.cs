using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public int myNum;
    
    public EnemySpawner enemySpawner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            enemySpawner.Spawn(myNum);
        }
    }
}
