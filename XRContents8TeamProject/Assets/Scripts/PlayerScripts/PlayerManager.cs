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
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            LogPrintSystem.SystemLogPrint(transform, "회피 사용", ELogType.Player);
            Sequence sequence = DOTween.Sequence();

            Vector3 DodgeDistance;
            Vector3 playerPos = transform.position;

            sequence.AppendCallback(() =>
            {
                playerStateInvincibility = true;
            });

            if (isPlayerViewDirRight)
                DodgeDistance = new Vector3(playerPos.x + dodgeDistance,playerPos.y,playerPos.z);
            else
                DodgeDistance = new Vector3(playerPos.x - dodgeDistance,playerPos.y,playerPos.z);
            
            LogPrintSystem.SystemLogPrint(transform, $"회피 사용 전 거리{transform.position.x}", ELogType.Player);
            sequence.Append(transform.DOMoveX(DodgeDistance.x, playerInvincibilityDuration));
            LogPrintSystem.SystemLogPrint(transform, $"회피 이동 완료{transform.position.x}", ELogType.Player);


            sequence.AppendCallback(() =>
            {
                playerStateInvincibility = false;
            });

            sequence.Play();
        }
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

    public void DiscountHp(float damage)
    {
        Sequence sequence = DOTween.Sequence();

        // 플레이어 피격시 빨간색으로 변함
        transform.GetComponent<Renderer>().material.DOColor(Color.red, 0.5f);
        LogPrintSystem.SystemLogPrint(transform, $"Player change Red", ELogType.Player);
        sequence.SetDelay(1.0f).OnComplete(() => {
            transform.GetComponent<Renderer>().material.DOColor(Color.white, 0.5f);
        });

        playerHp -= damage;
        LogPrintSystem.SystemLogPrint(transform, $"{damage}From Enemy -> Remain PlayerHP{playerHp}", ELogType.Player);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}