using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyScripts
{
    public class EEnemyController : MonoBehaviour
    {
        private Fsm fsm;
        private Fsm fsmLife;

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

        [Header("약점 노출을 위한 오브젝트")] 
        [SerializeField] private GameObject weak;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isInGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isSpecialAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> hasRemainAttackTime;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> rushDirection;

        void Start()
        {
            // About Attack FSM
            fsm = new Fsm();
            Blackboard b = new Blackboard();

            isAlive.Value = true;
            isGroggy.Value = false;
            isInGroggy.Value = false;
            canSpecialAttackReady.Value = true;
            hasRemainAttackTime.Value = false;
            rushDirection.Value = false;

            // Blackboard Initialize
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
        

            // Node Initialize
            var wait = new WaitNode();
            var trace = new EliteTraceNode();
            var attack = new NormalAttackNode();

            var ready = new EliteAttackReadyNode();
            // var rushReady = new EliteRushAttackReadyNode();

            var bombAttack = new EliteBombAttackNode();
            var rushAttack = new EliteRushAttackNode();

            var groggy = new EliteGroggyNode();
            var overRush = new EliteRushOverNode();

            // Connect Node
            wait.enterPlayer = trace;

            // normal : 0, bomb : 1, Rush : 2
            trace.attacks = new INode[3];
            trace.attacks[0] = attack;
            trace.attacks[1] = ready;
            trace.attacks[2] = ready;
            
            // Only use Rush Monster
            trace.overRush = overRush;
            
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
            
            // Only use Rush Monster
            overRush.enterPlayer = attack;
            
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
            if(!isAlive.Value)
                Destroy(gameObject);
            else
            {
                fsm.Update();
                fsmLife.Update();
                
                // Weakness Show Method
                WeakShow();
            }
        }

        // Called In Weakness Prefab
        public void WeakBreak()
        {
            isGroggy.Value = true;
        }

        public float GetMySpecialDamage()
        {
            return mySpecialAttackDamage.Value;
        }

        private void WeakShow()
        {
            weak.SetActive(isSpecialAttackReady.Value);
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