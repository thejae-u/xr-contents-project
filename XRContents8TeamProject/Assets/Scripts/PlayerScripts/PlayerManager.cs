using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    // 플레이어 추적 범위
    public float MyRadius => rad;
    public float rad = 1.0f;

    [Header("플레이어 이동 속도 조정")]
    [SerializeField] private float playerMoveSpeed = 5.0f;
    [Header("플레이어 점프 중력 * 점프 힘 조정")]
    [SerializeField] private float playerJumpForce = 20.0f;
    [SerializeField] private float playerGravityForce = 5.0f;

    private bool isPlayerViewDirRight = true;
    private bool isJumping = false;
    private bool canJump = true;

    [Header("플레이어 체력 조정")]
    [SerializeField] private float playerHp = 100.0f;

    [Header("플레이어 공격력 조정")]
    [SerializeField] public float playerAtk = 10.0f;

    [Header("플레이어 회피 거리")]
    [SerializeField] private float dodgeDistance = 4.0f;

    [Header("플레이어 무적 시간(지속 시간) 조정")]
    [SerializeField] private float playerInvincibilityDuration = 1.0f;
    private bool playerStateInvincibility = false;

    [Header("플레이어 한발당 사격 딜레이 조정")]
    [SerializeField] public float shootDelaySpeed = 1.0f;

    [Header("플레이어 총알 속도 조정")]
    [SerializeField] public float bulletSpeed = 30.0f;

    [Header("플레이어 유효 사격 거리")]
    [SerializeField] public float fireDistance = 15.0f;

    [Header("플레이어 최대 탄알")]
    [SerializeField] public int maxAmmo = 6;

    [Header("플레이어 한발당 재장전 시간")]
    [SerializeField] public float reloadTime = 0.7f;

    private Rigidbody2D playerRigidbody;
    Collider2D enemyCollider;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerRigidbody.gravityScale = playerGravityForce;
    }

    private void Update()
    {
        PlayerViewMousePoint();
        PlayerMove();
        PlayerDodge();
        if (Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerStateInvincibility && collision.gameObject.tag != "Ground")
        {
            enemyCollider = collision.gameObject.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), enemyCollider,true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canJump)
            isJumping = false;
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
        if (!isJumping)
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

    void PlayerDodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            LogPrintSystem.SystemLogPrint(transform, "회피 사용", ELogType.Player);
          
            playerStateInvincibility = true;

            Vector3 DodgeDistance = Vector3.zero;
            Vector3 playerPos = transform.position;

            if (isPlayerViewDirRight)
                DodgeDistance = new Vector3(playerPos.x + dodgeDistance, playerPos.y, playerPos.z);
            else
                DodgeDistance = new Vector3(playerPos.x - dodgeDistance, playerPos.y, playerPos.z);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveX(DodgeDistance.x, playerInvincibilityDuration))
                .OnKill(() =>
                {
                    Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), enemyCollider, false);
                    LogPrintSystem.SystemLogPrint(transform, $"회피 이동 완료 {transform.position.x}", ELogType.Player);
                });

            sequence.Play();
        }
    }



    public void DiscountHp(float damage)
    {
        LogPrintSystem.SystemLogPrint(transform, "Call Function : DiscountHP", ELogType.Player);
        if (!playerStateInvincibility)
        {
            Sequence sequence = DOTween.Sequence();

            // 플레이어 피격시 빨간색으로 변함
            transform.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f);
            LogPrintSystem.SystemLogPrint(transform, $"Player change Red", ELogType.Player);
            sequence.SetDelay(1.0f).OnComplete(() =>
            {
                transform.GetComponent<Renderer>().material.DOColor(Color.white, 0.1f);
            });

            playerHp -= damage;
            LogPrintSystem.SystemLogPrint(transform, $"{damage}From Enemy -> Remain PlayerHP{playerHp}", ELogType.Player);
        }
        else
        {
            StartCoroutine(BlockInvincibilityDuration(playerInvincibilityDuration));
            LogPrintSystem.SystemLogPrint(transform, "player State : Invincibility",ELogType.Player);
        }
    }

    private IEnumerator BlockInvincibilityDuration(float InvincibilityDuration)
    {
        yield return new WaitForSeconds(InvincibilityDuration);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), enemyCollider, false);
        playerStateInvincibility = false;
    }

    void PlayerViewMousePoint()
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}