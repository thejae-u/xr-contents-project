using System;
using UnityEngine;
using System.Collections.Generic;

public class AliveNode : INode
{
    public INode dead;

    public INode Execute(Blackboard blackboard)
    {
        if (blackboard.GetData<ReferenceValueT<float>>("myHp").Value <= 0)
            return Fsm.GuardNullNode(this, dead);
        blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = true;
        return Fsm.GuardNullNode(this, this);
    }
}

public class DeadNode : INode
{
    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = false;
        return Fsm.GuardNullNode(this, this);
    }
}