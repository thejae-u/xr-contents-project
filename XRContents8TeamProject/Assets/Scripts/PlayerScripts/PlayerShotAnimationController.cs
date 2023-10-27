using Spine.Unity;
using UnityEngine;
using static PlayerManager;
using static PlayerShot;

public class PlayerShotAnimationController : MonoBehaviour
{
    #region Inspector
    // PlayerState
    public AnimationReferenceAsset Idle;
    public AnimationReferenceAsset Move;
    public AnimationReferenceAsset Jump;
    public AnimationReferenceAsset Hit;
    public AnimationReferenceAsset Dodge;
    public AnimationReferenceAsset Dead;

    // PlayerShotState
    public AnimationReferenceAsset Backforward;
    public AnimationReferenceAsset Reloading;
    public AnimationReferenceAsset Forward;
    public AnimationReferenceAsset Shot;
    public AnimationReferenceAsset BoltAction;
    #endregion

    private PlayerManager playerManager;
    private PlayerShot playerShot;
    private SkeletonAnimation skeletonAnimation;

    [SerializeField] private EPlayerState playerState;
    [SerializeField] private EShotState playerShotState;

    void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerShot = GameObject.Find("Player").GetComponent<PlayerShot>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    void Update()
    {
        playerState = playerManager.state;
        LogPrintSystem.SystemLogPrint(transform, $"state = {playerState}", ELogType.Player);
        switch (playerState)
        {
            case EPlayerState.Idle:
                skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
                break;
            case EPlayerState.Move:
                skeletonAnimation.AnimationState.SetAnimation(0, Move, true);
                break;
            case EPlayerState.Jump:
                skeletonAnimation.AnimationState.SetAnimation(0, Jump, false);
                break;
            case EPlayerState.Hit:
                skeletonAnimation.AnimationState.SetAnimation(0, Hit, false);
                break;
            case EPlayerState.Dodge:
                skeletonAnimation.AnimationState.SetAnimation(0, Dodge, false);
                break;
            case EPlayerState.Dead:
                skeletonAnimation.AnimationState.SetAnimation(0, Dead, false);
                break;
        }

        playerShotState = playerShot.state;
        LogPrintSystem.SystemLogPrint(transform, $"Shot state = {playerShotState}", ELogType.Player);
        switch (playerShotState)
        {
            case EShotState.None:
                skeletonAnimation.AnimationState.SetAnimation(1, "None", true);
                break;
            case EShotState.Backforward:
                skeletonAnimation.AnimationState.SetAnimation(1, Backforward, false);
                break;
            case EShotState.Reloading:
                skeletonAnimation.AnimationState.SetAnimation(1, Reloading, false);
                break;
            case EShotState.Forward:
                skeletonAnimation.AnimationState.SetAnimation(1, Forward, false);
                break;
            case EShotState.Shot:
                skeletonAnimation.AnimationState.SetAnimation(1, Shot, false);
                break;
            case EShotState.BoltAction:
                skeletonAnimation.AnimationState.SetAnimation(1, BoltAction, false);
                break;
        }
    }

    void PlayerAnimation()
    {

    }
}