using Spine.Unity;
using UnityEngine;
using static PlayerManager;
using static PlayerShot;

public class PlayerShotAnimationController : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerShot playerShot;
    private SkeletonAnimation skeletonAnimation;

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

    void Awake()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        playerShot = gameObject.GetComponent<PlayerShot>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        if (skeletonAnimation == null)
            return;
    }

    void Update()
    {
        switch (playerManager.state)
        {
            case EPlayerState.Idle:
                AnimIdle();
                break;
            case EPlayerState.Move:
                AnimMove();
                break;
            case EPlayerState.Jump:
                AnimJump();
                break;
            case EPlayerState.Hit:
                AnimHit();
                break;
            case EPlayerState.Dodge:
                AnimDodge();
                break;
            case EPlayerState.Dead:
                AnimDead();
                break;
        }

        switch (playerShot.state)
        {
            case EShotState.None:
                AnimNone();
                break;
            case EShotState.Backforward:
                AnimBackforward();
                break;
            case EShotState.Reloading:
                AnimReloading();
                break;
            case EShotState.Forward:
                AnimForward();
                break;
            case EShotState.Shot:
                AnimShot();
                break;
            case EShotState.BoltAction:
                AnimBoltAction();
                break;
        }
    }

    // Player movement related
    private void AnimIdle()
    {
        if (skeletonAnimation.AnimationName == "Idle") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }
    private void AnimMove()
    {
        if (skeletonAnimation.AnimationName == "Move") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Move, true);
    }
    private void AnimJump()
    {
        if (skeletonAnimation.AnimationName == "Jump") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }
    private void AnimHit()
    {
        if (skeletonAnimation.AnimationName == "Hit") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }
    private void AnimDodge()
    {
        if (skeletonAnimation.AnimationName == "Dodge") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }
    private void AnimDead()
    {
        if (skeletonAnimation.AnimationName == "Dead") return;

        skeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }

    // Player shooting related
    private void AnimNone()
    {
        if (skeletonAnimation.AnimationName == "None") return;

        skeletonAnimation.AnimationState.SetAnimation(1, "None", true);
    }
    private void AnimBackforward()
    {
        if (skeletonAnimation.AnimationName == "Backforward") return;

        skeletonAnimation.AnimationState.SetAnimation(1, Backforward, false);
    }
    private void AnimReloading()
    {
        if (skeletonAnimation.AnimationName == "Reloading") return;

        skeletonAnimation.AnimationState.SetAnimation(1, Reloading, false);
    }
    private void AnimForward()
    {
        if (skeletonAnimation.AnimationName == "Forward") return;

        skeletonAnimation.AnimationState.SetAnimation(1, Forward, false);
    }
    private void AnimShot()
    {
        if (skeletonAnimation.AnimationName == "Shot") return;

        skeletonAnimation.AnimationState.SetAnimation(1, Shot, false);
    }
    private void AnimBoltAction()
    {
        if (skeletonAnimation.AnimationName == "BoltAction") return;

       skeletonAnimation.AnimationState.SetAnimation(1, BoltAction, false);
    }
}