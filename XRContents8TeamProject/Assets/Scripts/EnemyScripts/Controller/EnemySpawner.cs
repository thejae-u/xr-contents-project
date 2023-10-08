using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> sectors;

    public void Spawn(int triggerNumber)
    {
        LogPrintSystem.SystemLogPrint(transform, "Called Spawn", ELogType.EnemyAI);
        sectors[triggerNumber].SetActive(true);
    }
}
