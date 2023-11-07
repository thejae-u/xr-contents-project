using UnityEngine;
using System.Collections;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance = null;

    private Rigidbody2D playerRigidbody;
    private GameObject playerHpUI;
    private GameObject playerAim;
    public Image tutorialtImg;

    // 플레이어 추적 범위
    public float MyRadius => rad;
    public float rad = 1.0f;

    // Player movement & ability related
    [Header("플레이어 체력 조정")]
    [SerializeField] private float playerMaxHp = 100.0f; // 최대 체력
    [SerializeField] private float playerHp = 100.0f;    // 현재 체력

    [Header("플레이어 이동 속도 조정")]
    [SerializeField] private float playerMoveSpeed = 5.0f;

    [Header("플레이어 점프 중력 * 점프 힘 조정")]
    [SerializeField] private float playerJumpForce = 20.0f;
    [SerializeField] private float playerGravityForce = 5.0f;

    [Header("플레이어 피격 시 무적 시간")]
    [SerializeField] private float playerHitInvincibilityDuration = 1.0f;

    [Header("플레이어 피격 시 넉백 거리")]
    [SerializeField] private float playerKnockbackDistance = 3.0f;

    private bool isJumping = false;
    private bool canJump = true;
    private bool isKnockback = false;
    private bool canMove = true;
    private bool isPlayerDead = false;

    // Player dodge(evasion) related
    [Header("플레이어 회피 거리")]
    [SerializeField] private float dodgeDistance = 4.0f;

    [Header("플레이어 회피 쿨타임")]
    [SerializeField] private float dodgeCoolTime = 3.0f;

    [Header("플레이어 회피 사용 시 무적 시간(지속 시간) 조정")]
    [SerializeField] private float dodgeInvincibilityDuration = 1.5f;

    private bool canDodge = true;
    private bool isDodgeDirRight;
    public bool isInvincibility = false;

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

    [Header("플레이어 애니메이션")]
    public SkeletonAnimation skeletonAnimation;
    [SpineEvent] public string eventName;
    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset Move;
    public AnimationReferenceAsset Jump;
    public AnimationReferenceAsset Hit;
    public AnimationReferenceAsset Dodge;
    public AnimationReferenceAsset Dead;
    public AnimationReferenceAsset Aim;

    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;
    public Camera cam;
    public Bone bone;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerHpUI = GameObject.Find("HP");
        playerAim = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        playerHp = playerMaxHp;
        playerRigidbody.gravityScale = playerGravityForce;
        Cursor.visible = false;

        CurrentAnimation(0, Idle, true);
        CurrentAnimation(1, Aim, true);
    }

    private void Update()
    {
        if(!isPlayerDead)
        {
            if (CameraController.Inst.IsNowCutScene) return;

            PlayerViewMousePoint();

            float moveDir = Input.GetAxis("Horizontal");
            if (moveDir == 0)
            {
                if (!isJumping)
                {
                    CurrentAnimation(0, Idle, true);
                }
            }
            else
            {
                PlayerMove(moveDir);
            }

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
    }

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public float GetPlayerHp()
    {
        return playerHp;
    }
    public bool GetIsJumping()
    {
        return isJumping;
    }
    public bool GetIsPlayerDead()
    {
        return isPlayerDead;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canJump)
        {
            isJumping = false;
        }
    }

    #region MOVEMENT
    void PlayerMove(float moveDir)
    {
        CurrentAnimation(0, Move, true);
        LogPrintSystem.SystemLogPrint(transform, "Move On", ELogType.Player);
       
        Vector2 playerMove = new Vector2(moveDir * playerMoveSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = playerMove;
    }
    #endregion
    #region JUMP
    void PlayerJump()
    {
        if (!isJumping)
        {
            CurrentAnimation(0, Jump, false);

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
            if (playerHp > 0)
            {
                PlayerKnockback(enemyXPos);
            }
            else
            {
                PlayerDeath();
            }
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

    public void PlayerRecovery(float amount)
    {
        if (playerMaxHp >= playerHp + amount)
        {
            playerHp += amount;
        }
        else
        {
            playerHp = playerMaxHp;
        }
    }

    private void PlayerDeath()
    {
        isPlayerDead = true;
        skeletonAnimation.ClearState();
        CurrentAnimation(0, Dead, false);
    }

    void PlayerViewMousePoint()
    {
        var mousePosition = Input.mousePosition;
        var worldMousePosition = cam.ScreenToWorldPoint(mousePosition);
        playerAim.transform.position = worldMousePosition;

        if (worldMousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
    }

    void PlayerMoveLimit()
    {
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);

        if (worldpos.x < 0f)
        {
            canMove = false;
            worldpos.x = 0f;
        }

        if (worldpos.x < 1f && worldpos.x > 0f)
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rad);
    }

    // 튜토리얼 표지판
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

    // SetAnimation : 애니메이션을 실행 -> 기존에 재생되는 것을 강제로 끊음
    private void CurrentAnimation(int trackindex, AnimationReferenceAsset AnimClip, bool loop)
    {
        if (skeletonAnimation.AnimationName == AnimClip.name) return;

        skeletonAnimation.AnimationState.SetAnimation(trackindex, AnimClip, loop);

        LogPrintSystem.SystemLogPrint(transform, $"animation => {AnimClip}", ELogType.Player);
    }
    // AddAnimation: 현재 실행되고 있는 애니메이션이 종료되고 실행되는 애니메이션 delay는 끝나고 얼마만에 실행되는 지
    private void NextAnimation(int trackindex, AnimationReferenceAsset AnimClip, bool loop, float delay)
    {
        if (skeletonAnimation.AnimationName == AnimClip.name) return;
        skeletonAnimation.AnimationState.AddAnimation(trackindex, AnimClip, loop, delay);

        LogPrintSystem.SystemLogPrint(transform, $"next animation => {AnimClip}", ELogType.Player);
    }
}