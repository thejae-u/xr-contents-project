using UnityEngine;
using EnemyScripts;

public class Bullet : MonoBehaviour
{
    private float bulletCreateTime = 0;
    [SerializeField] private float bulletDestroyTime = 0.1f;

    public bool isBounsDamage;

    private PlayerManager playerManager;
    private PlayerShot playerShot;
    private NEnemyController nEnemyController;
    private EEnemyController eEnemyController;

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerShot = GameObject.Find("Player").GetComponent<PlayerShot>();
    }
    void Start()
    {
        isBounsDamage = playerShot.isMaxGauge;
        LogPrintSystem.SystemLogPrint(transform, $"bounsDamage : {isBounsDamage}", ELogType.Player);
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
            float damage = playerManager.playerNormalAtk;
            
            if (isBounsDamage) 
                damage += playerManager.playerBonusAtk;
            
            nEnemyController.DiscountHp(damage);

            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, $"Hit {damage}  NormalEnemy", ELogType.Player);
        }
        else if(collision.gameObject.CompareTag("EliteEnemy"))
        { 
            eEnemyController = collision.GetComponent<EEnemyController>();
            float damage = playerManager.playerNormalAtk;

            if (isBounsDamage)
                damage += playerManager.playerBonusAtk;

            eEnemyController.DiscountHp(damage);        
            
            BulletDestroy();
            LogPrintSystem.SystemLogPrint(transform, $"Hit {damage} EliteEnemy", ELogType.Player);
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