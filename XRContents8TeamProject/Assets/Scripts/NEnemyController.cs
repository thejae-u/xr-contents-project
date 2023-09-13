using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEnemyController : MonoBehaviour, IState1, IState2
{
    private IState1 myState;

    override void IState2.EnemyIdleState()
    {

    }

    public float MyHp { get => myState.MyHp; set => myState.MyHp = value; }

    void Start()
    {
        myState.MyHp = 100;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(MyHp);
    }
}
