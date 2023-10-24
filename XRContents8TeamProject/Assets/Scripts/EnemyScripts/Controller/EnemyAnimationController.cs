using System;
using EnemyScripts;
using Spine.Unity;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private EEliteType myType;
    private EEnemyController instElite;
    private NEnemyController instNormal;
    private SkeletonAnimation anim;
        
    private void Start()
    {
        if (transform.CompareTag("EliteEnemy"))
        {
            instElite = gameObject.GetComponent<EEnemyController>();
            instNormal = null;
            myType = instElite.Data().GetData<ReferenceValueT<EEliteType>>("myType");
        }
        else
        {
            instElite = null;
            instNormal = gameObject.GetComponent<NEnemyController>();
            myType = instNormal.Data().GetData<ReferenceValueT<EEliteType>>("myType").Value;
        }

        anim = gameObject.GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        if (myType != EEliteType.None)
        {
            // Elite Enemy Animation Call
            switch (instElite.Data().GetData<ReferenceValueT<ENode>>("myNode").Value)
            {
                case ENode.Idle:
                    Idle();
                    break;
                case ENode.Trace:
                    Move();
                    break;
                case ENode.NormalAttack:
                    // Attack();
                    break;
                case ENode.Jump:
                    // Jump();
                    break;
                case ENode.SpecialAttackReady:
                     SpecialAttackWait();
                    break;
                case ENode.SpecialAttack:
                     SpecialAttack();
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
        else
        {
            // Normal Enemy Animation Call
            switch(instNormal.Data().GetData<ReferenceValueT<ENode>>("myNode").Value)
            {
                case ENode.Idle:
                    Idle();
                    break;
                case ENode.Trace:
                    Move();
                    break;
                case ENode.NormalAttack:
                    Attack();
                    break;
                case ENode.Jump:
                    // Jump();
                    break;
                case ENode.Dead:
                    Dead();
                    break;
                case ENode.Hit:
                    LogPrintSystem.SystemLogPrint(transform, "Hit Node Activate", ELogType.EnemyAI);
                    Hit();
                    break;
                
                // Use Elite Only
                case ENode.SpecialAttackReady:
                case ENode.SpecialAttack:
                case ENode.Groggy:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    
    private void Idle()
    {
        if (myType == EEliteType.None)
        {
            if (anim.AnimationName == "Monster_Idle") return;
            anim.AnimationState.SetAnimation(0, "Monster_Idle", true);
        }
        else
        {
            if (anim.AnimationName == "Rush_Idle") return;
            anim.AnimationState.SetAnimation(0, "Rush_Idle", true);
        }
    }
    
    private void Move()
    {
        if (myType == EEliteType.None)
        {
            if (anim.AnimationName == "Monster_Move") return;
            anim.AnimationState.SetAnimation(0, "Monster_Move", true);
        }
        else
        {
            if (anim.AnimationName == "Rush_Move") return;
            anim.AnimationState.SetAnimation(0, "Rush_Move", true);
        }
    }

    private void Groggy()
    {
        if (anim.AnimationName == "Rush_Groggy") return;
        anim.AnimationState.SetAnimation(0, "Rush_Groggy", true);
    }

    private void Attack()
    {
        if (myType == EEliteType.None)
        {
            if (anim.AnimationName == "Monster_Atk") return;
            anim.AnimationState.SetAnimation(0, "Monster_Atk", true);
        }
        else
        {
            if (anim.AnimationName == "") return;
            anim.AnimationState.SetAnimation(0, "", true);
        }
    }

    private void Jump()
    {
        if (anim.AnimationName == "") return;
        anim.AnimationState.SetAnimation(0, "", false);
    }

    private void SpecialAttackWait()
    {
        if (myType == EEliteType.None)
        {
            if (anim.AnimationName == "Monster_Atk_Ready") return;
            anim.AnimationState.SetAnimation(0, "Monster_Atk_Ready", true);
        }
        else
        {
            if (anim.AnimationName == "Rush_SpecialATK_Ready") return;
            anim.AnimationState.SetAnimation(0, "Rush_SpecialATK_Ready", true);
        }
    }

    private void SpecialAttack()
    {
        if (anim.AnimationName == "Rush_SpecialATK") return;
        anim.AnimationState.SetAnimation(0, "Rush_SpecialATK", false);
    }

    private void Dead()
    {
        if (myType == EEliteType.None)
        {
            if (anim.AnimationName == "Monster_Dead") return;
            anim.AnimationState.SetAnimation(0, "Monster_Dead", false);
        }
        else
        {
            if (anim.AnimationName == "Rush_Dead") return;
            anim.AnimationState.SetAnimation(0, "Rush_Dead", false);
        }
    }

    private void Hit()
    {
        if (anim.AnimationName == "Monster_Hit") return;
        anim.AnimationState.SetAnimation(0, "Monster_Hit", false);
    }
}
