using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> sectors;

    public void Spawn(int triggerNumber)
    {
        sectors[triggerNumber].SetActive(true);
    }
}
