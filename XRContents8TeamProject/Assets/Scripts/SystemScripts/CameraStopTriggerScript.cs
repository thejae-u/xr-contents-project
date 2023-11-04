using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopTriggerScript : MonoBehaviour
{
    [SerializeField] private int stopStage;
    [SerializeField] private int stopSector;

    private CameraStopper stopper;

    private void Start()
    {
        stopper = GameObject.Find("Camera_Stop").GetComponent<CameraStopper>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            stopper.CameraStop(stopStage, stopSector);
            Destroy(gameObject);
        }
    }
}