using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemyScripts
{
    public enum EEliteType
    {
        Bomb,
        Rush
    }

    public class EliteTraceNode : TraceNode
    {
        public INode[] attacks;
        public INode playerExit;
        public INode overRush;

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
                    // Bomb doesn't Normal Attack
                    return myType == EEliteType.Bomb
                        ? FSM.GuardNullNode(this, attacks[0])
                        : FSM.GuardNullNode(this, attacks[1]);
                
                // Only Use Rush Monster
                case ETraceState.PlayerEnterRush:
                    // Rush Monster Attack Node
                    LogPrintSystem.SystemLogPrint(
                        myTransform,
                        "Rush Entered",
                        ELogType.EnemyAI);
                    return FSM.GuardNullNode(this, attacks[2]);
                
                // Only Use Rush Monster
                case ETraceState.PlayerEnterOverRush:
                    return FSM.GuardNullNode(this, overRush);
                
                case ETraceState.PlayerTrace:
                    return FSM.GuardNullNode(this, this);
                
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
            if (!blackboard.GetData<ReferenceValueT<bool>>("isSpecialAttackReady").Value)
            {
                LogPrintSystem.SystemLogPrint(
                    blackboard.GetData<Transform>("myTransform"),
                    "Now Ready For Special Attack",
                    ELogType.EnemyAI);
                
                blackboard.GetData<ReferenceValueT<bool>>("isGroggy").Value = false;
                blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttack").Value = false;
                blackboard.GetData<ReferenceValueT<bool>>("isSpecialAttackReady").Value = true;
                Sequence sequence = DOTween.Sequence();
                sequence.SetDelay(blackboard.GetData<ReferenceValueT<float>>("specialAttackWait").Value).OnComplete(() =>
                {
                    LogPrintSystem.SystemLogPrint(
                        blackboard.GetData<Transform>("myTransform"),
                        "DOTween Callback Function Call",
                        ELogType.EnemyAI);
                    blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttack").Value = true;
                });
            }
            else
            {
                if (blackboard.GetData<ReferenceValueT<bool>>("isGroggy").Value)
                {
                    blackboard.GetData<ReferenceValueT<bool>>("isSpecialAttackReady").Value = false;
                    return FSM.GuardNullNode(this, enterGroggy);
                }

                if (blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttack").Value)
                {
                    blackboard.GetData<ReferenceValueT<bool>>("isSpecialAttackReady").Value = false;
                    return FSM.GuardNullNode(this, failedAttack);
                }

                return FSM.GuardNullNode(this, this);
            }

            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteRushAttackReadyNode : INode
    {
        public INode enterGroggy;
        public INode failedAttack;

        public INode Execute(Blackboard blackboard)
        {
            // if Weakness Point Attack Success ->
            //      return enterGroggy (Groggy Call)
            // else 
            //      return failedAttack (Special Attack Call)
            LogPrintSystem.SystemLogPrint(
                blackboard.GetData<Transform>("myTransform"),
                "Rush Ready State On",
                ELogType.EnemyAI);
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteBombAttackNode : INode
    {
        public INode endAttack;

        public INode Execute(Blackboard blackboard)
        {
            var playerTransform = blackboard.GetData<Transform>("playerTransform");
            var myTransform = blackboard.GetData<Transform>("myTransform");

            GameObject.Instantiate(blackboard.GetData<GameObject>("bombPrefab"),
                myTransform.position,
                quaternion.identity);

            Sequence sequence = DOTween.Sequence();
            
            LogPrintSystem.SystemLogPrint(
                blackboard.GetData<Transform>("myTransform"),
                "Bomb Attack",
                ELogType.EnemyAI);
            return FSM.GuardNullNode(this, endAttack);
        }
    }

    public class EliteRushAttackNode : INode
    {
        public INode endAttack;

        public INode Execute(Blackboard blackboard)
        {
            return FSM.GuardNullNode(this, this);
        }
    }

    public class EliteRushOverNode : INode
    {
        public INode enterPlayer;

        public INode Execute(Blackboard blackboard)
        {
            return FSM.GuardNullNode(this, enterPlayer);
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