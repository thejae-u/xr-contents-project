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

    [Header("Effect")]
    private bool playerHit = true;
    private bool playerStrongHit = true;

    public GameObject hit;
    public GameObject strongHit;

    private ParticleSystem hitParticle;
    private ParticleSystem strongHitParticle;

    private void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerShot = GameObject.Find("Player").GetComponent<PlayerShot>();
    }
    void Start()
    {
        isBounsDamage = playerShot.isPlayerCheckMaxGauge;
        LogPrintSystem.SystemLogPrint(transform, $"bounsDamage : {isBounsDamage}", ELogType.Player);

        hitParticle = hit.GetComponent<ParticleSystem>();
        strongHitParticle = strongHit.GetComponent<ParticleSystem>();
        playerHit = false;
        playerStrongHit = false;
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

        LogPrintSystem.SystemLogPrint(transform, $" 게이지 가득 참 상태 : {isBounsDamage}", ELogType.Player);
        // 게이지가 가득
        if (isBounsDamage)
        {
            if (!playerStrongHit) return;

            if (!strongHitParticle.isPlaying)
            {
                Instantiate(strongHit, transform.position, Quaternion.identity);
            }

            LogPrintSystem.SystemLogPrint(transform, "create strongHit effect", ELogType.Player);
        }
        else
        {
            if (!playerHit) return;

            if (!hitParticle.isPlaying)
            {
                Instantiate(hit, transform.position, Quaternion.identity);
            }

            LogPrintSystem.SystemLogPrint(transform, "create hit effect", ELogType.Player);
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