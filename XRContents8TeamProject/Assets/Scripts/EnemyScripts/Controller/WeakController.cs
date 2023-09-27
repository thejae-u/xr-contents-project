using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class WeakController : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;

    private EEnemyController parentComponent;
    
    /*private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.transform.CompareTag("Bullet")) return;
        parentComponent = parentObject.GetComponent<EEnemyController>();
        LogPrintSystem.SystemLogPrint(transform, "Weak Brake", ELogType.EnemyAI);
        parentComponent.WeakBreak();
    }*/


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.CompareTag("Bullet")) return;
        parentComponent = parentObject.GetComponent<EEnemyController>();
        LogPrintSystem.SystemLogPrint(transform, "Weak Brake", ELogType.EnemyAI);
        parentComponent.WeakBreak();
    }
}
