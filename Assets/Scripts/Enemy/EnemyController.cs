using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[Serializable]
public struct EnemyStatus
{
    public int maxHp;
    public int hp;
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    // AI 관련
    [Header("AI")]
    [SerializeField] private float patrolDetectionDistance = 10f;   // 정찰 범위
    [SerializeField] private float patrolWaitTime = 1f;             // 정찰 대기 시간
    [SerializeField] private float patrolChance = 30f;              // 정촬 확률
    [SerializeField] private LayerMask detectionTargetLayerMask;    // 추격 대상의 Layer Mask
    [SerializeField] private float detectionSightAngle = 30f;
    [SerializeField] private float minimumRunDistance = 1f;

    [Header("Status")] [SerializeField] private EnemyStatus enemyStatus;

    [Header("Renderer")] [SerializeField] private Renderer enemyRenderer;

    // Ragdoll
    [SerializeField] private GameObject ragdollRoot;
    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;
    private CharacterJoint[] ragdollJoints;
    
    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float DetectionSightAngle => detectionSightAngle;
    public float MinimumRunDistance => minimumRunDistance;
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    
    // HP 표시
    private HPBarController _hpBarController;

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
    
    // Dead 연출
    private Rigidbody _rigidbody;
    private Collider _collider;
    
    private void Awake()
    {
        // 필수요소 초기화
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        // NavMesh Agent 설정
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
        
        // 상태 객체 초기화
        var idleEnemyState = new IdleEnemyState(this, _animator, _navMeshAgent);
        var patrolEnemyState = new PatrolEnemyState(this, _animator, _navMeshAgent);
        var chaseEnemyState = new ChaseEnemyState(this, _animator, _navMeshAgent);
        var attackEnemyState = new AttackEnemyState(this, _animator, _navMeshAgent);
        var hitEnemyState = new HitEnemyState(this, _animator, _navMeshAgent);
        var deadEnemyState = new DeadEnemyState(this, _animator, _navMeshAgent);

        _states = new Dictionary<EEnemyState, ICharacterState>
        {
            { EEnemyState.Idle, idleEnemyState },
            { EEnemyState.Patrol, patrolEnemyState },
            { EEnemyState.Chase, chaseEnemyState },
            { EEnemyState.Attack, attackEnemyState },
            { EEnemyState.Hit, hitEnemyState },
            { EEnemyState.Dead, deadEnemyState },
        };
        SetState(EEnemyState.Idle);
        
        // 추격 정보 초기화
        _targetTransform = null;

        _hpBarController = GetComponent<HPBarController>();
        
        // Ragdoll 요소 할당
        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>();
        ragdollRigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        ragdollJoints = ragdollRoot.GetComponentsInChildren<CharacterJoint>();
        
        // Ragdoll 비활성화
        SetRagdollEnable(false);
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

    public void SetHit(int damage, Vector3 attackDirection)
    {
        enemyStatus.hp -= damage;

        float hpResult = (float)enemyStatus.hp / enemyStatus.maxHp;
        _hpBarController.SetHp(hpResult);
        
        if (enemyStatus.hp <= 0)
        {
            // 사망 처리
            SetState(EEnemyState.Dead);

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            var direction = attackDirection;
            direction.y = 1f;
            direction = direction.normalized;
            var force = direction * 10f;
            
            _rigidbody.AddForce(force, ForceMode.Impulse);
            _collider.isTrigger = false;
        }
        else
        {
            SetState(EEnemyState.Hit);
            StartCoroutine(Knockback(attackDirection));
        }
    }

    private IEnumerator Knockback(Vector3 direction)
    {
        Vector3 knockbackDirection = direction;
        float knockbackDistance = 1f;
        float knockbackDuration = 0.2f;
        float elapsed = 0f;
        
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackDirection * knockbackDistance;
        targetPosition.y = transform.position.y;

        while (elapsed < knockbackDuration)
        {
            Vector3 lerpPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / knockbackDuration);
            lerpPosition.y = startPosition.y;
            transform.position = lerpPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
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

    private void SetRagdollEnable(bool isEnable)
    {
        foreach (var ragdollCollider in ragdollColliders)
        {
            ragdollCollider.enabled = isEnable;
        }

        foreach (var ragdollRigidbody in ragdollRigidbodies)
        {
            ragdollRigidbody.isKinematic = !isEnable;
            ragdollRigidbody.detectCollisions = isEnable;
        }
        
        _animator.enabled = !isEnable;
        _collider.enabled = !isEnable;
        _rigidbody.detectCollisions = !isEnable;
        
        _animator.Rebind();
        _animator.Update(0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            SetRagdollEnable(true);
            StartCoroutine(Dissolve());
        }
    }

    IEnumerator Dissolve()
    {
        var propertyBlock = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(propertyBlock);
        
        var value = 0f;
        while (value < 1f)
        {
            value += Time.deltaTime;
            propertyBlock.SetFloat("_Cutoff", value);
            enemyRenderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
        Destroy(gameObject);
    }
    
}
