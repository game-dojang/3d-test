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
    [SerializeField] private LayerMask detectionTargetLayerMask;    // 추격 대상의 Layer Mask
    [SerializeField] private float detectionSightAngle = 30f;
    [SerializeField] private float minimumRunDistance = 1f;
    
    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float DetectionSightAngle => detectionSightAngle;
    public float MinimumRunDistance => minimumRunDistance;
    
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
    public static readonly int EnemyAniParamMoveSpeed = Animator.StringToHash("move_speed");
    
    // 추격 대상 찾기
    private Transform _targetTransform;
    private Collider[] _detectionResults = new Collider[1];
    
    private void Awake()
    {
        // 필수요소 초기화
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        // 상태 객체 초기화
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
        
        // 추격 정보 초기화
        _targetTransform = null;
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        
        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
    }

    private void Update()
    {
        if (State != EEnemyState.Dead && State != EEnemyState.None)
        {
            _states[State].Update();
        }
    }

    private void OnAnimatorMove()
    {
        var position = _animator.rootPosition;
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }
    
    // 일정 거리 안에 Player가 있는지 확인 후 있으면 Player의 Transform 정보를 반환,
    // 없으면 null 반환하는 함수.
    public Transform DetectionTargetInCircle()
    {
        if (!_targetTransform)
        {
            // _targetTransform이 없으면, 새롭게 찾기
            Physics.OverlapSphereNonAlloc(transform.position, patrolDetectionDistance, 
                _detectionResults, detectionTargetLayerMask);
            
            // detectionResults 배열 0번 인덱스에 값이 있다면 그걸 _targetTransform에 할당
            _targetTransform = _detectionResults[0]?.transform;
        }
        else
        {
            // _targetTransform이 있으면, 그 대상과의 거리를 계산해서 정해진 거리를 벗어나면 _targetTransform 정보 초기화
            float playerDistance = Vector3.Distance(transform.position, _targetTransform.position);
            if (playerDistance > PatrolDetectionDistance)
            {
                _targetTransform = null;
                _detectionResults[0] = null;
            }
        }
        return _targetTransform;
    }
    
    // 디버깅용 임시 함수
    private void OnDrawGizmos()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolDetectionDistance);
        
        // 시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0, detectionSightAngle, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionSightAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * patrolDetectionDistance);
        Gizmos.DrawRay(transform.position, leftDirection * patrolDetectionDistance);
        Gizmos.DrawRay(transform.position, transform.forward * patrolDetectionDistance);
        
        // Agent 목적지
        if (_navMeshAgent && _navMeshAgent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_navMeshAgent.destination, 0.5f);
            Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        }
    }
}
