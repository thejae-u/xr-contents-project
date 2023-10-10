using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public bool IsCameraStop { get; set; }
    private float trackSpeed = 10;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void LateUpdate()
    {
        if (target)
        {
            var v = transform.position;
            v.x = target.position.x + 5.5f;
            transform.position = Vector3.MoveTowards(transform.position, v, trackSpeed * Time.deltaTime);
        }
    }
}