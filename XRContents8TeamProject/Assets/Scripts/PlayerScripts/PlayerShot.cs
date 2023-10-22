using DG.Tweening;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private PlayerManager playerManager;
    private AimUIController aimUIController;
    private BalletUIController bulletUIController;

    Sequence sequenceBoltAction;
    Sequence sequenceBackforward;
    Sequence sequenceReloading;
    Sequence sequenceForward;
        
    private int curAmmo = 0;
    private float curGauge = 0;
    private float lastFireTime = 0;

    private bool isReloading = false;
    private bool isDiscountBullet;
    public bool isMaxGauge = false; // 게이지 최대치 오버

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 0.3f;
    [SerializeField] private float reloadingDelay = 0.3f;
    [SerializeField] private float forwardDelay = 0.3f;

    public enum EState
    {
        Idle,
        Backforward,
        Reloading,
        Forward,
        Shot,
        BoltAction
    }

    public EState state { get; private set; }

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        aimUIController = GameObject.Find("PlayerAim").GetComponent<AimUIController>();
        bulletUIController = GameObject.Find("Bullet").GetComponent<BalletUIController>();
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
            if(curAmmo > 0)
            {
                curGauge += Time.deltaTime;
                // 게이지가 차기 시작할 때 UI에 생성한 함수를 호출한다.
                aimUIController.SetGauge();
            }
            else if(curAmmo == 0) 
            {
                // 총알이 없는 경우 게이지가 차지 않는다.
                aimUIController.SetWarningGauge();
            }

            if (!isMaxGauge && curGauge >= playerManager.maxGauge)
            {
                isMaxGauge = true;
                LogPrintSystem.SystemLogPrint(transform, $"{curGauge} => MAXGAUGE", ELogType.Player);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            curGauge = 0;
            aimUIController.InitGauge();

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
            LogPrintSystem.SystemLogPrint(transform,"장전 실행",ELogType.Player);
        }
    }

    #region SHOOTING
    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.shotDelaySpeed)
        {
            state = EState.Shot;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -Camera.main.transform.position.z;

            GameObject bullet = Instantiate(bulletPrefab, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity);

            lastFireTime = Time.time;
            curAmmo--;          

            /* UI */
            isDiscountBullet = true;
            bulletUIController.SetAmmo(isDiscountBullet);
            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo}", ELogType.Player);
            StateBoltAction();
        }
    }

    void StateBoltAction()
    {
        sequenceBoltAction = DOTween.Sequence();

        state = EState.BoltAction;
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션", ELogType.Player);
        
        float shotDelayTime = playerManager.shotDelaySpeed;
        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EState.Idle;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 종료", ELogType.Player);
            sequenceBoltAction = null;

            isMaxGauge = false;
        });
    }
    #endregion
    #region RELOADING
    void StateBackforward()
    {
        if (curAmmo < playerManager.maxAmmo)
        {
            state = EState.Backforward;
            sequenceBackforward = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            isReloading = true;

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
            sequenceReloading = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "탄알 삽입", ELogType.Player);

            sequenceReloading.SetDelay(reloadingDelay).OnComplete(() =>
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
            sequenceForward = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 전진", ELogType.Player);

            sequenceForward.SetDelay(forwardDelay).OnComplete(() =>
            {
                curAmmo = playerManager.maxAmmo;

                /* -----UI----- */
                isDiscountBullet = false;
                bulletUIController.SetAmmo(isDiscountBullet);

                isReloading = false;
                state = EState.Idle;
                LogPrintSystem.SystemLogPrint(transform, $"Reload Requset Complete => Current : {curAmmo} -> ReloadTime : {reverseDelay + reloadingDelay + forwardDelay}", ELogType.Player);         
            });
        }    
    }
    #endregion
}