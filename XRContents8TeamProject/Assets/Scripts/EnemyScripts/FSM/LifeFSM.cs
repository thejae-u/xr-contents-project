using UnityEngine;
using System;

public class AliveNode : INode
{
    public INode dead;

    public INode Execute(Blackboard blackboard)
    {
        var myHp = blackboard.GetData<ReferenceValueT<float>>("myHp");
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