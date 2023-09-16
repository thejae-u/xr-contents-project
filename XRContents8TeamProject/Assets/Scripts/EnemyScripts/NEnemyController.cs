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

        private ReferenceValueT<bool> isAlive;
        
        private bool isHit;

        void Start()
        {
            fsm = new FSM();
            isAlive.Value = true;
            Blackboard b = new Blackboard();
            
            b.AddData("myHp", myHp);
            b.AddData("myTransform", transform);
            b.AddData("myAttackDamage", myAttackDamage);
            b.AddData("myTraceRange", myTraceRange);
            b.AddData("myAttackRange", myAttackRange);
            b.AddData("playerTransform", GameObject.Find("Player").transform);
            b.AddData("myMoveSpeed", myMoveSpeed);
            b.AddData("isAlive", isAlive);

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
            Debug.Log("Hit From Player");
            yield return new WaitForSeconds(0.2f);
            isHit = false;
        }

        public void DiscountHp(float damage)
        {
            myHp.Value -= damage;
            print($"Remain HP : {myHp.Value}");
            HitCall();
        }
    }
}