using System;
using System.Collections.Generic;
using UnityEngine;

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

    private long sid;

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

    public void Play(string soundName, GameObject obj)
    {
        var clip = Find(soundName);
        if (clip == null) return;

        var newObj = Instantiate(playPrefab, obj.transform);

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
               Destroy(delObj);
           }
       }
    }
}
