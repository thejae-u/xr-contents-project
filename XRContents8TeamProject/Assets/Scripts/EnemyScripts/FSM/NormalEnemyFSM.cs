using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NormalTraceNode : TraceNode
{
    public INode playerEnter;
    public INode playerExit;

    public override INode Execute(Blackboard blackboard)
    {
        var type = Trace(blackboard);

        // Trace Logic
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var myPos = myTransform.position;
        var playerPos = playerTransform.position;
        var myMoveSpeed = blackboard.GetData<ReferenceValueT<float>>("myMoveSpeed"); 

        myTransform.position = new Vector3(Mathf.MoveTowards(myPos.x,
                playerPos.x, myMoveSpeed.Value * Time.deltaTime),
            myPos.y, myPos.z);

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
        var isAttack = blackboard.GetData<ReferenceValueT<bool>>("isNowAttack");
        // animation start
        if (isAttack.Value)
            return Fsm.GuardNullNode(this, this);
        isAttack.Value = true;

        var sequence = DOTween.Sequence();
        sequence.SetDelay(1f).OnComplete(() => { isAttack.Value = false; });

        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");

        // 플레이어와 거리를 계산 하기 위한 변수 선언
        var d1 = playerTransform.GetComponent<PlayerManager>().MyRadius;
        var d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;
        var distance = (myTransform.position - playerTransform.position).magnitude;

        // 플레이어의 Component에 접근하기 위한 변수 선언
        var player = playerTransform.GetComponent<PlayerManager>();

        // 플레이어의 체력을 Discount
        var attackDamage = blackboard.GetData<ReferenceValueT<float>>("myAttackDamage").Value;
        player.PlayerDiscountHp(attackDamage);
        blackboard.GetData<ReferenceValueT<bool>>("isNowAttack").Value = true;

        LogPrintSystem.SystemLogPrint(myTransform, $"{attackDamage} Damage to Player!!", ELogType.EnemyAI);

        if (d1 + d2 >= distance)
            return this;

        return Fsm.GuardNullNode(this, outOfAttackRange);
    }
}