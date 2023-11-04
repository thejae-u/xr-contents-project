using System;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    enum MyType
    {
        Apple,
        Bomb
    }

    [SerializeField] private MyType type;
    public GameObject collision;
    public float value;
    
    private ItemColliderController collScript;

    private void Start()
    {
        collScript = collision.GetComponent<ItemColliderController>();
    }

    private void Update()
    {
        if (collision.activeSelf)
        {
            if (collScript.IsTimerEnd)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (type)
            {
                case MyType.Apple:
                    PlayerManager.Instance.PlayerRecovery(value);
                    Destroy(gameObject);
                    break;
                case MyType.Bomb:
                    collision.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                    collision.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
