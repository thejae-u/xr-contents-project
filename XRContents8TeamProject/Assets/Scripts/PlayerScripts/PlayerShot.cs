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
    public bool isPlayerCheckMaxGauge = false;

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 0.3f;
    [SerializeField] private float reloadingDelay = 0.3f;
    [SerializeField] private float forwardDelay = 0.3f;

    public enum EShotState
    {
        None,
        Backforward,
        Reloading,
        Forward,
        Shot,
        BoltAction
    }

    public EShotState state { get; private set; }

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        aimUIController = GameObject.Find("PlayerAim").GetComponent<AimUIController>();
        bulletUIController = GameObject.Find("PlayerAim").GetComponent<BalletUIController>();
    }

    private void Start()
    {
        curAmmo = playerManager.maxAmmo;
        state = EShotState.None;
    }

    private void Update()
    {
        if (curAmmo == 0)
        {
            aimUIController.SetWarningGauge();
        }

        if (Input.GetMouseButton(0))
        {
            if (curAmmo > 0)
            {
                if (sequenceBoltAction == null)
                {             
                    aimUIController.SetGauge();

                    LogPrintSystem.SystemLogPrint(transform, "에임 게이지 증가", ELogType.Player);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            curGauge = 0;
            aimUIController.InitGauge();
            aimUIController.isReadyWarningGauge = false;

            if (state == EShotState.None && curAmmo > 0)
            {
                // StateShot -> BoltAction
                StateShot();
            }
            else if (state == EShotState.Backforward && curAmmo > 0)
            {
                // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
                // equenceBackforward.Kill();
                if (DOTween.IsTweening(10))
                {
                    LogPrintSystem.SystemLogPrint(transform, "KILL TWEENER", ELogType.Player);
                    DOTween.Kill(10);
                }
                isReloading = false;

                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);

                StateForward();
                state = EShotState.None;
                StateShot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            // StateBackforward -> Reloading -> StateForward
            StateBackforward();
            LogPrintSystem.SystemLogPrint(transform, "장전 실행", ELogType.Player);
        }
    }

    #region SHOOTING
    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.shotDelaySpeed)
        {
            state = EShotState.Shot;

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

        state = EShotState.BoltAction;
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션", ELogType.Player);

        float shotDelayTime = playerManager.shotDelaySpeed;
        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EShotState.None;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 종료", ELogType.Player);
            sequenceBoltAction = null;

            isPlayerCheckMaxGauge = false;
        });
    }
    #endregion
    #region RELOADING
    void StateBackforward()
    {
        if (curAmmo < playerManager.maxAmmo)
        {
            state = EShotState.Backforward;
            sequenceBackforward = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            isReloading = true;

            sequenceBackforward.SetDelay(reverseDelay).OnComplete(() =>
            {
                StateReloading();
            }).SetId(10);
        }
    }

    void StateReloading()
    {
        if (isReloading)
        {
            state = EShotState.Reloading;
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
            state = EShotState.Forward;
            sequenceForward = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 전진", ELogType.Player);

            sequenceForward.SetDelay(forwardDelay).OnComplete(() =>
            {
                curAmmo = playerManager.maxAmmo;

                /* -----UI----- */
                isDiscountBullet = false;
                bulletUIController.SetAmmo(isDiscountBullet);

                isReloading = false;
                state = EShotState.None;
                LogPrintSystem.SystemLogPrint(transform, $"Reload Requset Complete => Current : {curAmmo} -> ReloadTime : {reverseDelay + reloadingDelay + forwardDelay}", ELogType.Player);
            });
        }
    }
    #endregion
}