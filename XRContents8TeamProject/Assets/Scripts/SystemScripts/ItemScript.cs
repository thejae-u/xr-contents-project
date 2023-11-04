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
    [SerializeField] private float value;

    private PlayerManager player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (type)
            {
                case MyType.Apple:
                    // Increase HP
                    // player.IncreaseHp(value);
                    break;
                case MyType.Bomb:
                    // Decrease HP
                    // player.DecreaseHp(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Destroy(gameObject);
        }
    }
}
