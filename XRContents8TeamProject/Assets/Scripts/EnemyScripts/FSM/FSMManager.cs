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

    public static INode GuardNullNode(INode current, INode next)
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
    PlayerEnterRush,
    NeedJump
}

public abstract class TraceNode : INode
{
    public INode enterJump;
    
    // Jump를 하기 위한 로직을 만들어야 함
    // x값이  공격 범위에 들어오지만 Attack 노드로 못넘어 갈때를 판단
    // y값을 체크하고 상향 or 하향 점프
    
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
        
        // For Jump Node
        float distanceForJump = Mathf.Abs(myTransform.position.x - playerTransform.position.x);
        var isJumping = blackboard.GetData<ReferenceValueT<bool>>("isJumping");
        if (isJumping.Value) return ETraceState.PlayerTrace;
        if (distanceForJump <= d2 && myType != EEliteType.Bomb)
        {
            float playerYPos = playerTransform.position.y;
            float myYPos = myTransform.position.y;

            float yPosCalc = Mathf.Abs(playerYPos - myYPos);

            return yPosCalc <= 5.0f ? ETraceState.PlayerTrace : ETraceState.NeedJump;
        }

        // Trace Logic
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
    public INode endJump;
    
    public INode Execute(Blackboard blackboard)
    {
        // Enemy가 플레이어를 제데로 따라 갈 수 있도록 y값을 판단
        // 상향점프와 하향점프 모두 필요함
        
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var isJumping = blackboard.GetData<ReferenceValueT<bool>>("isJumping");

        if (playerTransform.GetComponent<PlayerManager>().GetIsJumping())
        {
            return Fsm.GuardNullNode(this, endJump);
        }

        Vector3 jumpPos = new Vector3(myTransform.position.x,
            playerTransform.position.y, 0);
        
        isJumping.Value = true;
        myTransform.DOJump(jumpPos, 3f, 1, 1f).OnComplete(() =>
        {
            isJumping.Value = false;
        });

        LogPrintSystem.SystemLogPrint(myTransform, "Jump Execute", ELogType.EnemyAI);

        return Fsm.GuardNullNode(this, endJump);
    }
}