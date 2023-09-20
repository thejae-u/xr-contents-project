using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemyScripts
{
    public enum EEliteType
    {
        Bomb,
        Run
    }

    public class EliteTraceNode : TraceNode
    {
        public INode[] attacks;
        public INode playerExit;

        public override INode Execute(Blackboard blackboard)
        {
            var type = Trace(blackboard);
            var myType = blackboard.GetData<ReferenceValueT<EEliteType>>("myType").Value;

            // Trace Logic
            Transform myTransform = blackboard.GetData<Transform>("myTransform");
            Transform playerTransform = blackboard.GetData<Transform>("playerTransform");

            myTransform.position = new Vector3(Mathf.MoveTowards(myTransform.position.x,
                    playerTransform.position.x,
                    blackboard.GetData<ReferenceValueT<float>>("myMoveSpeed").Value * Time.deltaTime),
                myTransform.position.y,
                myTransform.position.z);

            LogPrintSystem.SystemLogPrint(myTransform, "Now Tracing", ELogType.EnemyAI);

            switch (type)
            {
                case ETraceState.PlayerEnter:
                    int rNum = Random.Range(0, 2);
                    switch (rNum)
                    {
                        case 0:
                            // Normal Attack
                            return FSM.GuardNullNode(this,attacks[0]);
                        case 1:
                            // Special Attack
                            return FSM.GuardNullNode(this, myType == EEliteType.Bomb ? attacks[1] : attacks[2]);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ETraceState.PlayerTrace:
                    return FSM.GuardNullNode(this, this);
                    break;
                case ETraceState.PlayerExit:
                    return FSM.GuardNullNode(this, playerExit);
                default:
                    throw new Exception("Error");
            }
        }
    }

    public class EliteBombAttackReadyNode : INode
    {
        public INode enterGroggy;
        public INode failedAttack;

        public INode Execute(Blackboard blackboard)
        {
            // if Weakness Point Attack Success ->
            //      return enterGroggy (Groggy Call)
            // else 
            //      return failedAttack (Special Attack Call)
            
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteRunAttackReadyNode : INode
    {
        public INode enterGroggy;
        public INode failedAttack;

        public INode Execute(Blackboard blackboard)
        {
            // if Weakness Point Attack Success ->
            //      return enterGroggy (Groggy Call)
            // else 
            //      return failedAttack (Special Attack Call)
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteBombAttackNode : INode
    {
        public INode endAttack;

        public INode Execute(Blackboard blackboard)
        {
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteRunAttackNode : INode
    {
        public INode endAttack;

        public INode Execute(Blackboard blackboard)
        {
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteGroggyNode : INode
    {
        public INode endGroggy;

        public INode Execute(Blackboard blackboard)
        {
            // Spend Groggy Time and Start Animation
            return FSM.GuardNullNode(this, this);
        }
    }
}