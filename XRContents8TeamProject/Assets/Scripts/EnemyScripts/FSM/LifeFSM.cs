using UnityEngine;
using System;

public class AliveNode : INode
{
    public INode dead;

    public INode Execute(Blackboard blackboard)
    {
        var myHp = blackboard.GetData<ReferenceValueT<float>>("myHp");
        var myType = blackboard.GetData<ReferenceValueT<EEliteType>>("myType");
        var myTransform = blackboard.GetData<Transform>("myTransform");

        if (myHp.Value > 0.0f)
            return Fsm.GuardNullNode(this, this);

        switch (myType.Value)
        {
            case EEliteType.None:
                SoundManager.Inst.Play("NormalMonsterDead");
                break;
            case EEliteType.Rush:
                SoundManager.Inst.Play("RushMonsterDead");
                break;
            case EEliteType.Bomb:
                SoundManager.Inst.Play("BombMonsterDead");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Fsm.GuardNullNode(this, myHp.Value > 0.0f ? this : dead);
    }
}

public class DeadNode : INode
{
    public INode Execute(Blackboard blackboard)
    {
        var myTransform = blackboard.GetData<Transform>("myTransform");
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.Dead;
        blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = false;
        
        return Fsm.GuardNullNode(this, this);
    }
}