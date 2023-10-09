using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    
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
    }
}
