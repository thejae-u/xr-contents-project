using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    public class NEnemyController : MonoBehaviour
    {
        private FSM fsm;
        private FSM fsmLife;

        [SerializeField] private ReferenceValueT<float> myHp;
        [SerializeField] private ReferenceValueT<float> myAttackDamage;
        [SerializeField] private ReferenceValueT<float> myTraceRange;
        [SerializeField] private ReferenceValueT<float> myAttackRange;

        void Start()
        {
            fsm = new FSM();
            Blackboard b = new Blackboard();
            b.AddData("myHp", myHp);
            b.AddData("myTransform", transform);
            b.AddData("myAttackDamage", myAttackDamage);
            b.AddData("myTraceRange", myTraceRange);
            b.AddData("myAttackRange", myAttackRange);
            b.AddData("playerTransform", GameObject.Find("Player").transform);

            var wait = new WaitNode();
            var trace = new NormalTraceNode();
            var attack = new NormalAttackNode();
            var hit = new HitNode();

            wait.enterPlayer = trace;
            wait.hitMe = hit;
            trace.playerEnter = attack;
            trace.playerExit = wait;
            trace.hitMe = hit;
            attack.hitMe = hit;
            attack.outOfAttackRange = trace;
            attack.outOfTraceRange = wait;

            fsmLife = new FSM();
            var alive = new AliveNode();
            var dead = new DeadNode();
            alive.dead = dead;

            fsmLife.Init(b, alive);
            fsm.Init(b, wait);
        }

        // Update is called once per frame
        void Update()
        {
            fsm.Update();
            fsmLife.Update();
        }
    }
}