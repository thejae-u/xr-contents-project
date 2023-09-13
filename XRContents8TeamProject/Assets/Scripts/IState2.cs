using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState2
{
    enum EState
    {
        // Every Monster Apply
        Idle,
        Trace,
        NormalAttack,
        Hit,
        
        // Elite Monster Limited
        BombWait,
        BombAttack,
        RunWait,
        RunAttack,
        StormWait,
        StormAttack,
        
        // Boss, Elite Monster Limited
        Groggy
        
        // Boss Monster Limited
    }

    #region Base Enemy State

    public virtual void EnemyIdleState()
    {
           
    }

    public void EnemyTraceState()
    {

    }

    public void EnemyNormalAttackState()
    {

    }

    #endregion

    // Normal Enemy Limited
    public void EnmeyHitState()
    {
        
    }

    #region Elite Enemy State

    public void EnemyBombWaitState()
    {

    }

    public void EnemyBombAttackState()
    {

    }

    public void EnemyRunWaitState()
    {

    }

    public void EnemyRunAttackState()
    {

    }

    public void EnemyStormWaitState()
    {

    }

    public void EnemyStormAttackState()
    {

    }

    #endregion

    // Elite & Boss Limited
    public void EnemyGroggyState()
    {

    }

    #region Boss State

    #endregion


}