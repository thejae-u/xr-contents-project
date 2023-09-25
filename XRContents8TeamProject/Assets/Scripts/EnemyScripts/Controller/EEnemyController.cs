using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        [Header("특수 공격 사거리를 조정(돌진 몬스터)")]
        [SerializeField] private ReferenceValueT<float> myRushRange;
        [SerializeField] private ReferenceValueT<float> myOverRushRange;
        
        [Header("특수 공격 준비 시간을 조정")]
        [SerializeField] private ReferenceValueT<float> specialAttackWait;

        [Header("툭수 공격 대기 시간을 조정")] 
        [SerializeField] private ReferenceValueT<float> specialAttackCooldown;
        
        [Header("엘리트 몬스터 타입 선택")]
        [SerializeField] private ReferenceValueT<EEliteType> myType;

        [Header("폭탄 프리팹")] 
        [SerializeField] private GameObject bombPrefab;

        [Header("약점 노출을 위한 오브젝트")] 
        [SerializeField] private GameObject weak;

        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isGroggy;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isAlive;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isNowAttack;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> isSpecialAttackReady;
        [HideInInspector] [SerializeField] private ReferenceValueT<bool> canSpecialAttack;

        void Start()
        {
            // About Attack FSM
            fsm = new FSM();
            Blackboard b = new Blackboard();

            isAlive.Value = true;
            isGroggy.Value = false;

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
            
            // 특수 공격 그로기 가능 시간
            b.AddData("specialAttackWait", specialAttackWait);
            b.AddData("isGroggy", isGroggy);
            b.AddData("canSpecialAttack", canSpecialAttack);
            
            // Only Use Rush Monster
            b.AddData("myRushRange", myRushRange);
            b.AddData("myOverRushRange", myOverRushRange);
        

            // Node Initialize
            var wait = new WaitNode();
            var trace = new EliteTraceNode();
            var attack = new NormalAttackNode();

            var bombReady = new EliteBombAttackReadyNode();
            var rushReady = new EliteRushAttackReadyNode();

            var bombAttack = new EliteBombAttackNode();
            var rushAttack = new EliteRushAttackNode();

            var groggy = new EliteGroggyNode();
            var overRush = new EliteRushOverNode();

            // Connect Node
            wait.enterPlayer = trace;

            trace.attacks = new INode[3];
            trace.attacks[0] = attack;
            trace.attacks[1] = bombReady;
            trace.attacks[2] = rushReady;
            trace.overRush = overRush;
            trace.playerExit = wait;

            attack.outOfAttackRange = wait;
            
            bombReady.failedAttack = bombAttack;
            bombReady.enterGroggy = groggy;
            bombAttack.endAttack = wait;
            
            rushReady.failedAttack = rushAttack;
            rushReady.failedAttack = groggy;
            rushAttack.endAttack = wait;

            groggy.endGroggy = wait;
            overRush.enterPlayer = attack;
            
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
                
                // Weakness Show Method
                WeakShow();
            }
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
        }
    }
}