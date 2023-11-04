using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private List<GameObject> timers;
    [SerializeField] private List<GameObject> item;
    
    private WeakTimeController timer;
    private bool isTimerEnded;
    private bool istimerStarted;

    private bool isInteractable;

    private Vector3 myPos;
    private Vector3 myCameraPos;

    private void Start()
    {
        timer = timers[0].GetComponentInChildren<WeakTimeController>();
        myPos = transform.position;
        myCameraPos = Camera.main.WorldToViewportPoint(myPos);
        isTimerEnded = false;
        istimerStarted = false;
        isInteractable = Probability();
    }

    private void Update()
    {
        if (!istimerStarted)
        {
            myCameraPos = Camera.main.WorldToViewportPoint(myPos);
            if (myCameraPos.x > 0.1f && myCameraPos.x < 0.9f)
            {
                Debug.Log($"{myPos.x}");
                foreach (var timer in timers)
                {
                    timer.SetActive(true);
                }

                timer.Init(waitTime);
                istimerStarted = true;
            }
        }
        else
        {
            if (!timer.IsEnded) return;

            if (timer.IsAttacked)
            {
                Instantiate(isInteractable ? item[0].gameObject : item[1].gameObject, transform.position,
                    Quaternion.identity);
                transform.GetComponent<InteractableObjectController>().enabled = false;
            }

            timer.Checked();
        }
    }

    private bool Probability()
    {
        int range = Random.Range(0, 2);

        return range == 0;
    }
}