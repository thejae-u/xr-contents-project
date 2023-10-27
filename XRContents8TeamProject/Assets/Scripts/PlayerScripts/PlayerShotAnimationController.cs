using Spine.Unity;
using UnityEngine;
using static PlayerManager;
using static PlayerShot;

public class PlayerShotAnimationController : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerShot playerShot;

    // 스파인 애니메이션을 위한 것
    private SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] animClip;

    // 애니메이션에 대한 구조체
    [SerializeField] private EPlayerState playerState;
    [SerializeField] private EShotState playerShotState;

    // 현재 처리되고 있는 애니메이션
    private string currentAnimation;

    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerShot = GameObject.Find("Player").GetComponent<PlayerShot>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    } 

    private void Move()
    {
        // move 상태일 때 애니메이션 적용
    }

    private void AsyncAnimation(AnimationReferenceAsset animClip,bool loop,float timeScale)
    {
        // 해당 애니메이션으로 변경한다.
        skeletonAnimation.state.SetAnimation(0,animClip,loop).TimeScale = timeScale;
    }
}