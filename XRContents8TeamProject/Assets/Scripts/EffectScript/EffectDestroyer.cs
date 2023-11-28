using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{

    
    private void Update()
    {
        if (transform.GetComponent<ParticleSystem>().isPlaying) return;
        
        Destroy(gameObject);
    }
}
