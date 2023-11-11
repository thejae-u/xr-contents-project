using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras;
    private List<bool> visited;
    private CinemachineVirtualCamera curCamera;

    [Range(0.1f, 5.0f)]
    public float shakeTime;

    public bool IsCameraStop { get; set; }
    public bool IsNowCutScene { get; private set; }
    public bool IsNowCameraShaking { get; private set; }

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

        curCamera = cameras[0];
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

    private void LateUpdate()
    {
        if (IsCameraStop) return;
        
        if (target)
        {
            var v = cameras[0].transform.position;
            v.x = target.position.x + 5.5f;
            cameras[0].transform.position = Vector3.MoveTowards(cameras[0].transform.position, v, trackSpeed * Time.deltaTime);
        }
    }

    public void ShakeCamera()
    {
        if (IsNowCameraShaking) return;
        IsNowCameraShaking = true;
        curCamera.transform.DOShakePosition(shakeTime).OnComplete(() =>
        {
            IsNowCameraShaking = false;
        });
    }
}