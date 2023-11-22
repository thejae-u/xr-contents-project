using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BackgroundScroller : MonoBehaviour
{
    public float x;
    public float y;

    public List<Material> materials;
    private Material mat;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Afternoon : first index of list
        // Night : second index of list
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = materials[0];
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "TestScene3")
            spriteRenderer.material = materials[1];
        
        mat = spriteRenderer.material;
        Vector2 pos = Camera.main.transform.position;
        pos.x = pos.x * x / 50;
        pos.y = pos.y * y / 50;
        mat.SetVector("_att", pos);
    }
}
