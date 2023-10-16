using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using Spine.Unity;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private EEnemyController inst;
    private SkeletonAnimation anim;
        
    private void Start()
    {
        inst = gameObject.GetComponent<EEnemyController>();
        anim = gameObject.GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        switch (inst.Data().GetData<ReferenceValueT<ENode>>("myNode").Value)
        {
            case ENode.Idle:
                Idle();
                // Idle animation Call
                break;
            case ENode.Trace:
                Run();
                // Run Animation Call
                break;
            case ENode.NormalAttack:
                // NormalAttack Animation Call
                break;
            case ENode.Jump:
                // Jump Animation Call
                break;
            case ENode.SpecialAttackReady:
                // Special Attack Ready Animation Call
                break;
            case ENode.SpecialAttack:
                // Special Attack Animation Call
                break;
            case ENode.Groggy:
                // Groggy Animation Call
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Idle()
    {
        if (anim.AnimationName == "idle") return;
        anim.AnimationState.SetAnimation(0, "idle", true);
    }
    
    private void Run()
    {
        if (anim.AnimationName == "run") return;
        anim.AnimationState.SetAnimation(0, "run", true);
    }
    
}
