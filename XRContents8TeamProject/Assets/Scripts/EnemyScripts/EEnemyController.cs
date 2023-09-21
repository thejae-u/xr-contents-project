using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    public class EEnemyController : MonoBehaviour
    {
        private FSM fsm;
        private FSM fsmLife;

        // Blackboard Var initialize
        [Header("체력을 조정")]
        [SerializeField] private ReferenceValueT<float> myHp;
        [Header("속도를 조정")]
        [SerializeField] private ReferenceValueT<float> myMoveSpeed;
        [Header("공격 대미지를 조정")]
        [SerializeField] private ReferenceValueT<float> myAttackDamage;
        [Header("특수 공격 대미지를 조정")]
        [SerializeField] private ReferenceValueT<float> mySpecialAttackDamage;
        [Header("탐지 거리를 조정")]
        [SerializeField] private ReferenceValueT<float> myTraceRange;
        [Header("공격 사거리를 조정")]
        [SerializeField] private ReferenceValueT<float> myAttackRange;
        [Header("특수 공격 사거리를 조정")]
        [SerializeField] private ReferenceValueT<float> mySpecialAttackRange;
        [SerializeField] private ReferenceValueT<EEliteType> myType;
        
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttack;

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
            b.AddData("isAttackReady", isAttackReady);
            b.AddData("isGroggy", isGroggy);
            b.AddData("canSpecialAttack", canSpecialAttack);
            
            b.AddData("myType", myType);
            b.AddData("mySpecialAttackDamage", mySpecialAttackDamage);
            b.AddData("mySpecialAttackRange", mySpecialAttackRange);

            // Node Initialize
            
            var wait = new WaitNode();
            var trace = new EliteTraceNode();
            var attack = new NormalAttackNode();

            var bombReady = new EliteBombAttackReadyNode();
            var rushReady = new EliteRushAttackReadyNode();

            var bombAttack = new EliteBombAttackNode();
            var rushAttack = new EliteRushAttackNode();

            var groggy = new EliteGroggyNode();

            // Connect Node
            wait.enterPlayer = trace;

            trace.attacks = new INode[3];
            trace.attacks[0] = attack;
            trace.attacks[1] = bombReady;
            trace.attacks[2] = rushAttack;
            trace.playerExit = wait;

            attack.outOfAttackRange = wait;
            
            bombReady.failedAttack = bombAttack;
            bombReady.enterGroggy = groggy;
            bombAttack.endAttack = wait;

            rushReady.failedAttack = rushAttack;
            rushReady.failedAttack = groggy;
            rushAttack.endAttack = wait;

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