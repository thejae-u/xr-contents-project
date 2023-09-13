using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState1
{
    enum ELifeState
    {
        Dead,
        Alive
    }

    public float MyHp { get; set; }

    public void EnemyDeadState()
    {
        // Destroy Call
    }

    public void EnemyAliveState()
    {
        if (MyHp <= 0)
        {
            // State Change
        }
    }
}