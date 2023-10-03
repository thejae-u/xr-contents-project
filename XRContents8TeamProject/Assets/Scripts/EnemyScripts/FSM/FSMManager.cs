using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;


[Serializable]
public class ReferenceValueT<T> where T : struct
{
    public T Value;

    public static implicit operator T(ReferenceValueT<T> v)
    {
        return v.Value;
    }
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

public class Fsm
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
            return Fsm.GuardNullNode(this, enterPlayer);
        }

        LogPrintSystem.SystemLogPrint(myTransform, "Waiting", ELogType.EnemyAI);
        return Fsm.GuardNullNode(this, this);
    }
}

public enum ETraceState
{
    PlayerEnter,
    PlayerExit,
    PlayerTrace,
    PlayerEnterRush
}

public abstract class TraceNode : INode
{
    public ETraceState Trace(Blackboard blackboard)
    {
        // Whole Monster Use this variable
        // Bomb Elite Monster's Position is Locked
        
        Transform myTransform = blackboard.GetData<Transform>("myTransform");
        Transform playerTransform = blackboard.GetData<Transform>("playerTransform");

        float d1 = playerTransform.GetComponent<PlayerManager>().MyRadius;
        float d2 = blackboard.GetData<ReferenceValueT<float>>("myAttackRange").Value;

        EEliteType myType = blackboard.GetData<ReferenceValueT<EEliteType>>("myType").Value;

        // Distance of Player to Monster
        float distance = (myTransform.position - playerTransform.position).magnitude;
        float traceRange = blackboard.GetData<ReferenceValueT<float>>("myTraceRange").Value;

        if (myType != EEliteType.Rush)
        {
            if (d1 + d2 >= distance)
                return ETraceState.PlayerEnter;
            // Check Player Out Range when Tracing
            return distance >= traceRange ? ETraceState.PlayerExit : ETraceState.PlayerTrace;
        }

        // From here Only Use Rush Monster
        var hasRemainAttackTime = blackboard.GetData<ReferenceValueT<bool>>("hasRemainAttackTime");
        var isOverRush = blackboard.GetData<ReferenceValueT<bool>>("isOverRush");
        
        if (!hasRemainAttackTime.Value && !isOverRush.Value)
        {
            LogPrintSystem.SystemLogPrint(myTransform, "Check in Rush Monster", ELogType.EnemyAI);
            float rushD2 = blackboard.GetData<ReferenceValueT<float>>("myRushRange").Value;
            float overRushD2 = blackboard.GetData<ReferenceValueT<float>>("myOverRushRange").Value;

            if (d1 + rushD2 >= distance)
            {
                if (d1 + overRushD2 >= distance)
                {
                    isOverRush.Value = true;
                    return ETraceState.PlayerTrace;
                }
                return ETraceState.PlayerEnterRush;
            }
            
            // if Player In rush over range : Don't Special Attack
            if (d1 + rushD2 >= distance)
                return overRushD2 + d1 >= distance ? ETraceState.PlayerTrace : ETraceState.PlayerEnterRush;
        }

        // Check Normal Attack Range When Player Tracing
        if (d1 + d2 >= distance)
        {
            isOverRush.Value = false;
            return ETraceState.PlayerEnter;
        }

        // Check Player Out Range when Tracing
        return distance >= traceRange ? ETraceState.PlayerExit : ETraceState.PlayerTrace;
    }

    public abstract INode Execute(Blackboard blackboard);
}

public class JumpNode : INode
{
    public INode Execute(Blackboard blackboard)
    {
        // Enemy가 플레이어를 제데로 따라 갈 수 있도록 y값을 판단
        // 상향점프와 하향점프 모두 필요함
        
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        
        
        
        return Fsm.GuardNullNode(this, this);
    }
}