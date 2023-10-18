using Spine.Unity;
using UnityEngine;

public class PlayerShotAnimationController : MonoBehaviour
{
    private PlayerShot playerShot;
    private SkeletonAnimation playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        playerShot = GetComponent<PlayerShot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
