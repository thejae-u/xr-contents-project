using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotAnimationController : MonoBehaviour, IHasSkeletonDataAsset, IHasSkeletonComponent
{
    public SkeletonAnimation skeletonAnimation;
    public SkeletonDataAsset SkeletonDataAsset { get { return skeletonAnimation.SkeletonDataAsset; } }
    public ISkeletonComponent SkeletonComponent { get { return skeletonAnimation; } }

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
        public KeyCode key; // 애니메이션 동작 키 설정

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

    void Start()
    {
        if (useOverrideMixDuration)
        {
            skeletonAnimation.AnimationState.Data.DefaultMix = overrideMixDuration;
        }
    }

    void Update()
    {
        var animationState = skeletonAnimation.AnimationState;

        // For each track
        for (int trackIndex = 0; trackIndex < trackControls.Count; trackIndex++)
        {
            // For each control in the track
            foreach (var control in trackControls[trackIndex].controls)
            {
                // Check each control, and play the appropriate animation.
                if (Input.GetKeyDown(control.key))
                {
                    TrackEntry trackEntry;
                    if (!string.IsNullOrEmpty(control.animationName))
                    {
                        trackEntry = animationState.SetAnimation(trackIndex, control.animationName, control.loop);
                    }
                    else
                    {
                        float mix = control.useCustomMixDuration ? control.mixDuration : animationState.Data.DefaultMix;
                        trackEntry = animationState.SetEmptyAnimation(trackIndex, mix);
                    }

                    if (trackEntry != null)
                    {
                        if (control.useCustomMixDuration)
                            trackEntry.MixDuration = control.mixDuration;

                        if (useOverrideAttachmentThreshold)
                            trackEntry.AttachmentThreshold = attachmentThreshold;

                        if (useOverrideDrawOrderThreshold)
                            trackEntry.DrawOrderThreshold = drawOrderThreshold;
                    }

                    // Don't parse more than one animation per track.
                    break;
                }
            }
        }

    }

}