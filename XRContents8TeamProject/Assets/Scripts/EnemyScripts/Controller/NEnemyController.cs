using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace EnemyScripts
{
    public class NEnemyController : MonoBehaviour
    {
        private Fsm fsm;
        private Fsm fsmLife;

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

        [Header("타입 지정(일반 몬스터 None)")] 
        [SerializeField] private ReferenceValueT<EEliteType> myType;
        
        [Header("경직 시간을 조정")] [Range(0.1f,0.5f)]
        [SerializeField] private float hitTime;

        [Header("넉백 거리를 조정")] [Range(0.1f, 3.0f)] 
        [SerializeField] private float knockbackPower;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isJumping;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGround;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canJumpNextNode;
        [HideInInspector] [SerializeField] private ReferenceValueT<ENode> myNode;
        
        private bool isHit;

        private Blackboard b;
        private SkeletonAnimation anim;

        void Start()
        {
            fsm = new Fsm();
            b = new Blackboard();
            anim = gameObject.GetComponent<SkeletonAnimation>();
            
            isAlive.Value = true;
            myNode.Value = ENode.Idle;
            
            b.AddData("myNode", myNode);
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
            b.AddData("isJumping", isJumping);
            b.AddData("canJumpNextNode", canJumpNextNode);
            b.AddData("isGround", isGround);

            var wait = new WaitNode();
            var trace = new NormalTraceNode();
            var attack = new NormalAttackNode();
            var jump = new JumpNode();

            wait.enterPlayer = trace;
            trace.playerEnter = attack;
            trace.playerExit = wait;
            trace.enterJump = jump;
            jump.endJump = trace;
            attack.outOfAttackRange = trace;

            fsmLife = new Fsm();
            var alive = new AliveNode();
            var dead = new DeadNode();
            alive.dead = dead;

            fsmLife.Init(b, alive);
            fsm.Init(b, wait);
        }

        private void Update()
        {
            Flip();
            
            if (!isHit)
                fsm.Update();
            
            if (isAlive.Value)
                fsmLife.Update();
            else
            {
                if (DOTween.IsTweening(this))
                {
                    DOTween.Kill(this);
                }

                /*
                if (anim.AnimationName == "dead" && anim.AnimationState.GetCurrent(0).IsComplete)
                {
                    Destroy(gameObject);
                }*/
                Destroy(gameObject);
            }
        }


        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.transform.CompareTag("Ground"))
            {
                isGround.Value = true;
            }
        }
        
        private void Flip()
        {
            var playerTransform = b.GetData<Transform>("playerTransform");
            float dir = playerTransform.position.x - transform.position.x;

            transform.rotation = dir > 0 ? new Quaternion(0, 180, 0, 0) : new Quaternion(0, 0, 0, 0);
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
            
            sequence.Play().SetId(this);
        }
    }
}