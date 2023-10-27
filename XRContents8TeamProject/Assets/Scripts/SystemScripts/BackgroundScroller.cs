using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    /*
    public float x;
    public float y;

    public Material mat;

    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mat = spriteRenderer.material;
        Vector2 pos = Camera.main.transform.position;
        pos.x = pos.x * x / 50;
        pos.y = pos.y * y / 50;
        mat.SetVector("_att", pos);
    }*/

    [SerializeField] private List<Transform> sprites;

    private Vector2 camPos;
    private Vector2 prevPos;
    
    // True : Right, False : Left
    private bool moveDir;

    private void Start()
    {
        prevPos = Camera.main.transform.position;
    }


    private void Update()
    {
        camPos = Camera.main.transform.position;

        // Camera Moving Check
        if (camPos == prevPos) return;
        
        Vector2 dir = (prevPos - camPos).normalized;
        float dist = (prevPos - camPos).magnitude;
        moveDir = dir.x > 0;

        foreach (var sprite in sprites)
        {
            
        }

        prevPos = camPos;
    }
}
