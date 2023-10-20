using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class NormalTraceNode : TraceNode
{
    public INode playerEnter;
    public INode playerExit;

    public override INode Execute(Blackboard blackboard)
    {
        var type = Trace(blackboard);

        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.Trace;
        
        // Trace Logic
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var myPos = myTransform.position;
        var playerPos = playerTransform.position;
        var myMoveSpeed = blackboard.GetData<ReferenceValueT<float>>("myMoveSpeed");
        var isGround = blackboard.GetData<ReferenceValueT<bool>>("isGround");


        if (isGround.Value)
        {
            LogPrintSystem.SystemLogPrint(myTransform, "isGround -> Trace Player", ELogType.EnemyAI);
            myTransform.position = new Vector3(Mathf.MoveTowards(myPos.x,
                    playerPos.x, myMoveSpeed.Value * Time.deltaTime),
                myPos.y, myPos.z);
        }
        else
        {
            LogPrintSystem.SystemLogPrint(myTransform, "isGround FALSE", ELogType.EnemyAI);
        }

        LogPrintSystem.SystemLogPrint(myTransform, "Now Tracing", ELogType.EnemyAI);

        switch (type)
        {
            case ETraceState.PlayerEnter:
                return Fsm.GuardNullNode(this, playerEnter);
            case ETraceState.PlayerExit:
                return Fsm.GuardNullNode(this, playerExit);
            case ETraceState.PlayerTrace:
                return Fsm.GuardNullNode(this, this);
            case ETraceState.NeedJump:
                return Fsm.GuardNullNode(this, enterJump);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public class NormalAttackNode : INode
{
    public INode outOfAttackRange;

    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.NormalAttack;
        
        var isNowAttack = blackboard.GetData<ReferenceValueT<bool>>("isNowAttack");
        
        if (isNowAttack.Value)
            return Fsm.GuardNullNode(this, this);

        // Attack On
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");

        // 거리 계산을 위한 변수
        var d1 = playerTransform.GetComponent<PlayerManager>().MyRadius;
        var d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;
        var distance = (myTransform.position - playerTransform.position).magnitude;
        
        var player = playerTransform.GetComponent<PlayerManager>();

        var attackDamage = blackboard.GetData<ReferenceValueT<float>>("myAttackDamage").Value;
        
        var sequence = DOTween.Sequence();

        if (d1 + d2 < distance)
        {
            return Fsm.GuardNullNode(this, outOfAttackRange);
        }
        
        isNowAttack.Value = true;
        
        player.PlayerDiscountHp(attackDamage, myTransform.position.x);
        GameManager.Inst.HitPlayer();

        sequence.SetDelay(1.5f).OnComplete(() =>
        {
            isNowAttack.Value = false;
        }).SetId(this);

        
        LogPrintSystem.SystemLogPrint(myTransform, $"{attackDamage} Damage to Player!!", ELogType.EnemyAI);
        return Fsm.GuardNullNode(this, this);
    }
}