using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras;

    public bool IsCameraStop { get; set; }
    public bool IsNowCutScene { get; set; }
    
    private Transform target;
    private float trackSpeed = 10;

    private Vector3 prevPosition;

    private static CameraController inst;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);       
    }

    public static CameraController Inst
    {
        get
        {
             return inst == null ? null : inst;
        }
    }

    public void CameraTransition()
    {
        GameObject curCamera = cameras[0].gameObject;
        IsNowCutScene = true;
        curCamera.SetActive(false);
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
            prevPosition = v;
            v.x = target.position.x + 5.5f;
            cameras[0].transform.position = Vector3.MoveTowards(cameras[0].transform.position, v, trackSpeed * Time.deltaTime);
        }
    }
}