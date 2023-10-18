using System;
using EnemyScripts;
using Spine.Unity;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
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
                break;
            case ENode.Trace:
                Run();
                break;
            case ENode.NormalAttack:
                // Attack();
                break;
            case ENode.Jump:
                // Jump();
                break;
            case ENode.SpecialAttackReady:
                // SpecialAttackWait();
                break;
            case ENode.SpecialAttack:
                // SpecialAttack();
                break;
            case ENode.Groggy:
                Groggy();
                break;
            case ENode.Dead:
                Dead();
                break;
            case ENode.Hit:
                // Hit();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region Animatinons
    
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

    private void Groggy()
    {
        if (anim.AnimationName == "paralyzation") return;
        anim.AnimationState.SetAnimation(0, "paralyzation", true);
    }

    private void Attack()
    {
        if (anim.AnimationName == "attack") return;
        anim.AnimationState.SetAnimation(0, "attack", true);
    }

    private void Jump()
    {
        if (anim.AnimationName == "jump") return;
        anim.AnimationState.SetAnimation(0, "jump", false);
    }

    private void SpecialAttackWait()
    {
        if (anim.AnimationName == "specialAttackWait") return;
        anim.AnimationState.SetAnimation(0, "specialAttackWait", true);
    }

    private void SpecialAttack()
    {
        if (anim.AnimationName == "specialAttack") return;
        anim.AnimationState.SetAnimation(0, "specialAttack", false);
    }

    private void Dead()
    {
        if (anim.AnimationName == "dead") return;
        anim.AnimationState.SetAnimation(0, "dead", false);
    }

    private void Hit()
    {
        if (anim.AnimationName == "hit") return;
        anim.AnimationState.SetAnimation(0, "hit", false);
    }
    
    #endregion
}
