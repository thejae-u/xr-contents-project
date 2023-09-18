using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using Random = UnityEngine.Random;

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

        static public INode GuardNullNode(INode current, INode next)
        {
            if (next == null)
            {
                Debug.LogError(current.GetType());
                Debug.Assert(false);
            }

            return next;
        }
    }

    public interface INode
    {
        public INode Execute(Blackboard blackboard);
    }
    
    public class WaitNode : INode
    {
        public INode enterPlayer;

        public INode Execute(Blackboard blackboard)
        {
            //..대기로직
            var myTransform = blackboard.GetData<Transform>("myTransform");
            var playerTransform = blackboard.GetData<Transform>("playerTransform");

            float d1 = playerTransform.GetComponent<PlayerManager>().MyRadius;
            float d2 = blackboard.GetData<ReferenceValueT<float>>("myTraceRange").Value;

            float distance = (myTransform.position - playerTransform.position).magnitude;

            if (d1 + d2 >= distance)
            {
                LogPrintSystem.SystemLogPrint(myTransform, "Trace Start", ELogType.EnemyAI);
                return enterPlayer;
            }

            LogPrintSystem.SystemLogPrint(myTransform, "Waiting", ELogType.EnemyAI);
            return this;
        }
    }

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
            // Range Calculate
            Transform myTransform = blackboard.GetData<Transform>("myTransform");
            Transform playerTransform = blackboard.GetData<Transform>("playerTransform");
            
            
            float d1 = playerTransform.GetComponent<PlayerManager>().MyRadius;
            float d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;
            float distance = (myTransform.position - playerTransform.position).magnitude;

            float traceRange = blackboard.GetData<ReferenceValueT<float>>("myTraceRange").Value;

            // IF ENTER ATTACK RANGE
            if (d1 + d2 >= distance)
                return ETraceState.PlayerEnter;
            
            // IF EXIT TRACE RANGE
            if (distance >= traceRange)
                return ETraceState.PlayerExit;
            
            // IF NOW TRACING
            return ETraceState.PlayerTrace;
        }

        public abstract INode Execute(Blackboard blackboard);
    }
}