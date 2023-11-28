using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    public string effectName;
    public GameObject effectObj;
}

public class EffectController : MonoBehaviour
{
    [SerializeField] private List<Effect> effects;

    private static EffectController inst;

    public static EffectController Inst
    {
        get { return inst; }
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    public void PlayEffect(Vector3 pos, string effectName)
    {
        foreach (var effect in effects)
        {
            if (effect.effectName == effectName)
            {
                pos = new Vector3(pos.x, pos.y, 0);
                GameObject newObj;

                newObj = Instantiate(effect.effectObj, pos, Quaternion.identity);

                newObj.transform.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public void PlayEffect(Vector3 pos, string effectName, Transform parent)
    {
        foreach (var effect in effects)
        {
            if (effect.effectName == effectName)
            {
                pos = new Vector3(pos.x, pos.y, 0);
                var newObj = Instantiate(effect.effectObj, parent);
                newObj.transform.position = pos;

                newObj.transform.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
