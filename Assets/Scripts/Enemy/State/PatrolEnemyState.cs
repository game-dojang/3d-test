using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyState: EnemyState, ICharacterState
{
    public PatrolEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetBool(EnemyController.EnemyAniParamPatrol, true);
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
        else if (!_navMeshAgent.pathPending &&
              _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            // Patrol 상태에서 목적지에 도착했을 경우, Idle로 전환
            _enemyController.SetState(EnemyController.EEnemyState.Idle);
        }
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamPatrol, false);
    }
}