using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotAnimationController : MonoBehaviour 
{
    public SkeletonAnimation skeletonAnimation;
    public SkeletonDataAsset skeletonDataAsset { get { return skeletonAnimation.skeletonDataAsset; } }


    [System.Serializable]
    public struct AnimationControl
    {
        [SpineAnimation]
        public string animationName;
        public bool loop;
        public KeyCode key;

        [Space]
        public bool useCustomMixDuration;
        public float mixDuration;
        //public bool useChainToControl;
        //public int chainToControl;
    }

    [System.Serializable]
    public class ControlledTrack
    {
        public List<AnimationControl> controls = new List<AnimationControl>();
    }

    [Space]
    public List<ControlledTrack> trackControls = new List<ControlledTrack>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
