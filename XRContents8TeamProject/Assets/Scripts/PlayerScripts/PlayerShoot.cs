using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;
    
    private GameObject playerManager;
    private Coroutine reloadCoroutine;

    private float lastFireTime = 0f;
    private int curAmmo = 0;

    public enum EState
    {
        Ready,
        Reloading,
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
        state = EState.Ready;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == EState.Ready && curAmmo > 0)
            {
                Shoot();
            }
            else if (state == EState.Reloading && reloadCoroutine != null)
            { // 재장전 중에 사격 버튼을 입력한 경우 장전을 취소한다.
                StopCoroutine(reloadCoroutine);        
                LogPrintSystem.SystemLogPrint(transform, $"Reload Cancel", ELogType.Player);

                state = EState.Ready;
                Shoot();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // 한발씩 장전해주기 위해 ReloadComplete 상태를 이용하여 장전이 완료된 후 curAmmo의 수치를 확인한다.
        if(state == EState.ReloadComplete)
        {
            if(curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                Reload();
            }
            else if(curAmmo >= playerManager.GetComponent<PlayerManager>().maxAmmo)
            {
                state = EState.Ready;
                LogPrintSystem.SystemLogPrint(transform, "Requset Complete", ELogType.Player);
            }
        }
    }

    public void Shoot()
    {
        if (Time.time >= lastFireTime + playerManager.GetComponent<PlayerManager>().shootDelaySpeed)
        {
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
        }
    }

    public void Reload()
    {
        
        if (curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo && reloadCoroutine == null)
        {
            LogPrintSystem.SystemLogPrint(transform, "Requset Reload", ELogType.Player);
            reloadCoroutine = StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        state = EState.Reloading;

        yield return new WaitForSeconds(playerManager.GetComponent<PlayerManager>().reloadTime);

        curAmmo++;
        reloadCoroutine = null;
        state = EState.ReloadComplete;

        LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo} -> ReloadTime : {playerManager.GetComponent<PlayerManager>().reloadTime}", ELogType.Player);
    }
}