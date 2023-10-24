using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using static PlayerManager;
using static PlayerShot;

public class PlayerShotAnimationController : MonoBehaviour, IHasSkeletonDataAsset, IHasSkeletonComponent
{
    public SkeletonAnimation skeletonAnimation;
    public SkeletonDataAsset SkeletonDataAsset { get { return skeletonAnimation.SkeletonDataAsset; } }
    public ISkeletonComponent SkeletonComponent { get { return skeletonAnimation; } }

    private PlayerManager playerManager;
    private PlayerShot playerShot;
    public EPlayerState playerState;
    public EShotState playerShotState;

    // 애니메이션의 믹스 지속 시간을 제어
    public bool useOverrideMixDuration;
    public float overrideMixDuration = 0.2f;

    // 첨부물 임계값을 재정의
    public bool useOverrideAttachmentThreshold = true;
    [Range(0f, 1f)] public float attachmentThreshold = 0.5f;

    // 그리기 순서 임계값을 재정의
    public bool useOverrideDrawOrderThreshold;
    [Range(0f, 1f)] public float drawOrderThreshold = 0.5f;

    [System.Serializable]
    public struct AnimationControl
    {
        [SpineAnimation]
        public string animationName; // 애니메이션 이름
        public bool loop;   // 애니메이션 반복 여부

        [Space]
        public bool useCustomMixDuration;
        public float mixDuration;
    }

    [System.Serializable]
    public class ControlledTrack
    {
        public List<AnimationControl> controls = new List<AnimationControl>();
    }

    [Space] public List<ControlledTrack> trackControls = new List<ControlledTrack>();

    void Awake() 
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerShot = GameObject.Find("Player").GetComponent<PlayerShot>();
    }

    void Start()
    {
        if (useOverrideMixDuration)
        {
            skeletonAnimation.AnimationState.Data.DefaultMix = overrideMixDuration;
        }
    }

    void Update()
    { 
    //    playerState = playerManager.state;
    //    playerShotState = playerShot.state;

    //    var animationState = skeletonAnimation.AnimationState;
    //    // 현재 플레이어 상태에 따라 애니메이션 선택
    //    switch (playerState)
    //    {
    //        case EPlayerState.Idle:
    //            PlayAnimationForState(0, "IdleAnimationName", true);
    //            break;
    //        case EPlayerState.Move:
    //            PlayAnimationForState(1, "RunningAnimationName", true);
    //            break;
    //        case EPlayerState.Jump:
    //            PlayAnimationForState(2, "JumpingAnimationName", false);
    //            break;
    //        case EPlayerState.Hit:
    //            PlayAnimationForState(3, "IdleAnimationName", true);
    //            break;
    //        case EPlayerState.Dodge:
    //            PlayAnimationForState(4, "RunningAnimationName", true);
    //            break;
    //        case EPlayerState.Dead:
    //            PlayAnimationForState(5, "JumpingAnimationName", false);
    //            break;
    //    }

    //    switch (playerShotState)
    //    {
    //        case EShotState.None:
    //            PlayAnimationForState(0, "IdleAnimationName", true);
    //            break;
    //        case EShotState.Backforward:
    //            PlayAnimationForState(1, "RunningAnimationName", true);
    //            break;
    //        case EShotState.Reloading:
    //            PlayAnimationForState(2, "JumpingAnimationName", false);
    //            break;
    //        case EShotState.Forward:
    //            PlayAnimationForState(3, "IdleAnimationName", true);
    //            break;
    //        case EShotState.Shot:
    //            PlayAnimationForState(4, "RunningAnimationName", true);
    //            break;
    //        case EShotState.BoltAction:
    //            PlayAnimationForState(5, "JumpingAnimationName", false);
    //            break;
    //    }
    }

    private void Idle()
    {

    }

    private void Move()
    {

    }

    private void Jump()
    {

    }

    private void Hit()
    {

    }

    private void Dodge()
    {

    }

    private void Dead()
    {

    }

    private void PlayAnimationForState(int trackIndex, string animationName, bool loop)
    {
        var animationState = skeletonAnimation.AnimationState;

        TrackEntry trackEntry;
        if (!string.IsNullOrEmpty(animationName))
        {
            trackEntry = animationState.SetAnimation(trackIndex, animationName, loop);
        }
        else
        {
            float mix = useOverrideMixDuration ? overrideMixDuration : animationState.Data.DefaultMix;
            trackEntry = animationState.SetEmptyAnimation(trackIndex, mix);
        }

        if (trackEntry != null)
        {
            if (useOverrideMixDuration)
                trackEntry.MixDuration = overrideMixDuration;

            if (useOverrideAttachmentThreshold)
                trackEntry.AttachmentThreshold = attachmentThreshold;

            if (useOverrideDrawOrderThreshold)
                trackEntry.DrawOrderThreshold = drawOrderThreshold;
        }
    }
}