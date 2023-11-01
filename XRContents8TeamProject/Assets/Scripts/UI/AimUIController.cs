using UnityEngine;
using UnityEngine.UI;

public class AimUIController : MonoBehaviour
{
    public Transform aimingPoint;
    public Image Gauge;

    private float fillGauge;
    private float fillMax;
    private Color currentColor;

    public bool isReadyWarningGauge = false;
    private bool PlayerCheckMaxGauge = false;

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
    }

    public void SetGauge()
    {
        Gauge.fillAmount = Gauge.fillAmount + fillGauge + Time.deltaTime;
        Gauge.color = Color.white;

        if (Gauge.fillAmount == 1)
        {
            Gauge.color = Color.yellow;
            PlayerCheckMaxGauge = true;
            PlayerCheckMaxGauge = GameObject.Find("Player").GetComponent<PlayerShot>().isPlayerCheckMaxGauge;
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

        PlayerCheckMaxGauge = false;
        Gauge.color = Color.white;
    }
}
