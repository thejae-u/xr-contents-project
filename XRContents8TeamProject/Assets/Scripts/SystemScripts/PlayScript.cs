using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScript : MonoBehaviour
{
    public Sound mySound;

    private AudioSource source;

    private bool isPlaying;

    private void Start()
    {
        mySound = new Sound();
        source = transform.GetComponent<AudioSource>();
        isPlaying = false;
    }

    private void Update()
    {
        if (isPlaying)
        {
            switch (mySound.playType)
            {
                case EPlayType.Once:
                    if (source.isPlaying) return;
                    Destroy(gameObject);
                    break;
                case EPlayType.Loop:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void Play()
    {
        source.clip = mySound.soundData;
        source.Play();
        isPlaying = true;
    }
}
