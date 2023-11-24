using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("카메라 흔들리는 강도 설정")]
    [SerializeField] public float cameraShakeAmount;

    float shakeTime; // 카메라가 흔들리는 시간

    private void Update()
    { 

        if(shakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * cameraShakeAmount + transform.position;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0.0f;
            transform.position = new Vector3(transform.position.x,0,-20);
        }
    }

    public void CameraShakeForTime(float time)
    {
        shakeTime = time;
    }
}
