using UnityEngine;
using UnityEngine.UI;

public class AimUIController : MonoBehaviour
{
    public Transform aimingPoint;
    //public Image aimImage;
    public Image Gauge;

    private float fillGauge;
    private float fillMax;
    private Color currentColor;

    public bool isReadyWarningGauge = false;
    private bool isMaxGauge = false;

    private void Awake()
    {
        fillMax = GameObject.Find("Player").GetComponent<PlayerManager>().maxGauge;
        fillGauge = fillMax / 100;

        Gauge.fillAmount = 0;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        aimingPoint.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

        if (isMaxGauge)
        {
            Gauge.color = Color.yellow;
        }
    }

    public void SetGauge()
    {
        Gauge.color = Color.white;
        Gauge.fillAmount = Gauge.fillAmount + fillGauge + Time.deltaTime;
     
        if (Gauge.fillAmount == 1)
        {
            isMaxGauge = true;
        }
    }

    public void SetWarningGauge()
    {
        if (!isReadyWarningGauge)
        {
            Gauge.fillAmount = 1;
            Gauge.color = Color.red;
            currentColor = Gauge.color;
            isReadyWarningGauge = true;
        }

        if (Gauge.color.a > 0)
        {
            currentColor.a = Mathf.Clamp(currentColor.a - Time.deltaTime * 2f, 0.0f, 1.0f);
            Gauge.color = currentColor;
        }
        else if (Gauge.color.a <= 0)
        {
            InitGauge();
        }
    }

    public void InitGauge()
    {
        Gauge.fillAmount = 0;

        isMaxGauge = false;
        Gauge.color = Color.white;
    }
}
