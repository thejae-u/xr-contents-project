using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopTriggerScript : MonoBehaviour
{
    public int mySectionNumber;
    private CameraStopper stopper;
    
    void Start()
    {
        stopper = GameObject.Find("Camera_Stop").GetComponent<CameraStopper>();    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            stopper.StopCamera(mySectionNumber);
        }
    }
}
