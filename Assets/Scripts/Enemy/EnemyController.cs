using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    // AI 관련
    [SerializeField] private float patrolDetectionDistance = 10f;   // 정찰 범위
    [SerializeField] private float patrolWaitTime = 1f;             // 정찰 대기 시간
    [SerializeField] private float patrolChance = 30f;              // 정촬 확률
    
    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    // 상태
    public enum EEnemyState
    {
        None, Idle, Patrol, Chase, Attack, Hit, Dead
    }
    public EEnemyState State { get; private set; }
    private Dictionary<EEnemyState, ICharacterState> _states;

    // 애니메이터 파라미터
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamPatrol = Animator.StringToHash("patrol");
    public static readonly int EnemyAniParamChase = Animator.StringToHash("chase");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        var idleEnemyState = new IdleEnemyState(this, _animator, _navMeshAgent);
        var patrolEnemyState = new PatrolEnemyState(this, _animator, _navMeshAgent);
        var chaseEnemyState = new ChaseEnemyState(this, _animator, _navMeshAgent);

        _states = new Dictionary<EEnemyState, ICharacterState>
        {
            { EEnemyState.Idle, idleEnemyState },
            { EEnemyState.Patrol, patrolEnemyState },
            { EEnemyState.Chase, chaseEnemyState },
        };
        SetState(EEnemyState.Idle);
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        
        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
    }
}
