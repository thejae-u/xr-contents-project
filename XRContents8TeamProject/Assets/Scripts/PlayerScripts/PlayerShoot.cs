using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;
    
    private GameObject playerManager;
 
    Sequence sequence = DOTween.Sequence();
    Sequence sequenceReleaseTheBolt = DOTween.Sequence();

    private float lastFireTime = 0f;
    private int curAmmo = 0;

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
    }

    private void Start()
    {
        curAmmo = playerManager.GetComponent<PlayerManager>().maxAmmo;
        
        state = EState.Idle;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == EState.Idle && curAmmo > 0)
            {
                StateReleaseTheBolt();
            }
            else if (state == EState.ReleaseTheBolt)
            { // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
                sequenceReleaseTheBolt.Kill();
                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);
                StateRackYourBolt();
                StateShot();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            StateReleaseTheBolt();
        }

        // 한발씩 장전해주기 위해 ReloadComplete 상태를 이용하여 장전이 완료된 후 curAmmo의 수치를 확인한다.
        if(state == EState.ReloadComplete)
        {
            if(curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                StateReloading();
            }
            else if(curAmmo >= playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                state = EState.Idle;
                LogPrintSystem.SystemLogPrint(transform, "Requset Complete", ELogType.Player);
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
            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo}", ELogType.Player);

            StateBoltAction();
        }
    }

    void StateBoltAction()
    {
        // 볼트액션 애니메이션 출력

        float shotDelayTime = playerManager.GetComponent<PlayerManager>().shotDelaySpeed;

        sequence.SetDelay(shotDelayTime).OnComplete(() =>
        {

        });
    }

    void StateReleaseTheBolt()
    {        
        if (curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
        {
            // 노리쇠 후퇴 애니메이션 출력

            sequenceReleaseTheBolt.SetDelay(0.1f).OnComplete(() =>
            {

            });
        }
    }

    public void StateReloading()
    {
        sequence.SetDelay(0.5f).OnComplete(() =>
        {
            StateRackYourBolt();
        });
    }

    void StateRackYourBolt()
    {
        // 노리쇠 전진 애니메이션 출력

        sequence.SetDelay(0.1f).OnComplete(() =>
        {
            curAmmo++;
            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo} -> ReloadTime : {playerManager.GetComponent<PlayerManager>().reloadTime}", ELogType.Player);
        });

    }

    private IEnumerator ReloadRoutine()
    {
        state = EState.Reloading;

        yield return new WaitForSeconds(playerManager.GetComponent<PlayerManager>().reloadTime);

        curAmmo++;
        state = EState.ReloadComplete;

        LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo} -> ReloadTime : {playerManager.GetComponent<PlayerManager>().reloadTime}", ELogType.Player);
    }
}