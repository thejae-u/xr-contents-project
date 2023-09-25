using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EnemyScripts
{
    public class NEnemyController : MonoBehaviour
    {
        private FSM fsm;
        private FSM fsmLife;

        [Header("체력 조정")]
        [SerializeField] private ReferenceValueT<float> myHp;
        [Header("공격 대미지 조정")]
        [SerializeField] private ReferenceValueT<float> myAttackDamage;
        [Header("탐지 거리 조정")]
        [SerializeField] private ReferenceValueT<float> myTraceRange;
        [Header("공격 거리 조정")]
        [SerializeField] private ReferenceValueT<float> myAttackRange;
        [Header("속도 조정")]
        [SerializeField] private ReferenceValueT<float> myMoveSpeed;
        
        [Header("경직 시간을 조정")] [Range(0.1f,0.5f)]
        [SerializeField] private float hitTime;

        [Header("넉백 거리를 조정")] [Range(0.1f, 3.0f)] 
        [SerializeField] private float knockbackPower;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        
        private bool isHit;

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
        }

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, myAttackRange.Value);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, myTraceRange.Value);
        }
        
        public void DiscountHp(float damage)
        {
            myHp.Value -= damage;
            LogPrintSystem.SystemLogPrint(transform, $"{damage} From Player -> remain {myHp.Value}", ELogType.EnemyAI);
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() =>
            {
                isHit = true;
            });
            // 플레이어와 자신의 포지션을 빼준다 -> 정규화 해준다 -> 속도를 곱한다
            // 자신의 위치와 구한 벡터를 더해준다
            var myPos = transform.position;
            var playerPos = GameObject.Find("Player").transform.position;

            var dirVector = (myPos - playerPos).normalized;

            myPos += dirVector * knockbackPower;
            
            sequence.Append(transform.DOMoveX(myPos.x, hitTime));
            
            // s.Join animation call
            
            sequence.AppendCallback(() =>
            {
                isHit = false;
            });
            
            sequence.Play();
        }
    }
}