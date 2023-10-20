using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

namespace EnemyScripts
{
    public class EEnemyController : MonoBehaviour
    {
        # region AboutFSM
        private Fsm fsm;
        private Fsm fsmLife;

        // Blackboard Var initialize
        [Header("체력을 조정")]
        [SerializeField] private ReferenceValueT<float> myHp;
        
        [Header("속도를 조정")]
        [SerializeField] private ReferenceValueT<float> myMoveSpeed;
        
        [Header("공격 대미지를 조정 (폭탄 대미지 포함)")]
        [SerializeField] private ReferenceValueT<float> myAttackDamage;
        
        [Header("특수 공격 대미지를 조정 (돌진 몬스터)")]
        [SerializeField] private ReferenceValueT<float> mySpecialAttackDamage;
        
        [Header("탐지 거리를 조정")]
        [SerializeField] private ReferenceValueT<float> myTraceRange;
        
        [Header("공격 사거리를 조정")]
        [SerializeField] private ReferenceValueT<float> myAttackRange;
        
        [Header("특수 공격 사거리를 조정(돌진 몬스터)")]
        [SerializeField] private ReferenceValueT<float> myRushRange;
        [SerializeField] private ReferenceValueT<float> myOverRushRange;

        [Header("돌진 속도를 조정")]
        [SerializeField] private ReferenceValueT<float> myRushSpeed;
        
        [Header("특수 공격 준비 시간을 조정")]
        [SerializeField] private ReferenceValueT<float> specialAttackWait;

        [Header("툭수 공격 대기 시간을 조정")] 
        [SerializeField] private ReferenceValueT<float> specialAttackCooldown;

        [Header("무력화 상태 시간 조정")] 
        [SerializeField] private ReferenceValueT<float> groggyTime;
        
        [Header("엘리트 몬스터 타입 선택")]
        [SerializeField] private ReferenceValueT<EEliteType> myType;

        [Header("폭탄 프리팹")] 
        [SerializeField] private GameObject bombPrefab;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isInGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isSpecialAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> hasRemainAttackTime;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> rushDirection;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isOverRush;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isJumping;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canJumpNextNode;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGround;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isTimerWait;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isTimerEnded;
            
        [HideInInspector] [SerializeField] private ReferenceValueT<ENode> myNode;
        
        
        private Blackboard b;
        # endregion
        
        private SkeletonAnimation anim;
        [SerializeField] private List<GameObject> timers;

        private void Awake()
        {
            fsm = new Fsm();
            b = new Blackboard();
            anim = gameObject.GetComponent<SkeletonAnimation>();
            
            // First Node State Store
            b.AddData("myNode", myNode);

            // About First Weak Timer
            b.AddData("isTimerWait", isTimerWait);
            b.AddData("isTimerEnded", isTimerEnded);
            b.AddData("timers", timers);

            // About player Info
            b.AddData("playerTransform", GameObject.Find("Player").transform);

            // About Life
            b.AddData("isAlive", isAlive);
            b.AddData("myHp", myHp);

            // About the Monster Basic Info
            b.AddData("myTransform", transform);
            b.AddData("myType", myType);

            b.AddData("myTraceRange", myTraceRange);
            b.AddData("myMoveSpeed", myMoveSpeed);

            b.AddData("myAttackDamage", myAttackDamage);
            b.AddData("myAttackRange", myAttackRange);
            b.AddData("isSpecialAttackReady", isSpecialAttackReady);
            b.AddData("isNowAttack", isNowAttack);

            // Monster Special Attack Info
            b.AddData("bombPrefab", bombPrefab);
            b.AddData("mySpecialAttackDamage", mySpecialAttackDamage);
            b.AddData("specialAttackCooldown", specialAttackCooldown);
            b.AddData("canSpecialAttackReady", canSpecialAttackReady);
            b.AddData("hasRemainAttackTime", hasRemainAttackTime);

            // 특수 공격 그로기 가능 시간
            b.AddData("specialAttackWait", specialAttackWait);
            b.AddData("isGroggy", isGroggy);
            b.AddData("isInGroggy", isInGroggy);
            b.AddData("canSpecialAttack", canSpecialAttack);

            // Groggy Time
            b.AddData("groggyTime", groggyTime);

            // Only Use Rush Monster
            b.AddData("myRushRange", myRushRange);
            b.AddData("myOverRushRange", myOverRushRange);
            b.AddData("rushDirection", rushDirection);
            b.AddData("myRushSpeed", myRushSpeed);
            b.AddData("isOverRush", isOverRush);

            // Jump
            b.AddData("isJumping", isJumping);
            b.AddData("canJumpNextNode", canJumpNextNode);

            // Ground Check
            b.AddData("isGround", isGround);
        }

        void Start()
        {
            isAlive.Value = true;
            isGroggy.Value = false;
            isInGroggy.Value = false;
            canSpecialAttackReady.Value = true;
            hasRemainAttackTime.Value = false;
            rushDirection.Value = false;
            isOverRush.Value = false;
            isTimerWait.Value = false;
            isTimerEnded.Value = false;

            myNode.Value = ENode.Idle;
            
            // Node Initialize
            var wait = new WaitNode();
            var trace = new EliteTraceNode();
            var jump = new JumpNode();
            var attack = new NormalAttackNode();

            var ready = new EliteAttackReadyNode();

            var bombAttack = new EliteBombAttackNode();
            var rushAttack = new EliteRushAttackNode();

            var groggy = new EliteGroggyNode();

            // Connect Node
            wait.enterPlayer = trace;

            // normal : 0, bomb : 1, Rush : 2
            trace.attacks = new INode[3];
            trace.attacks[0] = attack;
            trace.attacks[1] = ready;
            trace.attacks[2] = ready;
            trace.enterJump = jump;
            
            // End of Jump
            jump.endJump = trace;
            
            // Player Out of Range
            trace.playerExit = wait;
            attack.outOfAttackRange = wait;

            // Share Ready Node
            ready.failedAttack = new INode[2];
            
            // bomb Ready
            ready.failedAttack[0] = bombAttack;
            bombAttack.endAttack = wait;
            
            // rush Ready
            ready.failedAttack[1] = rushAttack;
            rushAttack.endAttack = wait;
            
            // Share Groggy Node
            ready.enterGroggy = groggy;

            // End of Groggy
            groggy.endGroggy = wait;
            
            // About Life FSM
            fsmLife = new Fsm();
            
            var alive = new AliveNode();
            var dead = new DeadNode();
            
            alive.dead = dead;
            
            fsm.Init(b, wait);
            fsmLife.Init(b, alive);
        }

        private void Update()
        {
            if (!isAlive.Value)
            {
                if (DOTween.IsTweening(this))
                {
                    DOTween.Kill(this);
                }

                if (anim.AnimationName == "dead" && anim.AnimationState.GetCurrent(0).IsComplete)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                fsm.Update();
                fsmLife.Update();
                
                // Flip X Rotation
                Flip();
            }
        }

        
        // Called In WeakController
        public void WeakBreak()
        {
            isGroggy.Value = true;
        }

        public float GetMySpecialDamage()
        {
            return myAttackDamage.Value;
        }

        public Blackboard Data()
        {
            return b;
        }
        
        public void DiscountHp(float damage)
        {
            if (isSpecialAttackReady.Value)
                return;
            myHp.Value -= damage;
        }

        private void Flip()
        {
            var playerTransform = b.GetData<Transform>("playerTransform");
            float dir = playerTransform.position.x - transform.position.x;

            transform.rotation = dir > 0 ? new Quaternion(0, 180, 0, 0) : new Quaternion(0, 0, 0, 0);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.transform.CompareTag("Ground"))
            {
                isGround.Value = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, myAttackRange.Value);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, myTraceRange.Value);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, myRushRange.Value);

            if (myType.Value == EEliteType.Rush)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, myOverRushRange.Value);
            }
        }
    }
}