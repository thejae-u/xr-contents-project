using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10.0f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float fireDistance = 50.0f;
    private Vector3 initialPosition;

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

    private void BulletDestroy()
    {
        Destroy(gameObject);
    }
}
