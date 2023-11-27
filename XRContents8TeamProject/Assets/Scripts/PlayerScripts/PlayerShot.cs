using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private PlayerManager playerManager;
    private AimUIController aimUIController;
    private BalletUIController bulletUIController;
    private CameraShake cameraShake;

    Sequence sequenceBoltAction;
    Sequence sequenceBackforward;
    Sequence sequenceReloading;
    Sequence sequenceForward;

    private int curAmmo = 0;
    private float lastFireTime = 0;
    public float curDamage = 0;

    private bool isReloading = false;
    private bool isDiscountBullet;

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 0.3f;
    [SerializeField] private float reloadingDelay = 0.3f;
    [SerializeField] private float forwardDelay = 0.3f;

    [Header("사격 시 카메라 강도")]
    [SerializeField] private float hit = 0.5f;
    [SerializeField] private float hitStrong = 2.0f;

    [Header("플레이어 사격 애니메이션")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset Backforward;
    public AnimationReferenceAsset Reloading;
    public AnimationReferenceAsset Forward;
    public AnimationReferenceAsset Shot;
    public AnimationReferenceAsset BoltAction;

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
        cameraShake = GameObject.Find("PlayerFollowCamera").GetComponent<CameraShake>();
    }

    private void Start()
    {
        curAmmo = PlayerManager.Instance.maxAmmo;
        state = EShotState.None;
    }

    private void Update()
    {
        if (!PlayerManager.Instance.GetIsPlayerDead())
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
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                aimUIController.InitGauge();

                if (state == EShotState.None && curAmmo > 0)
                {
                    // StateShot -> BoltAction
                    StateShot();
                }
                else if (state == EShotState.Backforward && curAmmo > 0)
                {
                    // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
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
            if(Input.GetMouseButtonDown(1)  && !isReloading)
            {
                // StateBackforward -> Reloading -> StateForward
                StateBackforward();
                LogPrintSystem.SystemLogPrint(transform, "장전 실행", ELogType.Player);
            }
        }
    }

    #region SHOOTING
    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.shotDelaySpeed)
        {
            state = EShotState.Shot;

            CurrentAnimation(Shot, false);

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -Camera.main.transform.position.z;

            if (aimUIController.isPlayerCheckMaxGauge)
            {
                EffectController.Inst.PlayEffect(Camera.main.ScreenToWorldPoint(mousePosition), "HitStrong");
                curDamage = PlayerManager.Instance.playerMaxAtk;

                cameraShake.CameraShakeForTime(0.05f, hitStrong);
            }
            else
            {
                EffectController.Inst.PlayEffect(Camera.main.ScreenToWorldPoint(mousePosition), "Hit");
                curDamage = PlayerManager.Instance.playerNormalAtk;

                cameraShake.CameraShakeForTime(0.05f, hit);
            }

            aimUIController.isPlayerCheckMaxGauge = false;
            Instantiate(bulletPrefab, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity);

            lastFireTime = Time.time;
            curAmmo--;

            // 사운드 재생
            int randNum = 0;
            randNum = Random.Range(0, 3);
            switch (randNum)
            {
                case 0:
                    SoundManager.Inst.Play("GunShot1", transform.gameObject);
                    break;
                case 1:
                    SoundManager.Inst.Play("GunShot2", transform.gameObject);
                    break;
                case 2:
                    SoundManager.Inst.Play("GunShot3", transform.gameObject);
                    break;
                case 3:
                    SoundManager.Inst.Play("GunShot4", transform.gameObject);
                    break;
            }

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
        NextAnimation(BoltAction, false, 0f);
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션", ELogType.Player);

        float shotDelayTime = playerManager.shotDelaySpeed;
        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EShotState.None;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 종료", ELogType.Player);
            sequenceBoltAction = null;

            aimUIController.InitGauge();
        });
    }
    #endregion
    #region RELOADING
    void StateBackforward()
    {
        LogPrintSystem.SystemLogPrint(transform, "StateBackforward 실행", ELogType.Player);
        if (curAmmo < playerManager.maxAmmo)
        {
            state = EShotState.Backforward;
            isReloading = true;

            NextAnimation(Backforward, false, 0);

            sequenceBackforward = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            sequenceBackforward.SetDelay(reverseDelay).OnComplete(() =>
            {
                NextAnimation(Reloading, false, 0);
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
                NextAnimation(Forward, false, 0);
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

    // 플레이어 사격은 stackindex 2번에서 관리

    // SetAnimation : 애니메이션을 실행 -> 기존에 재생되는 것을 강제로 끊음
    private void CurrentAnimation(AnimationReferenceAsset AnimClip, bool loop)
    {
        if (skeletonAnimation.AnimationName == AnimClip.name) return;
        skeletonAnimation.AnimationState.SetAnimation(2, AnimClip, loop);

        LogPrintSystem.SystemLogPrint(transform, $"animation => {AnimClip}", ELogType.Player);
    }

    // AddAnimation: 현재 실행되고 있는 애니메이션이 종료되고 실행되는 애니메이션 delay는 끝나고 얼마만에 실행되는 지
    private void NextAnimation(AnimationReferenceAsset AnimClip, bool loop, float delay)
    {
        if (skeletonAnimation.AnimationName == AnimClip.name) return;
        skeletonAnimation.AnimationState.AddAnimation(2, AnimClip, loop, delay);

        LogPrintSystem.SystemLogPrint(transform, $"next animation => {AnimClip}", ELogType.Player);
    }
}