using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public enum EPlayType
{
    Once,
    Loop
}

public enum ESoundType
{
    Bgm,
    Sfx
}


[Serializable]
public class Sound
{
    [HideInInspector] public float soundVolume;
    public string soundName;
    public AudioClip soundData;
    public EPlayType playType;
    public ESoundType soundType;
}
    
public class SoundManager : MonoBehaviour
{
    // Sound
    public GameObject bgmPlayPrefab;
    public GameObject sfxPlayPrefab;

    public AudioMixer mixer;
    
    [SerializeField] private List<Sound> sounds;
    private List<GameObject> createdObjs;

    private long sid;

    #region Singleton
    
    private static SoundManager inst = null;

    public static SoundManager Inst => inst;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);   
        DontDestroyOnLoad(gameObject);
    }
    
    #endregion

    private void Start()
    {
        createdObjs = new List<GameObject>();
        sid = 0;
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

    public void Play(string soundName)
    {
        var clip = Find(soundName);
        if (clip == null) return;
        GameObject newObj;

        switch (clip.soundType)
        {
            case ESoundType.Bgm:
                newObj = Instantiate(bgmPlayPrefab, transform);
                break;
            case ESoundType.Sfx:
                newObj = Instantiate(sfxPlayPrefab, transform);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        newObj.name = $"{soundName} {sid++}";
        newObj.transform.GetComponent<PlayScript>().Data.soundName = clip.soundName;
        newObj.transform.GetComponent<PlayScript>().Data.soundData = clip.soundData;
        newObj.transform.GetComponent<PlayScript>().Data.playType = clip.playType;
        newObj.transform.GetComponent<PlayScript>().Play();

        createdObjs.Add(newObj);
    }

    public void DeleteSound(GameObject obj)
    {
       for (int i = createdObjs.Count - 1; i >= 0; i--)
       {
           if (obj.name == createdObjs[i].name)
           {
               var delObj = createdObjs[i];
               createdObjs.Remove(createdObjs[i]);
               if (obj.activeSelf)
                   Destroy(delObj);
           }
       }
    }

    public void DeleteAllSound()
    {
        foreach (var obj in createdObjs)
        {
            Destroy(obj);
        }    
    }
    
    public void SoundFadeOut()
    {
        float volume;
        mixer.GetFloat("Master", out volume);
        Debug.Log(volume);
        mixer.DOSetFloat("Master", -80.0f, 2.0f).OnComplete(() =>
        {
            mixer.SetFloat("Master", volume);
        });
    }

    public void PlayBombSound()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                Play("Explosion1");
                break;
            case 1:
                Play("Explosion2");
                break;
            case 2:
                Play("Explosion3");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
