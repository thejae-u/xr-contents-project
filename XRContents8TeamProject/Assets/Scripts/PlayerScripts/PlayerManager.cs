using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
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
    private bool canDodge = true;
    [Header("플레이어 회피 사용 시 무적 시간(지속 시간) 조정")]
    [SerializeField] private float dodgeInvincibilityDuration = 1.5f;

    private bool isInvincibility = false;
    private bool isDodgeDirRight;

    // Player shooting related
    [Header("플레이어 공격력 조정")]
    [SerializeField] public float playerAtk = 10.0f;
    [Header("플레이어 한발당 사격 딜레이 조정")]
    [SerializeField] public float shotDelaySpeed = 1.0f;
    [Header("플레이어 총알 속도 조정")]
    [SerializeField] public float bulletSpeed = 30.0f;
    [Header("플레이어 유효 사격 거리")]
    [SerializeField] public float fireDistance = 15.0f;
    [Header("플레이어 최대 탄알")]
    [SerializeField] public int maxAmmo = 6;
    [Header("플레이어 한발당 재장전 시간")]
    [SerializeField] public float reloadTime = 0.7f;

    private Rigidbody2D playerRigidbody;
    // private CameraController cameraController;
    private GameObject playerHpUI;
    private Animator animator;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHpUI = GameObject.Find("HP");
    }

    private void Start()
    {
        playerRigidbody.gravityScale = playerGravityForce;
        // cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
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

        //if (cameraController.IsCameraStop) 
        //{
        //    PlayerMoveLimit();
        //}
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
            isJumping = false;
            animator.SetBool("isJump", false);
        }
    }

    #region MOVEMENT
    void PlayerMove()
    {
        float moveDir = Input.GetAxis("Horizontal");

        if (isPlayerViewDirRight && moveDir != 0)
        {
            Vector3 dir = moveDir * Vector3.right;
            transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            animator.SetBool("isMove", true);
        }
        else if (!isPlayerViewDirRight && moveDir != 0)
        {
            Vector3 dir = moveDir * Vector3.left;
            transform.Translate(dir * playerMoveSpeed * Time.deltaTime);
            animator.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
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
            
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
        }
    }

    IEnumerator PlayerJumpResetTime()
    {
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
    }
    #endregion JUMP

    void PlayerDodge(bool dodgeDirRight)
    { 
        if (canMove)
        {
            Sequence sequence = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "회피 사용", ELogType.Player);

            canDodge = false;
            Vector3 playerPos = transform.position;

            if (dodgeDirRight)
            {
                transform.DOMoveX(playerPos.x + dodgeDistance, 1.0f);
            }
            else
            {
                transform.DOMoveX(playerPos.x - dodgeDistance, 1.0f);
            }

            PlayerInvincibility(dodgeInvincibilityDuration);

            sequence.SetDelay(dodgeCoolTime).OnComplete(() =>
            {
                LogPrintSystem.SystemLogPrint(transform, "회피 쿨타임 종료", ELogType.Player);
                canDodge = true;
            });

            return;
        }
    }

    public void PlayerDiscountHp(float damage, float enemyXPos)
    {
        LogPrintSystem.SystemLogPrint(transform, "Call PlayerDiscountHp", ELogType.Player);
        if (!isInvincibility)
        {
            Sequence sequence = DOTween.Sequence();

            // Turns red when hit
            transform.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f);
            LogPrintSystem.SystemLogPrint(transform, $"Player change Red", ELogType.Player);
            sequence.SetDelay(1.0f).OnComplete(() =>
            {
                transform.GetComponent<Renderer>().material.DOColor(Color.white, 0.1f);
            });

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

    void PlayerInvincibility(float Duration)
    {
        Sequence sequence = DOTween.Sequence();
        LogPrintSystem.SystemLogPrint(transform, "플레이어 무적 상태", ELogType.Player);
        isInvincibility = true;
        gameObject.layer = 3; // 3: PlayerInvincibility
        canJump = false;

        sequence.SetDelay(Duration).OnComplete(() =>
        {
            LogPrintSystem.SystemLogPrint(transform, "플레이어 무적 상태 해제", ELogType.Player);
            gameObject.layer = 6;
            canJump = true;
            isInvincibility = false;
            isKnockback = false;
        });

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
                
        if (worldpos.y < 0f) 
                worldpos.y = 0f;
        if (worldpos.y > 1f)
                worldpos.y = 1f;
        
        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }
}