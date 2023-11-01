using UnityEngine;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private GameObject playerHpUI;
    public Image tutorialtImg;

    // 플레이어 추적 범위
    public float MyRadius => rad;
    public float rad = 1.0f;

    // Player movement & ability related
    [Header("플레이어 체력 조정")]
    [SerializeField] private float playerHp = 100.0f;

    [Header("플레이어 이동 속도 조정")]
    [SerializeField] private float playerMoveSpeed = 5.0f;

    [Header("플레이어 점프 중력 * 점프 힘 조정")]
    [SerializeField] private float playerJumpForce = 20.0f;
    [SerializeField] private float playerGravityForce = 5.0f;

    [Header("플레이어 피격 시 무적 시간")]
    [SerializeField] private float playerHitInvincibilityDuration = 1.0f;

    [Header("플레이어 피격 시 넉백 거리")]
    [SerializeField] private float playerKnockbackDistance = 3.0f;

    private bool isPlayerViewDirRight = true;
    private bool isJumping = false;
    private bool canJump = true;
    private bool isKnockback = false;
    private bool canMove = true;

    // Player dodge(evasion) related
    [Header("플레이어 회피 거리")]
    [SerializeField] private float dodgeDistance = 4.0f;

    [Header("플레이어 회피 쿨타임")]
    [SerializeField] private float dodgeCoolTime = 3.0f;

    [Header("플레이어 회피 사용 시 무적 시간(지속 시간) 조정")]
    [SerializeField] private float dodgeInvincibilityDuration = 1.5f;

    private bool canDodge = true;
    public bool isInvincibility = false;
    private bool isDodgeDirRight;

    // Player shooting related
    [Header("플레이어 노말 공격력 조정")]
    [SerializeField] public float playerNormalAtk = 10.0f;

    [Header("플레이어 사격 게이지에 따른 추가 공격력 조정")]
    [SerializeField] public float playerBonusAtk = 5.0f;

    [Header("플레이어 한발당 사격 딜레이 조정")]
    [SerializeField] public float shotDelaySpeed = 1.0f;

    [Header("플레이어 최대 탄알")]
    [SerializeField] public int maxAmmo = 6;

    [Header("플레이어 최대 발사 게이지")]
    [SerializeField] public float maxGauge = 0.5f;

    // animation
    private bool isDodge = false;

    public enum EPlayerState
    {
        Idle,
        Move,
        Jump,
        Dodge,
        Hit,
        Dead
    }

    public EPlayerState state { get; private set; }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerHpUI = GameObject.Find("HP");
    }

    private void Start()
    {
        playerRigidbody.gravityScale = playerGravityForce;
        Cursor.visible = false;

        state = EPlayerState.Idle;
    }

    private void Update()
    {
        if (CameraController.Inst.IsNowCutScene) return;
        
        PlayerViewMousePoint();
        PlayerMove();

        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.A) && canDodge)
        {
            isDodgeDirRight = false;
            PlayerDodge(isDodgeDirRight);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) && canDodge)
        {
            isDodgeDirRight = true;
            PlayerDodge(isDodgeDirRight);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }

        PlayerMoveLimit();
    }

    public float GetPlayerHp()
    {
        return playerHp;
    }

    public bool GetIsJumping()
    {
        return isJumping;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canJump)
        {
            state = EPlayerState.Idle;

            isJumping = false;
        }
    }

    #region MOVEMENT
    void PlayerMove()
    {
        if (!isDodge)
        {
            state = EPlayerState.Move;

            float moveDir = Input.GetAxis("Horizontal");

            if (isPlayerViewDirRight && moveDir != 0)
            {
                Vector3 dir = moveDir * Vector3.right;
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }
            else if (!isPlayerViewDirRight && moveDir != 0)
            {
                Vector3 dir = moveDir * Vector3.left;
                transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            }

            state = EPlayerState.Idle;
        }
    }
    #endregion
    #region JUMP
    void PlayerJump()
    {
        if (!isJumping)
        {
            state = EPlayerState.Jump;

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
    #region HIT
    public void PlayerDiscountHp(float damage, float enemyXPos)
    {
        if (!isInvincibility)
        {
            playerHp -= damage;

            playerHpUI.GetComponent<hpUIController>().Sethp(damage);

            LogPrintSystem.SystemLogPrint(transform, $"{damage}From Enemy -> Remain PlayerHP{playerHp}", ELogType.Player);

            PlayerKnockback(enemyXPos);
        }
    }

    private void PlayerKnockback(float enemyXPos)
    {
        if (!isKnockback)
        {
            if (canMove)
            {
                state = EPlayerState.Hit;

                Sequence sequence = DOTween.Sequence();

                isKnockback = true;

                // Knockback avatar
                float playerXPos = transform.position.x;

                if (playerXPos > enemyXPos) // 플레이어가 오른쪽에 있다면
                {
                    transform.DOMoveX(transform.position.x + playerKnockbackDistance, playerHitInvincibilityDuration);
                    LogPrintSystem.SystemLogPrint(transform, $"넉백 시 플레이어와 적위치 : {playerXPos},{enemyXPos}", ELogType.Player);
                }
                else if (playerXPos < enemyXPos)
                {
                    transform.DOMoveX(transform.position.x - playerKnockbackDistance, playerHitInvincibilityDuration);
                    LogPrintSystem.SystemLogPrint(transform, $"넉백 시 플레이어와 적위치 : {playerXPos},{enemyXPos}", ELogType.Player);
                }
            }

            PlayerInvincibility(playerHitInvincibilityDuration);
        }
    }

    // 무적상태 호출
    void PlayerInvincibility(float Duration)
    {
        isInvincibility = true;        
        gameObject.layer = 3; // 3: PlayerInvincibility
        canJump = false;
        LogPrintSystem.SystemLogPrint(transform, "플레이어 무적 상태", ELogType.Player);

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(Duration).OnComplete(() =>
        {
            gameObject.layer = 6;
            canJump = true;
            isInvincibility = false;
            isKnockback = false;
            LogPrintSystem.SystemLogPrint(transform, "플레이어 무적 상태 해제", ELogType.Player);
        });
    }
    #endregion

    void PlayerDodge(bool dodgeDirRight)
    {
        if (canMove)
        {
            isDodge = true;
            state = EPlayerState.Dodge;

            Sequence sequence = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "회피 사용", ELogType.Player);

            canDodge = false;
            Vector3 playerPos = transform.position;

            PlayerInvincibility(dodgeInvincibilityDuration);

            if (dodgeDirRight)
            {
                transform.DOMoveX(playerPos.x + dodgeDistance, 1.0f);
            }
            else
            {
                transform.DOMoveX(playerPos.x - dodgeDistance, 1.0f);
            }

            sequence.SetDelay(dodgeCoolTime).OnComplete(() =>
            {
                LogPrintSystem.SystemLogPrint(transform, "회피 쿨타임 종료", ELogType.Player);
                canDodge = true;
            });

            return;
        }
    }

    private void PlayerDeath()
    {
        if (playerHp <= 0)
        {
            state = EPlayerState.Dead;
            // 게임 오버 씬으로 변경
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }

    void PlayerMoveLimit()
    {
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
        
        if (worldpos.x < 0f)
        {
            canMove = false;
            worldpos.x = 0f;
        }

        if(worldpos.x < 1f && worldpos.x > 0f)
        {
            canMove = true;
        }

        if (worldpos.x > 1f)
        {
            canMove = false;
            worldpos.x = 1f;
        }
        
        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tutorial"))
        {
            tutorialtImg.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tutorial"))
        {
            tutorialtImg.gameObject.SetActive(false);
        }
    }
}