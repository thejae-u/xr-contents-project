using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;

    private PlayerManager playerManager;
    private Animator animator;
    private BalletUIController bulletUI;

    Sequence sequence;
    Sequence sequenceBoltAction;
    Sequence sequenceReleaseTheBolt;

    private float lastFireTime = 0f;
    private int curAmmo = 0;
    private bool isReloading = false;
    private bool isFinishReloadCheck = false; // 현재 장전 사이클이 끝났는지 확인
    private bool notMaxAmmoFill = false;
    private bool isDiscountBullet;

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 1.5f;
    [SerializeField] private float reloadingDelay = 1.5f;
    [SerializeField] private float forwardDelay = 1.5f;

    public enum EState
    {
        Idle,
        ReleaseTheBolt, // 후퇴
        Reloading,
        RackYourBolt, // 전진
        Shot,
        BoltAction,
        ReloadCheck
    }

    public EState state { get; private set; }

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        animator = gameObject.GetComponent<Animator>();
        bulletUI = GameObject.Find("Bullet").GetComponent<BalletUIController>();
    }

    private void Start()
    {
        curAmmo = playerManager.maxAmmo;
        state = EState.Idle;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == EState.Idle && curAmmo > 0)
            {
                StateShot();
            }
            else if (state == EState.ReleaseTheBolt && curAmmo > 0)
            { // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
                sequenceReleaseTheBolt.Kill();
                isReloading = false;

                animator.SetBool("isReloadCancle", true);
                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);
                 
                StateRackYourBolt();
                state = EState.Idle;
                animator.SetBool("isReloadCancle", false);
                StateShot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StateReleaseTheBolt();
        }

        if(notMaxAmmoFill && isFinishReloadCheck)
        {
            StateReleaseTheBolt();
        }
    }

    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.shotDelaySpeed)
        {
            state = EState.Shot;

            // 총 쏘는 애니메이션 출력
            animator.SetBool("isShot", true);
            animator.SetTrigger("doShot");

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -Camera.main.transform.position.z;

            Vector3 direction = (Camera.main.ScreenToWorldPoint(mousePosition) - fireTransform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * playerManager.bulletSpeed;

            lastFireTime = Time.time;
            curAmmo--;

            isDiscountBullet = true;
            bulletUI.SetAmmo(isDiscountBullet);

            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo}", ELogType.Player);

            StateBoltAction();
        }
    }

    void StateBoltAction()
    {
        sequenceBoltAction = DOTween.Sequence();

        state = EState.BoltAction;
        // 볼트액션 애니메이션 출력
        animator.SetBool("isShot", false);
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 출력", ELogType.Player);
        
        float shotDelayTime = playerManager.shotDelaySpeed;
        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EState.Idle;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 종료", ELogType.Player);
            animator.SetBool("isShot", true);
            sequenceBoltAction = null;
        });
    }

    void StateReleaseTheBolt()
    {
        if (curAmmo < playerManager.maxAmmo)
        {
            // 값 초기화
            notMaxAmmoFill = false;
            isFinishReloadCheck = false;
            animator.SetBool("isReloading", false);
            animator.SetBool("isRack", false);


            sequenceReleaseTheBolt = DOTween.Sequence();

            state = EState.ReleaseTheBolt;
            isReloading = true;

            animator.SetTrigger("doReload"); 

            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            sequenceReleaseTheBolt.SetDelay(reverseDelay).OnComplete(() =>
            {
                StateReloading();
            });
        }
    }

    void StateReloading()
    {
        if (isReloading)
        {
            state = EState.Reloading;
            sequence = DOTween.Sequence();

            animator.SetBool("isReloading", true);
            LogPrintSystem.SystemLogPrint(transform, "탄알 삽입", ELogType.Player);

            sequence.SetDelay(reloadingDelay).OnComplete(() =>
            {
                StateRackYourBolt();
            });
        }
    }

    void StateRackYourBolt()
    {
        if (isReloading)
        {
            state = EState.RackYourBolt;
            sequence = DOTween.Sequence();
            animator.SetBool("isRack", true);

            sequence.SetDelay(forwardDelay).OnComplete(() =>
            {
                LogPrintSystem.SystemLogPrint(transform, "노리쇠 전진", ELogType.Player);

                curAmmo++;
                isDiscountBullet = false;
                bulletUI.SetAmmo(isDiscountBullet);

                isReloading = false;
                LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo} -> ReloadTime : {playerManager.reloadTime}", ELogType.Player);         
                
                if (curAmmo < playerManager.maxAmmo)
                {
                    animator.SetBool("notMaxAmmo", true);
                    notMaxAmmoFill = true;
                }

                else if (curAmmo >= playerManager.maxAmmo)
                {
                    animator.SetBool("notMaxAmmo", false);
                    state = EState.Idle;
                    LogPrintSystem.SystemLogPrint(transform, "Reload Requset Complete", ELogType.Player);
                }

                isFinishReloadCheck = true;
            });
        }    
    }
}