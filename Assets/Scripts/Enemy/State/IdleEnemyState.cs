using UnityEngine;
using UnityEngine.AI;

public class IdleEnemyState: EnemyState, ICharacterState
{
    private float _waitTime;
    
    public IdleEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _waitTime = 0f;
        _animator.SetBool(EnemyController.EnemyAniParamIdle, true);
    }

    public void Update()
    {
        // Enemy 주변에서 Player를 찾는 함수 호출
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        
        if (detectionTargetTransform)
        {
            // 주변에서 Player를 찾으면 추격으로 상태 전환
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
            _enemyController.SetState(EnemyController.EEnemyState.Chase);
        } 
        else if (_waitTime > _enemyController.PatrolWaitTime)
        {
            // 설정된 PatrolWaitTime을 초과하면 정찰 시도
            // 설정된 PatrolChange 값 보다 작은 랜덤 값이 나오면 정찰 시작
            var randomValue = Random.Range(0, 100);
            if (randomValue < _enemyController.PatrolChance)
            {
                // 정찰 위치를 찾기
                var patrolPosition = FindRandomPatrolPosition();
                
                // 정찰 위치가 현 위치에서 2unit 이상 벗어났을 경우 정찰 시작
                var realDistance = Vector3.Magnitude(patrolPosition - _enemyController.transform.position);
                var minimumDistance = _navMeshAgent.stoppingDistance + 2;
                if (realDistance > minimumDistance)
                {
                    _navMeshAgent.SetDestination(patrolPosition);
                    _enemyController.SetState(EnemyController.EEnemyState.Patrol);
                }
            }
            _waitTime = 0f;
        }
        _waitTime += Time.deltaTime;
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamIdle, false);
    }
    
    // 정찰 목적지를 찾는 함수
    private Vector3 FindRandomPatrolPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyController.PatrolDetectionDistance;
        randomDirection += _enemyController.transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, _enemyController.PatrolDetectionDistance,
                NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return _enemyController.transform.position;
        }
    }
}