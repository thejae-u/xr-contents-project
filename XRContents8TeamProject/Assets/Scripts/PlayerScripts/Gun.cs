using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;
   
    private float lastFireTime = 0f;
    [Header("사격 속도")]
    [SerializeField] private float timeBetFire = 0.5f;
    [Header("재장전 시간")]
    [SerializeField] private float reloadTime = 2.0f;
    
    private int curAmmo = 6;
    [Header("최대 탄알")]
    [SerializeField] private int maxAmmo = 6;
    [Header("탄알 속도")]
    [SerializeField] private float bulletSpeed = 5.0f;   

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public enum EState
    {
        Ready,
        Empty,
        Reloading
    }

    public EState state { get; private set; }

    private void OnEnable()
    {
        curAmmo = maxAmmo;
        state = EState.Ready;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        Reload();
    }

    public void Shoot()
    {
        if (state == EState.Ready && Time.time >= lastFireTime + timeBetFire && curAmmo > 0)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            mousePosition.z = -cam.transform.position.z;

            Vector3 direction = (cam.ScreenToWorldPoint(mousePosition) - fireTransform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * bulletSpeed; 

            lastFireTime = Time.time;
            curAmmo--;
            LogPrintSystem.SystemLogPrint(transform, $"Current : {curAmmo}", ELogType.Player);

            if (curAmmo <= 0)
            {
                state = EState.Empty;
                StartCoroutine(ReloadRoutine());
            }

        }
    }

    public bool Reload()
    {
        if (state == EState.Empty || Input.GetKey(KeyCode.R) && curAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
            LogPrintSystem.SystemLogPrint(transform, "Reload Start", ELogType.Player);
            return true;
        }
        return false;
    }

    private IEnumerator ReloadRoutine()
    { 
        state = EState.Reloading;

        yield return new WaitForSeconds(reloadTime);

        LogPrintSystem.SystemLogPrint(transform, $"Reload Complete!!! -> ReloadTime : {reloadTime}", ELogType.Player);

        curAmmo = maxAmmo;

        state = EState.Ready;
    }
}
