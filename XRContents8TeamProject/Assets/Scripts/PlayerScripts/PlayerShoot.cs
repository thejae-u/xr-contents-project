using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;

    private GameObject playerManager;
    private GameObject bulletUI;

    Sequence sequence;
    Sequence sequenceBoltAction;
    Sequence sequenceReleaseTheBolt;

    private float lastFireTime = 0f;
    private int curAmmo = 0;
    private bool isReloading;
    private bool isDiscountBullet;

    [Header("총 딜레이 관련")]
    [SerializeField] private float reverseDelay = 0.1f;
    [SerializeField] private float reloadingDelay = 0.5f;
    [SerializeField] private float forwardDelay = 0.1f;

    public enum EState
    {
        Idle,
        ReleaseTheBolt, // 후퇴
        Reloading,
        RackYourBolt, // 전진
        Shot,
        BoltAction,
        ReloadComplete
    }

    public EState state { get; private set; }

    private void Awake()
    {
        playerManager = GameObject.Find("Player");
        bulletUI = GameObject.Find("Bullet");
    }

    private void Start()
    {
        curAmmo = playerManager.GetComponent<PlayerManager>().maxAmmo;
        isReloading = false;
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
                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);
                StateRackYourBolt();
                StateShot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StateReleaseTheBolt();
        }

        // 한발씩 장전해주기 위해 ReloadComplete 상태를 이용하여 장전이 완료된 후 curAmmo의 수치를 확인한다.
        if (state == EState.ReloadComplete)
        {
            if (curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                StateReleaseTheBolt();
            }
            else if (curAmmo >= playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                state = EState.Idle;
                isReloading = false;
                LogPrintSystem.SystemLogPrint(transform, "Reload Requset Complete", ELogType.Player);
            }
        }
    }

    void StateShot()
    {
        if (Time.time >= lastFireTime + playerManager.GetComponent<PlayerManager>().shotDelaySpeed)
        {
            state = EState.Shot;

            // 총 쏘는 애니메이션 출력

            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -Camera.main.transform.position.z;

            Vector3 direction = (Camera.main.ScreenToWorldPoint(mousePosition) - fireTransform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * playerManager.GetComponent<PlayerManager>().bulletSpeed;

            lastFireTime = Time.time;
            curAmmo--;

            isDiscountBullet = true;
            bulletUI.GetComponent<BalletUIController>().SetAmmo(isDiscountBullet);

            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo}", ELogType.Player);

            StateBoltAction();
        }
    }

    void StateBoltAction()
    {
        sequenceBoltAction = DOTween.Sequence();

        state = EState.BoltAction;
        // 볼트액션 애니메이션 출력
        LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 출력", ELogType.Player);
        float shotDelayTime = playerManager.GetComponent<PlayerManager>().shotDelaySpeed;

        sequenceBoltAction.SetDelay(shotDelayTime).OnComplete(() =>
        {
            state = EState.Idle;
            LogPrintSystem.SystemLogPrint(transform, "볼트 액션 애니메이션 종료", ELogType.Player);
            sequenceBoltAction = null;
        });
    }

    void StateReleaseTheBolt()
    {
        if (curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
        {
            sequenceReleaseTheBolt = DOTween.Sequence();

            state = EState.ReleaseTheBolt;
            isReloading = true;
            // 노리쇠 후퇴 애니메이션 출력
            LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴", ELogType.Player);

            sequenceReleaseTheBolt.SetDelay(reverseDelay).OnComplete(() =>
            {
                LogPrintSystem.SystemLogPrint(transform, "노리쇠 후퇴 완료", ELogType.Player);
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

            LogPrintSystem.SystemLogPrint(transform, "장전 중", ELogType.Player);

            // 장전 애니메이션 출력

            sequence.SetDelay(reloadingDelay).OnComplete(() =>
            {
                StateRackYourBolt();
            });
        }
    }

    void StateRackYourBolt()
    {
        state = EState.RackYourBolt;

        sequence = DOTween.Sequence();

        // 노리쇠 전진 애니메이션 출력

        if (isReloading)
        {
            sequence.SetDelay(forwardDelay).OnComplete(() =>
            {
                curAmmo++;

                isDiscountBullet = false;
                bulletUI.GetComponent<BalletUIController>().SetAmmo(isDiscountBullet);

                isReloading = false;
                LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo} -> ReloadTime : {playerManager.GetComponent<PlayerManager>().reloadTime}", ELogType.Player);

                state = EState.ReloadComplete;
            });
        }
    }
}