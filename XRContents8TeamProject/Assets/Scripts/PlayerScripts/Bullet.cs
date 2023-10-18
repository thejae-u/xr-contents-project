using UnityEngine;
using EnemyScripts;

public class Bullet : MonoBehaviour
{ 
    private GameObject playerManager;
    private NEnemyController nEnemyController;
    private EEnemyController eEnemyController;

    void Start()
    {
        playerManager = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NormalEnemy"))
        {           
            BulletDestroy();

            nEnemyController = collision.GetComponent<NEnemyController>();
            float damage = playerManager.GetComponent<PlayerManager>().playerNormalAtk;
            nEnemyController.DiscountHp(damage);
        }
        else if(collision.gameObject.CompareTag("EliteEnemy"))
        {
            BulletDestroy();
            
            eEnemyController = collision.GetComponent<EEnemyController>();
            float damege = playerManager.GetComponent<PlayerManager>().playerNormalAtk;
            eEnemyController.DiscountHp(damege);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            BulletDestroy();
        }
    }

    private void BulletDestroy()
    {
        Destroy(this.gameObject);
    }
}