using System;
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
    Once,
    Loop
}


[Serializable]
public class Sound
{
    public string soundName;
    public AudioClip soundData;
    public EPlayType playType;
}
    
public class SoundManager : MonoBehaviour
{
    // Sound
    public GameObject playPrefab;
    [SerializeField] private List<Sound> sounds;
    private List<GameObject> createdObjs;

    #region Singleton
    
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
    
    #endregion

    private void Start()
    {
        createdObjs = new List<GameObject>();
    }

    private Sound Find(string soundName)
    {
        foreach (var clip in sounds)
        {
            if (soundName == clip.soundName)
                return clip;
        }

        return null;
    }

    public void SoundPlay(string soundName, GameObject obj)
    {
        var clip = Find(soundName);
        if (clip == null) return;

        var newObj = Instantiate(playPrefab, obj.transform);
        newObj.GetComponent<PlayScript>().mySound = clip;
        newObj.GetComponent<PlayScript>().Play();
        
        createdObjs.Add(newObj);
    }

    public void DeleteSound(GameObject obj)
    {
        var reqObj = obj.transform.GetComponent<PlayScript>().mySound;

        for (int i = createdObjs.Count - 1; i >= 0; i++)
        {
            var myObj = createdObjs[i].transform.GetComponent<PlayScript>().mySound;
            if (myObj.soundName == reqObj.soundName)
                createdObjs.Remove(createdObjs[i]);
        }
    }
}
