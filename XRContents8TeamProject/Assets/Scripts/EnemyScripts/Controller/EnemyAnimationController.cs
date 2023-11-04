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
        if (CameraController.Inst.IsNowCutScene) return;
        
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
                    Attack();
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
            switch (instNormal.Data().GetData<ReferenceValueT<ENode>>("myNode").Value)
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
                    Hit();
                    break;
                case ENode.SpecialAttackReady:
                    SpecialAttackWait();
                    break;
                
                // Use Elite Only
                case ENode.SpecialAttack:
                case ENode.Groggy:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    private void Idle()
    {
        switch (myType)
        {
            case EEliteType.None when anim.AnimationName == "Monster_Idle":
                break;
            case EEliteType.None:
                anim.AnimationState.SetAnimation(0, "Monster_Idle", true);
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_Idle":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_Idle", true);
                break;
            case EEliteType.Bomb when anim.AnimationName == "Throw_Idle":
                break;
            case EEliteType.Bomb:
                anim.AnimationState.SetAnimation(0, "Throw_Idle", true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Move()
    {
        switch (myType)
        {
            case EEliteType.None when anim.AnimationName == "Monster_Move":
                break;
            case EEliteType.None:
                anim.AnimationState.SetAnimation(0, "Monster_Move", true);
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_Move":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_Move", true);
                break;
            case EEliteType.Bomb:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Groggy()
    {
        switch (myType)
        {
            case EEliteType.None:
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_Groggy":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_Groggy", true);
                break;
            case EEliteType.Bomb when anim.AnimationName == "Throw_Groggy":
                break;
            case EEliteType.Bomb:
                anim.AnimationState.SetAnimation(0, "Throw_Groggy", true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Attack()
    {
        switch (myType)
        {
            case EEliteType.None when anim.AnimationName == "Monster_Atk":
                if (anim.AnimationState.GetCurrent(0).IsComplete)
                {
                    anim.timeScale = 1.0f;
                    instNormal.Data().GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.Idle;
                }

                break;
            case EEliteType.None:
                anim.timeScale = 2.0f;
                anim.AnimationState.SetAnimation(0, "Monster_Atk", false);
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_ATK":
                if (anim.AnimationState.GetCurrent(0).IsComplete)
                    anim.AnimationState.SetAnimation(0, "Rush_ATK", false);
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_ATK", false);
                break;
            case EEliteType.Bomb:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Jump()
    {
        if (anim.AnimationName == "") return;
        anim.AnimationState.SetAnimation(0, "", false);
    }

    private void SpecialAttackWait()
    {
        switch (myType)
        {
            case EEliteType.None when anim.AnimationName == "Monster_Atk_Ready":
                return;
            case EEliteType.None:
                anim.AnimationState.SetAnimation(0, "Monster_Atk_Ready", true);
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_SpecialATK_Ready":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_SpecialATK_Ready", true);
                break;
            case EEliteType.Bomb when anim.AnimationName == "Throw_SpecialATK_Ready":
                break;
            case EEliteType.Bomb:
                anim.AnimationState.SetAnimation(0, "Throw_SpecialATK_Ready", true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void SpecialAttack()
    {
        switch (myType)
        {
            case EEliteType.None:
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_SpecialATK":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_SpecialATK", true);
                break;
            case EEliteType.Bomb when anim.AnimationName == "Throw_SpecialATK":
                break;
            case EEliteType.Bomb:
                anim.AnimationState.SetAnimation(0, "Throw_SpecialATK", false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Dead()
    {
        switch (myType)
        {
            case EEliteType.None when anim.AnimationName == "Monster_Dead":
                break;
            case EEliteType.None:
                anim.AnimationState.SetAnimation(0, "Monster_Dead", false);
                break;
            case EEliteType.Rush when anim.AnimationName == "Rush_Dead":
                break;
            case EEliteType.Rush:
                anim.AnimationState.SetAnimation(0, "Rush_Dead", false);
                break;
            case EEliteType.Bomb when anim.AnimationName == "Throw_Dead":
                break;
            case EEliteType.Bomb:
                anim.AnimationState.SetAnimation(0, "Throw_Dead", false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Hit()
    {
        if (anim.AnimationName == "Monster_Hit") return;
        anim.AnimationState.SetAnimation(0, "Monster_Hit", false);
    }
}
