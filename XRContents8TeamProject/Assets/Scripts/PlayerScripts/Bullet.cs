using EnemyScripts;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10.0f;

    private Vector3 initialPosition;
    [SerializeField] private float fireDistance = 50.0f;

    [SerializeField] private NEnemyController nEnemyController;
    [SerializeField] private float playerDamage = 5.0f;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(initialPosition, transform.position) > fireDistance)
        {
            BulletDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("State : other Collision");
            BulletDestroy();
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("State : Enemy Collision");
            BulletDestroy();
            // 적의 HP를 Ammor의 damage만큼 감소 시킨다.
        }
    }

    private void BulletDestroy()
    {
        Destroy(this.gameObject);
    }
}
