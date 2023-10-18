using DG.Tweening;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;

    private PlayerManager playerManager;
    private BalletUIController bulletUI;

    Sequence sequence;
    Sequence sequenceBoltAction;
    Sequence sequenceBackforward;
        
    private int curAmmo = 0;
    private float curGauge = 0;
    private float maxGaugeTime = 0;
    private float lastFireTime = 0;

    private bool isReloading = false;
    private bool isFinishReloadCheck = false; // 현재 장전 사이클이 끝났는지 확인
    private bool notMaxAmmoFill = false;
    private bool isDiscountBullet;
    private bool isMaxGauge = false; // 게이지 최대치 오버

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 1.5f;
    [SerializeField] private float reloadingDelay = 1.5f;
    [SerializeField] private float forwardDelay = 1.5f;

    public enum EState
    {
        Idle,
        Backforward,
        Reloading,
        Forward,
        Shot,
        BoltAction,
        ReloadCheck
    }

    public EState state { get; private set; }

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        bulletUI = GameObject.Find("Bullet").GetComponent<BalletUIController>();
    }

    private void Start()
    {
        curAmmo = playerManager.maxAmmo;
        state = EState.Idle;
    }

    private void Update()
    { 
        if (Input.GetMouseButton(0))
        {
            curGauge = Time.deltaTime;
               
            // 현재 게이지가 최대 게이지를 넘었다면
            if(curGauge >= maxGaugeTime)
            {
                isMaxGauge = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (state == EState.Idle && curAmmo > 0)
            {
                StateShot();
            }
            else if (state == EState.Backforward && curAmmo > 0)
            { 
                // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
                sequenceBackforward.Kill();
                isReloading = false;

                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);
                 
                StateForward();
                state = EState.Idle;
                StateShot();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !isReloading)
        {
            StateBackforward();
        }

        if(notMaxAmmoFill && isFinishReloadCheck)
        {
            StateBackforward();
        }
    }

    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.shotDelaySpeed)
        {
            state = EState.Shot;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -Camera.main.transform.position.z;

            GameObject bullet = Instantiate(bulletPrefab, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

            lastFireTime = Time.time;
            curAmmo--;

            /* UI */
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
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 출력", ELogType.Player);
        
        float shotDelayTime = playerManager.shotDelaySpeed;
        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EState.Idle;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 종료", ELogType.Player);
            sequenceBoltAction = null;
        });
    }

    void StateBackforward()
    {
        if (curAmmo < playerManager.maxAmmo)
        {
            notMaxAmmoFill = false;
            isFinishReloadCheck = false;

            sequenceBackforward = DOTween.Sequence();

            state = EState.Backforward;
            isReloading = true;

            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            sequenceBackforward.SetDelay(reverseDelay).OnComplete(() =>
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

            LogPrintSystem.SystemLogPrint(transform, "탄알 삽입", ELogType.Player);

            sequence.SetDelay(reloadingDelay).OnComplete(() =>
            {
                StateForward();
            });
        }
    }

    void StateForward()
    {
        if (isReloading)
        {
            state = EState.Forward;
            sequence = DOTween.Sequence();

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
                    notMaxAmmoFill = true;
                }

                else if (curAmmo >= playerManager.maxAmmo)
                {
                    state = EState.Idle;
                    LogPrintSystem.SystemLogPrint(transform, "Reload Requset Complete", ELogType.Player);
                }

                isFinishReloadCheck = true;
            });
        }    
    }
}