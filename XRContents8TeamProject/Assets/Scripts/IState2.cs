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
    
    public void 
}