using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public bool IsCameraStop { get; set; }
    private float trackSpeed = 10;

    private Vector3 prevPosition;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void LateUpdate()
    {
        if (IsCameraStop)
        {
            transform.position = prevPosition;
        }
        
        if (target)
        {
            var v = transform.position;
            prevPosition = v;
            v.x = target.position.x + 5.5f;
            transform.position = Vector3.MoveTowards(transform.position, v, trackSpeed * Time.deltaTime);
        }
    }
}