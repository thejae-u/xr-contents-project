using System;
using System.Collections;
using System.Collections.Generic;
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

public class SoundManager : MonoBehaviour
{
    // Sounds
    [SerializeField] private List<Sound> sounds;
    
    private void Start()
    {
       
    }
}
