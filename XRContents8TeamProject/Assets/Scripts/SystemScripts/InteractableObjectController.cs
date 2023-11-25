using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private List<GameObject> timers;
    [SerializeField] private List<GameObject> item;
    
    private WeakTimeController timer;
    private bool isTimerEnded;
    private bool isTimerStarted;

    private bool isInteractable;

    private Vector3 myPos;
    private Vector3 myCameraPos;

    private void Start()
    {
        timer = timers[0].GetComponentInChildren<WeakTimeController>();
        myPos = transform.position;
        myCameraPos = Camera.main.WorldToViewportPoint(myPos);
        isTimerEnded = false;
        isTimerStarted = false;
        isInteractable = Probability();
    }

    private void Update()
    {
        MainLogic();
    }

    private void MainLogic()
    {
        if (!isTimerStarted)
        {
            myCameraPos = Camera.main.WorldToViewportPoint(myPos);
            if (myCameraPos.x > 0.1f && myCameraPos.x < 0.9f)
            {
                foreach (var timer in timers)
                {
                    timer.SetActive(true);
                }

                timer.Init(waitTime);
                isTimerStarted = true;
            }
        }
        else
        {
            if (!timer.IsEnded) return;

            if (timer.IsAttacked)
            {
                var newPos = new Vector3(transform.position.x, transform.position.y, 0);
                switch (SceneManager.GetActiveScene().name)
                {
                    case "Stage1":
                        EffectController.Inst.PlayEffect(newPos, "Reaf1");
                        break;
                    case "Stage2":
                        EffectController.Inst.PlayEffect(newPos, "Reaf2");
                        break;
                    case "Stage3":
                        newPos.y -= 2f;
                        EffectController.Inst.PlayEffect(newPos, "Reaf3");
                        break;
                    default:
                        throw new ArgumentException();
                    
                }
                gameObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", false);
                if (GameManager.Inst.IsNight)
                {
                    Instantiate(isInteractable ? item[2].gameObject : item[3].gameObject, transform.position,
                        Quaternion.identity);
                    transform.GetComponent<InteractableObjectController>().enabled = false;

                }
                else
                {
                    Instantiate(isInteractable ? item[0].gameObject : item[1].gameObject, transform.position,
                        Quaternion.identity);
                    transform.GetComponent<InteractableObjectController>().enabled = false;
                }
            }
            else
            {
                foreach (var timer in timers)
                {
                    timer.SetActive(false);
                }
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