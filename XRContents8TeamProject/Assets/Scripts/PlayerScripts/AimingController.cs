using UnityEngine;

public class AimingController : MonoBehaviour
{
    public Transform aimingPoint;

    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        aimingPoint.transform.position = new Vector3(mousePosition.x, mousePosition.y, aimingPoint.transform.position.z);
    }
}
