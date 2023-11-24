using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("카메라 흔들리는 강도 설정")]
    [SerializeField] public float cameraShakeAmount;

    float shakeTime; // 카메라가 흔들리는 시간
    bool shakeEnabled;
    Vector3 cameraPosition;

    private void Update()
    {

        if (shakeEnabled)
        {
            if (shakeTime > 0)
            {
                transform.position = Random.insideUnitSphere * cameraShakeAmount + transform.position;
                transform.position = new Vector3(transform.position.x,transform.position.y,-20);
                shakeTime -= Time.deltaTime;
            }
            else
            {
                shakeTime = 0.0f;
                transform.position = cameraPosition;
                shakeEnabled = false;
            }
        }
    }

    public void CameraShakeForTime(float time)
    {
        shakeTime = time;
        cameraPosition = transform.position;
        shakeEnabled = true;
    }
}
