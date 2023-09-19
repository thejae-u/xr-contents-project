using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    public class EEnemyController : MonoBehaviour
    {
        private FSM fsm;
        private FSM fsmLife;

        // Blackboard Var initialize
        [SerializeField] private ReferenceValueT<float> myHp;
        [SerializeField] private ReferenceValueT<float> myMoveSpeed;
        [SerializeField] private ReferenceValueT<float> myAttackDamage;
        [SerializeField] private ReferenceValueT<float> mySpecialAttackDamage;
        [SerializeField] private ReferenceValueT<float> myTraceRange;
        [SerializeField] private ReferenceValueT<float> myAttackRange;
        [SerializeField] private ReferenceValueT<float> mySpecialAttackRange;
        [SerializeField] private ReferenceValueT<EEliteType> myType;
        
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowReady;

        void Start()
        {
            // About Attack FSM
            fsm = new FSM();
            Blackboard b = new Blackboard();

            isAlive.Value = true;
            isGroggy.Value = false;

            // Blackboard Initialize
            b.AddData("isAlive", isAlive);
            b.AddData("myHp", myHp);
            b.AddData("myTransform", transform);
            b.AddData("myAttackDamage", myAttackDamage);
            b.AddData("myTraceRange", myTraceRange);
            b.AddData("myAttackRange", myAttackRange);
            b.AddData("playerTransform", GameObject.Find("Player").transform);
            b.AddData("myMoveSpeed", myMoveSpeed);
            b.AddData("isNowAttack", isNowAttack);
            b.AddData("isNowReady", isNowReady);
            
            b.AddData("myType", myType);
            b.AddData("mySpecialAttackDamage", mySpecialAttackDamage);
            b.AddData("mySpecialAttackRange", mySpecialAttackRange);

            // Node Initialize
            
            var wait = new WaitNode();
            var trace = new EliteTraceNode();
            var attack = new NormalAttackNode();

            var bombReady = new EliteBombAttackReadyNode();
            var runReady = new EliteRunAttackReadyNode();

            var bombAttack = new EliteBombAttackNode();
            var runAttack = new EliteRunAttackNode();

            var groggy = new EliteGroggyNode();

            // Connect Node
            wait.enterPlayer = trace;

            trace.attacks = new INode[3];
            trace.attacks[0] = attack;
            trace.attacks[1] = bombReady;
            trace.attacks[2] = runAttack;
            trace.playerExit = wait;

            attack.outOfAttackRange = wait;
            
            bombReady.failedAttack = bombAttack;
            bombReady.enterGroggy = groggy;
            bombAttack.endAttack = wait;

            runReady.failedAttack = runAttack;
            runReady.failedAttack = groggy;
            runAttack.endAttack = wait;

            groggy.endGroggy = wait;
            
            // About Life FSM
            fsmLife = new FSM();
            
            var alive = new AliveNode();
            var dead = new DeadNode();
            
            alive.dead = dead;
            
            fsm.Init(b, wait);
            fsmLife.Init(b, alive);
        }

        void Update()
        {
            if(!isAlive.Value)
                Destroy(gameObject);
            else
            {
                fsm.Update();
                fsmLife.Update();
            }
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, myAttackRange.Value);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, myTraceRange.Value);
        }
    }
}