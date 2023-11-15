using UnityEngine;
using EnemyScripts;

public class Bullet : MonoBehaviour
{
    private float bulletCreateTime = 0;
    [SerializeField] private float bulletDestroyTime = 0.1f;

    private PlayerShot playershot;
    private NEnemyController nEnemyController;
    private EEnemyController eEnemyController;

    private void Start()
    {
        playershot = GameObject.Find("Player").GetComponent<PlayerShot>();
    }

    private void Update()
    {
        bulletCreateTime += Time.deltaTime;
        if(bulletCreateTime > bulletDestroyTime) 
        {    
            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, "TimeOver Bullet Destroy", ELogType.Player);

            bulletCreateTime = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NormalEnemy"))
        {
            nEnemyController = collision.GetComponent<NEnemyController>();
            nEnemyController.DiscountHp(playershot.curDamage);

            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, $"Hit {playershot.curDamage}  NormalEnemy", ELogType.Player);
        }
        else if(collision.gameObject.CompareTag("EliteEnemy"))
        { 
            eEnemyController = collision.GetComponent<EEnemyController>();
            eEnemyController.DiscountHp(playershot.curDamage);        
            
            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, $"Hit {playershot.curDamage} EliteEnemy", ELogType.Player);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, "Hit Ground", ELogType.Player);
        }
    }

    private void BulletDestroy()
    {
        Destroy(this.gameObject);
        LogPrintSystem.SystemLogPrint(transform, "Bullet Destroy", ELogType.Player);
    }
}