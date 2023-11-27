using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScript : MonoBehaviour
{
    [SerializeField]
    private Sound mySound;

    public Sound Data => mySound;
    private AudioSource source;

    private bool isPlaying;

    private void Awake()
    {
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
                    SoundManager.Inst.DeleteSound(gameObject);
                    break;
                case EPlayType.Loop:
                    if (source.isPlaying) return;
                    Play();
                    break;
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
