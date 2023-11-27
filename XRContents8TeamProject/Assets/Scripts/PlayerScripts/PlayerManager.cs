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
    private bool isHit = false;
    private bool isDodge = false;
    private bool isPlayerDead = false;
    private bool isFinishGame = false;

    // Player dodge(evasion) related
    [Header("플레이어 회피 거리")]
    [SerializeField] private float dodgeDistance = 8.0f;

    [Header("플레이어 회피 쿨타임")]
    [SerializeField] private float dodgeCoolTime = 3.0f;

    [Header("플레이어 회피 사용 시 무적 시간(지속 시간) 조정")]
    [SerializeField] private float dodgeInvincibilityDuration = 1.0f;

    private bool canDodge = true;
    private bool isPlayerDirRight; // 플레이어가 현재 바라보는 방향
    private bool isDodgeDirRight;  // 키 입력에 따른 방향
    public bool isInvincibility = false;

    // Player shooting related
    [Header("플레이어 노말 공격력 조정")]
    [SerializeField] public float playerNormalAtk = 10.0f;

    [Header("플레이어 사격 게이지에 따른 추가 공격력 조정")]
    [SerializeField] public float playerMaxAtk = 15.0f;

    [Header("플레이어 한발당 사격 딜레이 조정")]
    [SerializeField] public float shotDelaySpeed = 1.0f;

    [Header("플레이어 최대 탄알")]
    [SerializeField] public int maxAmmo = 6;

    [Header("플레이어 최대 발사 게이지")]
    [SerializeField] public float maxGauge = 0.5f;

    [Header("플레이어 애니메이션")]
    public SkeletonAnimation skeletonAnimation;

    /// <summary>
    /// [SpineEvent] public string eventName;
    /// </summary>
    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset Move;
    public AnimationReferenceAsset Jump;
    public AnimationReferenceAsset Hit;
    public AnimationReferenceAsset BackwardDodge;
    public AnimationReferenceAsset Dodge;
    public AnimationReferenceAsset Dead;
    public AnimationReferenceAsset Aim;

    private bool isAnimationBackwards; // 애니메이션 역재생 여부

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
                if (isJumping) return;
                if(isDodge) return;
                if(isHit) return;

                CurrentAnimation(0, Idle, true);
                CurrentAnimation(1, Aim, true);
            }
            else
            {
                if (isJumping) return;
                if (isDodge) return;
                if (isHit) return;

                PlayerMove(moveDir);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.A) && canDodge)
            {
                isDodgeDirRight = false;

                if (isPlayerDirRight)
                    isAnimationBackwards = true;
                else
                    isAnimationBackwards = false;

                PlayerDodge(isDodgeDirRight);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) && canDodge)
            {
                isDodgeDirRight = true;

                if (!isPlayerDirRight)
                    isAnimationBackwards = true;
                else
                    isAnimationBackwards = false;

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
    public bool GetIsFinishGame()
    {
        return isFinishGame;
    }

    #region MOVEMENT
    void PlayerMove(float moveDir)
    {
        CurrentAnimation(0, Move, true);

        Vector2 playerMove = new Vector2(moveDir * playerMoveSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = playerMove;
    }
    #endregion
    #region JUMP
    void PlayerJump()
    {
        if (!isJumping)
        {
            Sequence playerJumpSequence = DOTween.Sequence();

            isJumping=true;
            canJump = false;

            CurrentAnimation(0, Jump, false);

            playerRigidbody.AddForce(Vector2.up * playerJumpForce, ForceMode2D.Impulse);

            playerJumpSequence.SetDelay(0.5f).OnComplete(() =>
            {
                canJump = true;
            });
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canJump)
        {
            isJumping = false;
        }
    }
    #endregion JUMP
    #region HIT
    public void PlayerDiscountHp(float damage, float enemyXPos)
    {
        if (!isInvincibility)
        {
            playerHp -= damage;

            playerHpUI.GetComponent<hpUIController>().SetDiscountHp(damage);

            LogPrintSystem.SystemLogPrint(transform, $"{damage}From Enemy -> Remain PlayerHP{playerHp}", ELogType.Player);
            if (playerHp > 0)
            {
                isHit = true;
                skeletonAnimation.ClearState();
                CurrentAnimation(0, Hit, false);
                LogPrintSystem.SystemLogPrint(transform, "Hit state update", ELogType.Player);
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
                isKnockback = true;

                // Knockback avatar
                float playerXPos = transform.position.x;

                if (playerXPos > enemyXPos) // 플레이어가 오른쪽에 있다면
                {
                    playerRigidbody.velocity = new Vector2(playerKnockbackDistance, playerRigidbody.velocity.y);
                    playerRigidbody.AddForce(Vector2.right * playerMoveSpeed, ForceMode2D.Impulse);
                    LogPrintSystem.SystemLogPrint(transform, $"넉백 시 플레이어와 적위치 : {playerXPos},{enemyXPos}", ELogType.Player);
                }
                else if (playerXPos < enemyXPos)
                {
                    playerRigidbody.velocity = new Vector2(-playerKnockbackDistance, playerRigidbody.velocity.y);
                    playerRigidbody.AddForce(Vector2.left * playerMoveSpeed, ForceMode2D.Impulse);
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
            isHit = false;
            LogPrintSystem.SystemLogPrint(transform, "플레이어 무적 상태 해제", ELogType.Player);
        });
    }
    #endregion

    void PlayerDodge(bool dodgeDirRight)
    {
        if (canMove)
        {
            isDodge = true;
            Sequence sequence = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "회피 사용", ELogType.Player);

            canDodge = false;

            PlayerInvincibility(dodgeInvincibilityDuration);

            // 이동 실행
            if (dodgeDirRight)
            {
                playerRigidbody.velocity = new Vector2(dodgeDistance, playerRigidbody.velocity.y);
                playerRigidbody.AddForce(Vector2.right * playerMoveSpeed, ForceMode2D.Impulse);
            }
            else
            {
                playerRigidbody.velocity = new Vector2(-dodgeDistance, playerRigidbody.velocity.y);
                playerRigidbody.AddForce(Vector2.left * playerMoveSpeed, ForceMode2D.Impulse);
            }
 
            if (isAnimationBackwards)
            {
                skeletonAnimation.ClearState();
                CurrentAnimation(0, BackwardDodge, false);
            }
            else
            {      
                skeletonAnimation.ClearState();
                CurrentAnimation(0, Dodge, false);
            }

            LogPrintSystem.SystemLogPrint(transform, "Dodge Animation Play", ELogType.Player);

            sequence.SetDelay(dodgeCoolTime).OnComplete(() =>
            {
                LogPrintSystem.SystemLogPrint(transform, "회피 쿨타임 종료", ELogType.Player);
                canDodge = true;
                isDodge = false;
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
            playerHp = playerMaxHp; // 회복 후 최대 체력이 오버되면 최대 체력으로 변경
        }
        
        playerHpUI.GetComponent<hpUIController>().SetRecoveryHP(amount);
    }

    private void PlayerDeath()
    {
        isPlayerDead = true;
        skeletonAnimation.ClearState();
        CurrentAnimation(0, Dead, false);

        SoundManager.Inst.Play("PlayerDead", transform.gameObject);

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(2.5f).OnComplete(() =>
        {
            isFinishGame = true;
        });

    }

    void PlayerViewMousePoint()
    {
        var mousePosition = Input.mousePosition;
        var worldMousePosition = cam.ScreenToWorldPoint(mousePosition);
        playerAim.transform.position = worldMousePosition;

        if (worldMousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            isPlayerDirRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            isPlayerDirRight = true;
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
}