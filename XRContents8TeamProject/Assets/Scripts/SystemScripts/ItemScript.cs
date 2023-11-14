using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private enum EMyType
    {
        Apple,
        Bomb
    }

    [SerializeField] private EMyType type;
    [SerializeField] private float value;
    
    private float range;

    private void Start()
    {
        range = 3.0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (type)
            {
                case EMyType.Apple:
                    PlayerManager.Instance.PlayerRecovery(value);
                    break;
                case EMyType.Bomb:
                    // Play Effect
                    EffectController.Inst.PlayEffect(transform.position, "Bomb");
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
                case EMyType.Apple:
                    break;
                case EMyType.Bomb:
                    // Play Effect
                    EffectController.Inst.PlayEffect(transform.position, "Bomb");
                    
                    // Check Player & Enemy
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
