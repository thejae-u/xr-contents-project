using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombController : MonoBehaviour
{
    // Bomb Trace To Player
    [Header("폭탄의 속도를 조절(낮을 수록 빠름)")]
    [SerializeField] private float bombSpeed;

    [Header("폭탄의 각도를 조절")] 
    [SerializeField] private float bombDeg;

    private Vector3[] wayPoints;
    
    // Player Last Position Set
    private Vector3 playerLastPos;
    private Vector3 myPos;
    private float range;

    private void Start()
    {
        playerLastPos = GameObject.Find("Player").transform.position;
        myPos = transform.position;
        range = 1f;
        Shoot();
    }

    private void Shoot()
    {
        //Vector3 topPosition = new Vector3(
          //  Mathf.Pow(playerLastPos.x, 2) / (2*(playerLastPos.x - playerLastPos.y)) + myPos.x,
        //Mathf.Pow(playerLastPos.x, 2) / (4*(playerLastPos.x - playerLastPos.y)) + myPos.y);

        Vector3 topPosition = new Vector3((playerLastPos.x + myPos.x) / 2, myPos.y + bombDeg, 0);

        
        wayPoints = new Vector3[3];
        wayPoints.SetValue(myPos, 0);
        wayPoints.SetValue(topPosition, 1);
        wayPoints.SetValue(playerLastPos, 2);
        
        transform.DOPath(wayPoints, bombSpeed, PathType.CatmullRom, PathMode.Sidescroller2D).OnComplete(() =>
        {
            LogPrintSystem.SystemLogPrint(transform, "BOOM!!", ELogType.EnemyAI);
            Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();

            float d2 = playerTransform.GetComponent<PlayerManager>().MyRadius;

            float distance = (playerTransform.position - transform.position).magnitude;

            if (d2 + range >= distance)
            {
                LogPrintSystem.SystemLogPrint(transform, "Hit Bomb", ELogType.EnemyAI);
            }
            
            Destroy(gameObject);
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
