using System;
using System.Collections;
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
        [SerializeField] private ReferenceValueT<float> myMoveSpeed;
        
        [Header("경직 시간을 조정합니다")]
        [Range(0.1f,0.5f)]
        [SerializeField] private float hitTime;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        
        private bool isHit;
        private bool canCoroutineStart = true;

        void Start()
        {
            fsm = new FSM();
            Blackboard b = new Blackboard();

            isAlive.Value = true;

            b.AddData("isAlive", isAlive);
            b.AddData("myHp", myHp);
            b.AddData("myTransform", transform);
            b.AddData("myAttackDamage", myAttackDamage);
            b.AddData("myTraceRange", myTraceRange);
            b.AddData("myAttackRange", myAttackRange);
            b.AddData("playerTransform", GameObject.Find("Player").transform);
            b.AddData("myMoveSpeed", myMoveSpeed);
            b.AddData("isNowAttack", isNowAttack);

            var wait = new WaitNode();
            var trace = new NormalTraceNode();
            var attack = new NormalAttackNode();

            wait.enterPlayer = trace;
            trace.playerEnter = attack;
            trace.playerExit = wait;
            attack.outOfAttackRange = trace;

            fsmLife = new FSM();
            var alive = new AliveNode();
            var dead = new DeadNode();
            alive.dead = dead;

            fsmLife.Init(b, alive);
            fsm.Init(b, wait);
        }

        private void Update()
        {
            if (!isHit)
                fsm.Update();
            if (isAlive.Value)
                fsmLife.Update();
            else
                Destroy(gameObject);

            if (isNowAttack.Value && canCoroutineStart)
                StartCoroutine(AttackWaitTime());
        }

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, myAttackRange.Value);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, myTraceRange.Value);
        }

        public void HitCall()
        {
            if (!isHit)
                StartCoroutine(HitTime());
        }

        IEnumerator HitTime()
        {
            isHit = true;
            yield return new WaitForSeconds(0.2f);
            isHit = false;
        }

        IEnumerator AttackWaitTime()
        {
            canCoroutineStart = false;
            yield return new WaitForSeconds(0.5f);
            canCoroutineStart = true;
            isNowAttack.Value = false;
        }

        public void DiscountHp(float damage)
        {
            myHp.Value -= damage;
            LogPrintSystem.LogP().SystemLogPrint(transform, $"{damage} From Player -> remain {myHp.Value}");
            HitCall();
        }
    }
}