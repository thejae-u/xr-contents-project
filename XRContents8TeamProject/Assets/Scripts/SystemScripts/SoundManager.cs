using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum ESoundType
{
    // 사운드의 타입을 나눠서 작성
    System,
    Player,
    Enemy,
}

public enum EPlayType
{
    // 사운드의 플레이 방식을 나눠서 작성
    Once,
    Recall,
    Loop
}


[Serializable]
public class Sound
{
    public string soundName;
    public AudioClip soundData;
    public ESoundType soundType;
    public EPlayType playType;
}

[Serializable]
public class Origin
{
    public ESoundType soundType;
    public AudioSource origin;
    public string name;

    public Origin(ESoundType sType, AudioSource origin, GameObject obj)
    {
        this.origin = origin;
        soundType = sType;
        name = obj.name;
    }
}
    
public class SoundManager : MonoBehaviour
{
    // Sound
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private List<Origin> origins;

    private static SoundManager inst = null;

    public static SoundManager Inst
    {
        get
        {
            if (inst == null)
                inst = new SoundManager();
            return inst;
        }
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        origins = new List<Origin>();
    }

    public void SoundRegister(ESoundType sType, AudioSource source, GameObject obj)
    {
        Origin newOrigin = new Origin(sType, source, obj);
        origins.Add(newOrigin);
    }

    public bool Play(string soundName, GameObject obj)
    {
        var clip = Find(soundName);
        if (clip == null) return false;
        foreach (var origin in origins)
        {
            if (origin.name == obj.name)
            {
                origin.origin.clip = clip.soundData;

                switch (clip.playType)
                {
                    case EPlayType.Recall:
                        // Instantiate object and play once then unable AudioSource
                        return true;
                    case EPlayType.Loop:
                        // Instantiate object and loop sound
                        return true;
                    case EPlayType.Once:
                        // Play Once and Delete object
                        return true;
                    default:
                        // Human Error
                        return false;
                }
            }
        }

        return false;
    }

    private Sound Find(string soundName)
    {
        foreach (var clip in sounds)
        {
            if (clip.soundName == soundName)
                return clip;
        }

        // if not found
        return null;
    }
}
