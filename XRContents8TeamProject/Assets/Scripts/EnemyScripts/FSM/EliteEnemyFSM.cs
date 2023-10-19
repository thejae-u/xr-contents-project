using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public enum EEliteType
{
    Bomb,
    Rush,
    None
}

public class EliteTraceNode : TraceNode
{
    public INode[] attacks;
    public INode playerExit;

    public override INode Execute(Blackboard blackboard)
    {
        var type = Trace(blackboard);
        var myType = blackboard.GetData<ReferenceValueT<EEliteType>>("myType");
        
        // Trace Logic
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var myMoveSpeed = blackboard.GetData<ReferenceValueT<float>>("myMoveSpeed");
        var hasRemainAttackTime = blackboard.GetData<ReferenceValueT<bool>>("hasRemainAttackTime");
        var isGround = blackboard.GetData<ReferenceValueT<bool>>("isGround");
        

        var myPos = myTransform.position;

        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.Trace;

        if (isGround.Value)
        {
            myTransform.position = new Vector3(Mathf.MoveTowards(myPos.x,
                    playerTransform.position.x, myMoveSpeed.Value * Time.deltaTime),
                myPos.y, 0);
        }

        switch (type)
        {
            // All monster Use, Bomb Monster's Normal Attack is Special Attack
            case ETraceState.PlayerEnter:
                if (myType == EEliteType.Bomb)
                {
                    return hasRemainAttackTime.Value
                        ? Fsm.GuardNullNode(this, this)
                        : Fsm.GuardNullNode(this, attacks[1]);
                }

                return Fsm.GuardNullNode(this, attacks[0]);

            // Only Use Rush Monster
            case ETraceState.PlayerEnterRush:
                if (hasRemainAttackTime.Value)
                    return Fsm.GuardNullNode(this, this);
                
                LogPrintSystem.SystemLogPrint(myTransform, "Rush Entered", ELogType.EnemyAI);
                return Fsm.GuardNullNode(this, attacks[2]);

            case ETraceState.PlayerTrace:
                return Fsm.GuardNullNode(this, this);

            // All Monster Use
            case ETraceState.PlayerExit:
                return Fsm.GuardNullNode(this, playerExit);
            
            case ETraceState.NeedJump:
                return Fsm.GuardNullNode(this, enterJump);

            default:
                throw new Exception("Error");
        }
    }
}


public class EliteAttackReadyNode : INode
{
    public INode enterGroggy;
    
    // player failed attack weakness
    public INode[] failedAttack;

    private void Init(Blackboard blackboard)
    {
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var timers = blackboard.GetData<List<GameObject>>("timers");

        foreach (var obj in timers)
        {
            obj.SetActive(true);
        }

        myTransform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        myTransform.GetComponent<Collider2D>().enabled = false;
    }

    private void EndOfNode(Blackboard blackboard)
    {
        var myTransform = blackboard.GetData<Transform>("myTransform");
        
        myTransform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        myTransform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        myTransform.GetComponent<Collider2D>().enabled = true;
    }

    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.SpecialAttackReady;
        
        // Wait Time For Attack
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var specialAttackWait = blackboard.GetData<ReferenceValueT<float>>("specialAttackWait");
        var canSpecialAttack = blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttack");
        var isSpecialAttackReady = blackboard.GetData<ReferenceValueT<bool>>("isSpecialAttackReady");
        var myType = blackboard.GetData<ReferenceValueT<EEliteType>>("myType");
        
        var timer = myTransform.GetComponentInChildren<WeakTimeController>(true);

        if (!isSpecialAttackReady.Value)
        {
            Init(blackboard);
            timer.Init(specialAttackWait.Value);
            
            isSpecialAttackReady.Value = true;
            canSpecialAttack.Value = false;
        }

        if (!timer.IsEnded) return Fsm.GuardNullNode(this, this);
        
        EndOfNode(blackboard);
        isSpecialAttackReady.Value = false;

        if (!timer.IsAttacked)
        {
            timer.Checked();
            return myType == EEliteType.Bomb
                ? Fsm.GuardNullNode(this, failedAttack[0])
                : Fsm.GuardNullNode(this, failedAttack[1]);
        }

        timer.Checked();
        return Fsm.GuardNullNode(this, enterGroggy);

    }
}

public class EliteBombAttackNode : INode
{
    public INode endAttack;

    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.SpecialAttack;
        
        var sequence = DOTween.Sequence();
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var bombPrefab = blackboard.GetData<GameObject>("bombPrefab");
        var canSpecialAttackReady = blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttackReady");
        var specialAttackCooldown = blackboard.GetData<ReferenceValueT<float>>("specialAttackCooldown");
        var hasRemainAttackTime = blackboard.GetData<ReferenceValueT<bool>>("hasRemainAttackTime");

        // 폭탄 생성 시 몬스터의 자식으로 생성
        var myPos = myTransform.position;
        Vector3 bombPos = new Vector3(myPos.x, myPos.y, 0);
        var bomb = GameObject.Instantiate(bombPrefab, bombPos,
            quaternion.identity);
        bomb.transform.SetParent(myTransform);

        LogPrintSystem.SystemLogPrint(myTransform, "Bomb Attack", ELogType.EnemyAI);

        canSpecialAttackReady.Value = false;
        hasRemainAttackTime.Value = true;

        sequence.SetDelay(specialAttackCooldown.Value).OnComplete(() =>
        {
            hasRemainAttackTime.Value = false;
            LogPrintSystem.SystemLogPrint(myTransform, "Now Can Special Attack", ELogType.EnemyAI);
        }).SetId(this);

        return Fsm.GuardNullNode(this, endAttack);
    }
}

public class EliteRushAttackNode : INode
{
    public INode endAttack;

    private bool CheckPlayer(Blackboard blackboard)
    {
        var myAttackRange = blackboard.GetData<ReferenceValueT<float>>("myAttackRange");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var mySpecialAttackDamage = blackboard.GetData<ReferenceValueT<float>>("mySpecialAttackDamage");
        
        float playerRange = playerTransform.GetComponent<PlayerManager>().MyRadius;

        Vector3 myPos = new Vector3(myTransform.position.x, myTransform.position.y, 0);
        
        float distance = (playerTransform.position - myPos).magnitude;

        if (playerRange + myAttackRange.Value >= distance)
        {
            playerTransform.GetComponent<PlayerManager>().PlayerDiscountHp(mySpecialAttackDamage.Value,
                myTransform.position.x);
            return true;
        }
        return playerRange + myAttackRange.Value >= distance;
    }

    private void InitSetting(Blackboard blackboard)
    {
        var sequence = DOTween.Sequence();
        var isNowAttack = blackboard.GetData<ReferenceValueT<bool>>("isNowAttack");
        var canSpecialAttackReady = blackboard.GetData<ReferenceValueT<bool>>("canSpecialAttackReady");
        var hasRemainAttackTime = blackboard.GetData<ReferenceValueT<bool>>("hasRemainAttackTime");
        var specialAttackCooldown = blackboard.GetData<ReferenceValueT<float>>("specialAttackCooldown");
        
        isNowAttack.Value = false;
        canSpecialAttackReady.Value = false;
        hasRemainAttackTime.Value = true;
        
        sequence.SetDelay(specialAttackCooldown.Value).OnComplete(() =>
        {
            hasRemainAttackTime.Value = false;
        }).SetId(this);
    }

    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.SpecialAttack;
        
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var playerTransform = blackboard.GetData<Transform>("playerTransform");
        var isNowAttack = blackboard.GetData<ReferenceValueT<bool>>("isNowAttack");
        var rushDirection = blackboard.GetData<ReferenceValueT<bool>>("rushDirection");
        var myRushSpeed = blackboard.GetData<ReferenceValueT<float>>("myRushSpeed");

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        
        if (!isNowAttack)
        {
            rushDirection.Value = Vector3.Normalize(playerTransform.position - myTransform.position).x > 0.0f;
            isNowAttack.Value = true;
        }

        Vector3 pos = Camera.main.WorldToViewportPoint(myTransform.position);

        if (rushDirection)
        {
            if (pos.x < 1.0f)
            {
                pos = new Vector3(Mathf.MoveTowards(pos.x, 1.0f,
                    myRushSpeed.Value * Time.deltaTime), pos.y, 10.0f);
                if (CheckPlayer(blackboard))
                {
                    InitSetting(blackboard);
                    Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, false);
                    return Fsm.GuardNullNode(this, endAttack);
                }
            }
            else
            {
                InitSetting(blackboard);
                Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, false);
                return Fsm.GuardNullNode(this, endAttack);
            }

            myTransform.position = Camera.main.ViewportToWorldPoint(pos);
        }
        else
        {
            if (pos.x > 0.0f)
            {
                pos = new Vector3(Mathf.MoveTowards(pos.x, 0.0f,
                    myRushSpeed.Value * Time.deltaTime), pos.y, 10.0f);
                if (CheckPlayer(blackboard))
                {
                    InitSetting(blackboard);
                    Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, false);
                    return Fsm.GuardNullNode(this, endAttack);
                }
            }
            else
            {
                InitSetting(blackboard);
                Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, false);
                return Fsm.GuardNullNode(this, endAttack);
            }

            myTransform.position = Camera.main.ViewportToWorldPoint(pos);
        }
        
        return Fsm.GuardNullNode(this, this);
    }
}

public class EliteGroggyNode : INode
{
    public INode endGroggy;

    public INode Execute(Blackboard blackboard)
    {
        blackboard.GetData<ReferenceValueT<ENode>>("myNode").Value = ENode.Groggy;
        
        var myTransform = blackboard.GetData<Transform>("myTransform");
        var groggyTime = blackboard.GetData<ReferenceValueT<float>>("groggyTime");
        var isGroggy = blackboard.GetData<ReferenceValueT<bool>>("isGroggy");
        var isInGroggy = blackboard.GetData<ReferenceValueT<bool>>("isInGroggy");

        LogPrintSystem.SystemLogPrint(myTransform, "In Groggy", ELogType.EnemyAI);

        // Spend Groggy Time and Start Animation
        var sequence = DOTween.Sequence();

        if (!isInGroggy.Value)
        {
            isInGroggy.Value = true;
            sequence.SetDelay(groggyTime.Value).OnComplete(() =>
            {
                isGroggy.Value = false;
                isInGroggy.Value = false;
            }).SetId(this);
        }

        return !isGroggy.Value ?
            Fsm.GuardNullNode(this, endGroggy) : Fsm.GuardNullNode(this, this);
    }
}