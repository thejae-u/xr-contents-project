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

        void Start()
        {
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

            // Connect Node
            wait.enterPlayer = trace;
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
    }
}