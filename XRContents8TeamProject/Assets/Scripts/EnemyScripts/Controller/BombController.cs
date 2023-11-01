using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemyScripts;

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

    private EEnemyController parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<EEnemyController>();
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
            var player = GameObject.Find("Player");
            var playerTransform = player.GetComponent<Transform>();
            var playerManager = player.GetComponent<PlayerManager>();

            float d2 = playerManager.MyRadius;
            float distance = (playerTransform.position - transform.position).magnitude;

            if (d2 + range >= distance)
            {
                if (!player.GetComponent<PlayerManager>().isInvincibility)
                {
                    playerManager.PlayerDiscountHp(parent.GetMySpecialDamage(), myPos.x);
                    LogPrintSystem.SystemLogPrint(transform, "Hit Bomb", ELogType.EnemyAI);
                }
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
