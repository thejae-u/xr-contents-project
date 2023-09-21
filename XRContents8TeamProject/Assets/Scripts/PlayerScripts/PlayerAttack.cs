using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform fireTransform;
    private GameObject playerManager;

    private float lastFireTime = 0f;
    private int curAmmo = 0;

    public enum EState
    {
        Ready,
        Empty,
        Reloading
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
            Shoot();
        }
        Reload();
    }

    public void Shoot()
    {
        if (state == EState.Ready && Time.time >= lastFireTime + playerManager.GetComponent<PlayerManager>().shootSpeed && curAmmo > 0)
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

            if (curAmmo <= 0)
            {
                state = EState.Empty;
                StartCoroutine(ReloadRoutine());
            }

        }
    }

    public bool Reload()
    {
        if (state == EState.Empty || Input.GetKey(KeyCode.R) && curAmmo < playerManager.GetComponent<PlayerManager>().maxAmmo)
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

        yield return new WaitForSeconds(playerManager.GetComponent<PlayerManager>().reloadTime);

        LogPrintSystem.SystemLogPrint(transform, $"Reload Complete!!! -> ReloadTime : {playerManager.GetComponent<PlayerManager>().reloadTime}", ELogType.Player);

        curAmmo = playerManager.GetComponent<PlayerManager>().maxAmmo;

        state = EState.Ready;
    }
}
