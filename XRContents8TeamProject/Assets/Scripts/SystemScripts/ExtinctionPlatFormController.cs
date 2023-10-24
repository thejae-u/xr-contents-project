using UnityEngine;
using DG.Tweening;

public class ExtinctionPlatFormController : MonoBehaviour
{
    Sequence sequence;

    private bool isPlayerColliding = false;
    private float curTime = 0f; 
    [SerializeField] private float extinctionDelayTime = 1.5f; 
    [SerializeField] private float respawnPlatform = 3f;

    private Vector3 initialPosition = Vector3.zero;

    [Header("좌우로 움직일 거리")]
    [SerializeField] private float targetX = 0.25f;
    [Header("거리 이동 소요 시간")]
    [SerializeField] private float moveDuration = 0.1f;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = true;
        }
    }

    private void Update()
    {
        if (isPlayerColliding)
        {
            curTime += Time.deltaTime;

            StartMoveAnimation();

            if (curTime > extinctionDelayTime)
            {
                SetObjectActive();
            }
        }
    }

    private void SetObjectActive()
    {
        sequence = DOTween.Sequence();

        gameObject.SetActive(false);
        sequence.SetDelay(respawnPlatform).OnComplete(() => 
        {
            gameObject.SetActive(true);
            transform.position = initialPosition;
            curTime = 0f;
            isPlayerColliding = false;
        });
    }

    private void StartMoveAnimation()
    {
        transform.DOLocalMoveX(initialPosition.x + targetX, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOLocalMoveX(initialPosition.x - targetX, 0.1f);
        });
    }
}
