using System;
using UnityEngine;
using System.Collections.Generic;

namespace EnemyScripts
{
    public class AliveNode : INode
    {
        public INode dead;
        
        public INode Execute(Blackboard blackboard)
        {
            if (blackboard.GetData<ReferenceValueT<float>>("myHp").Value <= 0)
                return FSM.GuardNullNode(this, dead);
            blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = true;
            return FSM.GuardNullNode(this, this);

        }
    }

    public class DeadNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = false;
            return FSM.GuardNullNode(this, this);
        }
    }
}