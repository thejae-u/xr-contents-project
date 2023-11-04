using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class ItemColliderController : MonoBehaviour
{
    public GameObject parentObj;
    public ItemScript parent;

    public bool IsTimerEnd { get; private set; }

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.CompareTag("Player"))
        {
            PlayerManager.Instance.PlayerDiscountHp(parent.GetComponent<ItemScript>().value,
                parentObj.transform.position.x);
        }

        if (other.transform.CompareTag("NormalEnemy"))
        {
            other.gameObject.GetComponent<NEnemyController>().DiscountHp(parent.GetComponent<ItemScript>().value);
        }

        if (other.transform.CompareTag("EliteEnemy"))
        {
            other.gameObject.GetComponent<EEnemyController>().DiscountHp(parent.GetComponent<ItemScript>().value);
        }
    }

    private IEnumerator Timer()
    {
        IsTimerEnd = false;
        yield return new WaitForSeconds(0.5f);
        IsTimerEnd = true;
    }
}
