using UnityEngine;
using EnemyScripts;

public class Bullet : MonoBehaviour
{
    private Vector3 initialPosition;
 
    private GameObject playerManager;
    private NEnemyController nEnemyController;

    void Start()
    {
        initialPosition = transform.position;
        playerManager = GameObject.Find("Player");
    }

    void Update()
    {
        if (Vector3.Distance(initialPosition, transform.position) > playerManager.GetComponent<PlayerManager>().fireDistance)
        {
            BulletDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {           
            BulletDestroy();

            nEnemyController = collision.GetComponent<NEnemyController>();
            float damage = playerManager.GetComponent<PlayerManager>().playerAtk;
            nEnemyController.DiscountHp(damage);
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