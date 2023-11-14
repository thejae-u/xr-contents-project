using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    enum MyType
    {
        Apple,
        Bomb
    }

    [SerializeField] private MyType type;
    [SerializeField] private float value;
    
    private float range;
    

    private Transform playerTransform;
    

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        range = 3.0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (type)
            {
                case MyType.Apple:
                    PlayerManager.Instance.PlayerRecovery(value);
                    break;
                case MyType.Bomb:
                    PlayerManager.Instance.PlayerDiscountHp(value, transform.position.x);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Bullet"))
        {

            switch (type)
            {
                case MyType.Apple:
                    break;
                case MyType.Bomb:
                    var playerCollider = Physics2D.OverlapCircle(transform.position, range,
                        LayerMask.GetMask("Player"));
                    var enemiesCollider = Physics2D.OverlapCircleAll(transform.position, range,
                        LayerMask.GetMask("Enemy"));
                    if (playerCollider)
                        PlayerManager.Instance.PlayerDiscountHp(value, transform.position.x);

                    if (enemiesCollider.Length != 0)
                    {
                        foreach (Collider2D t in enemiesCollider)
                        {
                            if (t.transform.TryGetComponent<NEnemyController>(out var ne))
                            {
                                ne.DiscountHp(value);
                            }

                            if (t.transform.TryGetComponent<EEnemyController>(out var ee))
                            {
                                ee.DiscountHp(value);
                            }
                        }
                    }

                    StartCoroutine(Destroyer());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private IEnumerator Destroyer()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
