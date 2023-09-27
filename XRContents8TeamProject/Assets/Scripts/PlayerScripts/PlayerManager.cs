using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public float MyRadius => rad;
    public float rad = 1.0f;

    [Header("플레이어 이동 관련")]
    [SerializeField] private float playerMoveSpeed = 5.0f;
    [SerializeField] private float playerJumpForce = 20.0f;
    [SerializeField] private float playerGravityForce = 5.0f;

    private bool isPlayerViewDirRight = true;
    private bool isJumping = false;
    private bool canJump = true;

    [Header("플레이어 스테이터스 관련")]
    [SerializeField] private float playerHp = 100.0f;
    [SerializeField] public float playerAtk = 10.0f;
    [SerializeField] private float playerInvincibilityDuration = 1.0f;
    private bool stateInvincibility = false;


    [Header("플레이어 사격 관련")]
    [SerializeField] public float shootSpeed = 1.0f;
    [SerializeField] public float bulletSpeed = 5.0f;
    [SerializeField] public float fireDistance = 50.0f;
    [SerializeField] public int maxAmmo = 6;
    [SerializeField] public float reloadTime = 2.0f;

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
            if (canJump)
                isJumping = false;
        }
    }

    #region MOVEMENT
    void PlayerMove()
    {
        float moveDir = Input.GetAxis("Horizontal");

        if (isPlayerViewDirRight) 
        {
            Vector3 dir = moveDir * Vector3.right;
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }
        }
        else
        {
            Vector3 dir = moveDir * Vector3.left;

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }
        }
    }
    #endregion

    #region JUMP
    void PlayerJump()
    {
        if(!isJumping) 
        {
            playerRigidbody.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);
            StartCoroutine(PlayerJumpResetTime());
            isJumping = true;
        }
    }

    IEnumerator PlayerJumpResetTime()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }
    #endregion JUMP

    private void PlayerViewMousePoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            isPlayerViewDirRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            isPlayerViewDirRight = true;
        }
    }

    public void DiscountHp(float damage)
    {
        playerHp -= damage;
        LogPrintSystem.SystemLogPrint(transform, $"{damage}From Enemy -> Remain PlayerHP{playerHp}", ELogType.Player);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}