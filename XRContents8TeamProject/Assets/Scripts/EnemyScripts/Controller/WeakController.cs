using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class WeakController : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;

    private EEnemyController parentComponent;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        parentComponent = parentObject.GetComponent<EEnemyController>();
        
        parentComponent.WeakBreak();
    }
}
