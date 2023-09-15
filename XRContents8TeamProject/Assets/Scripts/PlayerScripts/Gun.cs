using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;

    [SerializeField] private float timeBetFire = 10.0f; // 발사 속도
    private float lastFireTime = 0f;
    [SerializeField] private float reloadTime = 1.0f;
    [SerializeField] private float bulletSpeed = 5.0f;

    [SerializeField] private int maxAmmo = 6;
    private int curAmmo = 6;

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

            // 마우스 커서 위치에 맞춰 이동
            //mousePosition = new Vector3(mousePosition.x - 50, mousePosition.y - 100, mousePosition.z);
            //mousePosition.z = -cam.transform.position.z;

            Vector3 direction = (cam.ScreenToWorldPoint(mousePosition) - fireTransform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * bulletSpeed; 

            lastFireTime = Time.time;
            curAmmo--;
            Debug.Log("Current : " + curAmmo);

            if (curAmmo <= 0)
            {
                state = EState.Empty;
                StartCoroutine(ReloadRoutine());
            }

        }
    }

    public bool Reload()
    {
        Debug.Log("State : " + state);
        if (state == EState.Empty || Input.GetKey(KeyCode.R) && curAmmo < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
            Debug.Log("Reload Complete");
            return true;
        }
        return false;
    }

    private IEnumerator ReloadRoutine()
    { 
        state = EState.Reloading;

        yield return new WaitForSeconds(reloadTime);
        Debug.Log("reloadTime : " + reloadTime);

        curAmmo = maxAmmo;

        state = EState.Ready;
    }
}
