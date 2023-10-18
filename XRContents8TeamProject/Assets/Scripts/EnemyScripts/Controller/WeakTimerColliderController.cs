using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakTimerColliderController : MonoBehaviour
{
    public GameObject weakTimer;
    private WeakTimeController timer;

    private void Start()
    {
        timer = weakTimer.GetComponent<WeakTimeController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.transform.CompareTag("Bullet")) return;
        timer.Hit();
        Destroy(gameObject);
    }
}