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

    public void FirstCameraTransition()
    {
        if (visited[0]) return;
        cameras[0].gameObject.SetActive(false);
        cameras[1].gameObject.SetActive(true);
        
        IsNowCutScene = true;
        visited[0] = true;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(5.0f).OnComplete(CutSceneEnd);
    }

    private void CutSceneEnd()
    {
        foreach (var camera in cameras)
        {
            if(camera.transform.name == "PlayerFollowCamera")
                camera.gameObject.SetActive(true);
            else
                camera.gameObject.SetActive(false);
        }
        
        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(3.0f).OnComplete(() =>
        {
            IsNowCutScene = false;
        });
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
            v.x = target.position.x + 2.5f;
            cameras[0].transform.position = Vector3.MoveTowards(cameras[0].transform.position, v, trackSpeed * Time.deltaTime);
        }
    }
}