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

    private void Start()
    {
        playerLastPos = GameObject.Find("Player").transform.position;
        myPos = transform.position;
        Shoot();
    }

    private void Update()
    {
        if (playerLastPos != transform.position) return;
        LogPrintSystem.SystemLogPrint(transform, "BOOM!!", ELogType.EnemyAI);
        Destroy(gameObject);
    }

    private void Shoot()
    {
        Sequence sequence = DOTween.Sequence();
        
        //Vector3 topPosition = new Vector3(
          //  Mathf.Pow(playerLastPos.x, 2) / (2*(playerLastPos.x - playerLastPos.y)) + myPos.x,
        //Mathf.Pow(playerLastPos.x, 2) / (4*(playerLastPos.x - playerLastPos.y)) + myPos.y);

        Vector3 topPosition = new Vector3((playerLastPos.x + myPos.x) / 2, myPos.y + bombDeg, 0);

        
        wayPoints = new Vector3[3];
        wayPoints.SetValue(myPos, 0);
        wayPoints.SetValue(topPosition, 1);
        wayPoints.SetValue(playerLastPos, 2);
        transform.DOPath(wayPoints, bombSpeed, PathType.CatmullRom, PathMode.Sidescroller2D);
    }
}
