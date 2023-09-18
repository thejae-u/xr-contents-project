using EnemyScripts;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float MyRadius => rad;
    [Header("적 추적 확인 반지름")]
    public float rad;
    [Header("플레이어 이동속도")]
    [SerializeField] private float moveSpeed = 5.0f;
    [Header("플레이어 점프힘")]
    [SerializeField] private float jumpForce = 5.0f;
    [Header("플레이어 중력")]
    [SerializeField] private float gravityForce = 5.0f;
    [Header("플레이어 체력")]
    [SerializeField] private float playerHp = 100.0f;

    private bool isJumping;
    private bool canJumpReset;
    
    private Rigidbody2D playerRigidbody;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerViewMousePoint();
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
            if (canJumpReset)
                isJumping = false;
        } 
    }

    void PlayerMove()
    {
        float moveDir = Input.GetAxis("Horizontal");
        Vector3 dir = moveDir * Vector3.right;
        //transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (moveDir < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.Translate(dir * -1 * moveSpeed * Time.deltaTime);
        }
        else if (moveDir > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
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
        }
        else return;
    }

    IEnumerator PlayerJumpResetTime()
    {
        print("Jump Reset Coroutine Start");
        canJumpReset = false;
        yield return new WaitForSeconds(0.5f);
        canJumpReset = true;
        print("Jump Reset Coroutine End");
    }

    private void PlayerViewMousePoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
    }

    public void DiscountHp(float damage)
    {
        playerHp -= damage;
        Debug.Log("현재 플레이어의 체력 : " + playerHp + "들어온 데미지 : " + damage);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}