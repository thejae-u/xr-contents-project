using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public float MyRadius => rad;
    public float rad;
    private float moveSpeed = 5f;

    private void Update()
    {
        PlayerMove();
        PlayerShoot();
    }

    [SerializeField] private NEnemyController nEnemyController;

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            nEnemyController.TestHIT();
        }
    }

    void PlayerMove()
    {
        Vector3 dir = Vector3.right;
        float moveDir = Input.GetAxis("Horizontal");

        transform.position += dir * moveDir * moveSpeed * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}