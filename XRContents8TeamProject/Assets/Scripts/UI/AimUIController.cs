using UnityEngine;
using UnityEngine.UI;

public class AimUIController : MonoBehaviour
{
    public Transform aimingPoint;
    public Image Gauge;

    private float fillGuage;
    private float fillMax;

    private void Start()
    {
        fillMax = GameObject.Find("Player").GetComponent<PlayerManager>().maxGauge;
        fillGuage = fillMax / 100;

        Gauge.fillAmount = 0;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        aimingPoint.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
    }

    public void SetGauge()
    {
        Gauge.fillAmount += fillGuage;
    }

    public void InitGauge()
    {
        Gauge.fillAmount = 0;
    }
}
