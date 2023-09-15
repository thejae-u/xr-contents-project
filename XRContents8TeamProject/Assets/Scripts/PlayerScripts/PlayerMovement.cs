using System;
using System.Collections;
using EnemyScripts;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MyRadius => rad;
    public float rad;
    [SerializeField] private NEnemyController nEnemyController;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;

    private bool isJumping;
    private bool canJumpReset;

    
    public Rigidbody2D playerRigidbody;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerMove();
        if (Input.GetButton("Jump"))
        {
            PlayerJump();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            print("Collision Stay in Ground");
            if (canJumpReset)
                isJumping = false;
        }
            
    }

    IEnumerator PlayerJumpResetTime()
    {
        print("Jump Reset Coroutine Start");
        canJumpReset = false;
        yield return new WaitForSeconds(0.5f);
        canJumpReset = true;
        print("Jump Reset Coroutine Start");
    }

    //void PlayerShoot()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        nEnemyController.TestHIT();
    //    }
    //}

    void PlayerMove()
    {
        float moveDir = Input.GetAxis("Horizontal");
        Vector3 dir = moveDir * Vector3.right;

        if (moveDir < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            dir = moveDir * Vector3.right;
            transform.Translate(dir * -1 * moveSpeed * Time.deltaTime);
        }
        else if (moveDir > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            dir = moveDir * Vector3.right;
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
    }

    void PlayerJump()
    {
        if (!isJumping)
        {
            playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(PlayerJumpResetTime());
            isJumping = true;
            Debug.Log("JumpingState : " + isJumping);
        }
        else return;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}