using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras;
    private List<bool> visited;

    public bool IsCameraStop { get; set; }
    public bool IsNowCutScene { get; private set; }
    
    private Transform target;
    private float trackSpeed = 10;

    private static CameraController inst;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        visited = new List<bool>();
        for (int i = 0; i < cameras.Count; i++)
            visited.Add(false);
    }

    public static CameraController Inst
    {
        get
        {
             return inst == null ? null : inst;
        }
    }


    private void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void LateUpdate()
    {
        if (IsCameraStop) return;
        
        if (target)
        {
            var v = cameras[0].transform.position;
            v.x = target.position.x + 5.5f;
            cameras[0].transform.position = Vector3.MoveTowards(cameras[0].transform.position, v, trackSpeed * Time.deltaTime);
        }
    }
}