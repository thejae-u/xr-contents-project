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

    public static SoundManager Inst => inst;

    private void Awake()
    {
        inst = this;
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

    public void Play(string soundName, GameObject obj)
    {
        var clip = Find(soundName);
        if (clip == null) return;

        var newObj = Instantiate(playPrefab, obj.transform);

        newObj.transform.GetComponent<PlayScript>().Data.soundName = clip.soundName;
        newObj.transform.GetComponent<PlayScript>().Data.soundData = clip.soundData;
        newObj.transform.GetComponent<PlayScript>().Data.playType = clip.playType;
        
        createdObjs.Add(newObj);
    }

    public void DeleteSound(GameObject obj)
    {
       Sound reqObj = null;
       
       for (int i = 0; i < obj.transform.childCount; i++)
       {
           var child = obj.transform.GetChild(i);
           Debug.Log(child.GetComponent<PlayScript>());
           
           if (child.GetComponent<AudioSource>() != null)
               reqObj = child.GetComponent<PlayScript>().Data;
       }
       
       if (reqObj == null) return;
       
       for (int i = createdObjs.Count - 1; i >= 0; i++)
       {
           var myObj = createdObjs[i].transform.GetComponent<PlayScript>().Data;
           if (myObj.soundName == reqObj.soundName)
           {
               Destroy(createdObjs[i].gameObject);
               createdObjs.Remove(createdObjs[i]);
           }
       }
    }
}
