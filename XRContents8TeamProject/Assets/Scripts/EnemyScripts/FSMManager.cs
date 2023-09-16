using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace EnemyScripts
{
    [System.Serializable]
    public class ReferenceValueT<T> where T : struct
    {
        public T Value;
    }
    public class Blackboard
    {
        private Dictionary<string, object> table = new Dictionary<string, object>();

        public void AddData(string key, object obj)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Error");
            table.Add(key, obj);
        }

        public object GetData(string key) => table[key];

        public T GetData<T>(string key)
        {
            if (!table.ContainsKey(key))
                throw new Exception("Error");
            return (T)table[key];
        }
        
        public bool TryGetData<T>(string key, out T data) where T : class
        {

            if (!table.ContainsKey(key))
            {
                data = null;
                return false;
            }

            data = (T)table[key];
            return true;

        }

        public bool TryGetDataStruct<T>(string key, out T data) where T : struct
        {
            if (!table.ContainsKey(key))
            {
                data = new T();
                return false;
            }

            if (table[key] is not ReferenceValueT<T> rv) throw new Exception();

            data = rv.Value;
            return true;
        }
    }

    public class FSM
    {
        private Blackboard blackboard;
        private INode currentNode;

        public void Init(Blackboard blackboard, INode defaultNode)
        {
            currentNode = defaultNode;
            this.blackboard = blackboard;
        }

        public void Update()
        {
            currentNode = currentNode.Execute(blackboard);
        }
    }

    public interface INode
    {
        public INode Execute(Blackboard blackboard);
    }
    
    #region Life

    public class AliveNode : INode
    {
        public INode dead;
        
        public INode Execute(Blackboard blackboard)
        {
            if (blackboard.GetData<ReferenceValueT<float>>("myHp").Value <= 0) return dead;
            blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = true;
            return this;

        }
    }

    public class DeadNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            blackboard.GetData<ReferenceValueT<bool>>("isAlive").Value = false;
            return this;
        }
    }
    
    #endregion
    

    public class WaitNode : INode
    {
        public INode enterPlayer;

        public INode Execute(Blackboard blackboard)
        {
            //..대기로직
            var myTransform = blackboard.GetData<Transform>("myTransform");
            var playerTransform = blackboard.GetData<Transform>("playerTransform");

            float d1 = playerTransform.GetComponent<PlayerMovement>().MyRadius;
            float d2 = blackboard.GetData<ReferenceValueT<float>>("myTraceRange").Value;

            float distance = (myTransform.position - playerTransform.position).magnitude;

            if (d1 + d2 >= distance)
            {
                Debug.Log("next trace");
                return enterPlayer;
            }
            else
            {
                Debug.Log("waiting");
                return this;
            }
        }
    }
    #region Trace

    public enum ETraceState
    {
        PlayerEnter,
        PlayerExit,
        PlayerTrace
    }

    public abstract class TraceNode : INode
    {
        public ETraceState Trace(Blackboard blackboard)
        {
            // Trace logic
            Transform myTransform = blackboard.GetData<Transform>("myTransform");
            Transform playerTransform = blackboard.GetData<Transform>("playerTransform");
            
            
            float d1 = playerTransform.GetComponent<PlayerMovement>().MyRadius;
            float d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;
            float distance = (myTransform.position - playerTransform.position).magnitude;

            float traceRange = blackboard.GetData<ReferenceValueT<float>>("myTraceRange").Value;

            if (d1 + d2 >= distance)
                return ETraceState.PlayerEnter;
            
            if (distance >= traceRange)
                return ETraceState.PlayerExit;
            
            return ETraceState.PlayerTrace;
        }

        public abstract INode Execute(Blackboard blackboard);
    }

    public class NormalTraceNode : TraceNode
    {
        public INode playerEnter;
        public INode playerExit;
        
        public override INode Execute(Blackboard blackboard)
        {
            var type = Trace(blackboard);
            
            // Enemy Move
            Transform myTransform = blackboard.GetData<Transform>("myTransform");
            Transform playerTransform = blackboard.GetData<Transform>("playerTransform");

            myTransform.position = new Vector3(Mathf.MoveTowards(myTransform.position.x,
                    playerTransform.position.x,
                blackboard.GetData<ReferenceValueT<float>>("myMoveSpeed").Value * Time.deltaTime),
                myTransform.position.y,
                myTransform.position.z);
            
            Debug.Log("tracing");
            
            switch(type)
            {
                case ETraceState.PlayerEnter:
                    return playerEnter;
                    break;
                case ETraceState.PlayerExit:
                    return playerExit;
                    break;
                case ETraceState.PlayerTrace:
                    return this;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class EliteTraceNode : TraceNode
    {
        public INode[] attacks;
        public INode playerExit;

        public override INode Execute(Blackboard blackboard)
        {
            var type = Trace(blackboard);

            switch (type)
            {
                case ETraceState.PlayerEnter:
                    // return attack
                    return this;
                    break;
                case ETraceState.PlayerTrace:
                    return this;
                    break;
                case ETraceState.PlayerExit:
                    return playerExit;
                default:
                    throw new Exception("Error");
            }
        }
    }
    #endregion
    
    #region Elite Attack Ready

    public class EliteBombAttackReadyNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            return this;
        }
    }

    public class EliteRunAttackReadyNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            return this;
        }
    }
    
    #endregion
    
    #region Attack

    public class NormalAttackNode : INode
    {
        public INode outOfAttackRange;

        public INode Execute(Blackboard blackboard)
        {
            // animation start
            // Damage to Player
            Debug.Log("Player Attack!!!");
            Transform myTransform = blackboard.GetData<Transform>("myTransform");
            Transform playerTransform = blackboard.GetData<Transform>("playerTransform");
            float d1 = playerTransform.GetComponent<PlayerMovement>().MyRadius;
            float d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;
            float distance = (myTransform.position - playerTransform.position).magnitude;

            PlayerMovement player = playerTransform.GetComponent<PlayerMovement>();
            player.DiscountHp(blackboard.GetData<ReferenceValueT<float>>("myAttackDamage").Value);

            if (d1 + d2 >= distance)
                return this;
            return outOfAttackRange;
        }
    }

    public class EliteBombAttackNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            return this;
        }
    }

    public class EliteRunAttackNode : INode
    {
        public INode Execute(Blackboard blackboard)
        {
            return this;
        }
    }
    
    #endregion
}