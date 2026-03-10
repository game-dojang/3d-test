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
        if (_waitTime > _enemyController.PatrolWaitTime)
        {
            var randomValue = Random.Range(0, 100);
            if (randomValue < _enemyController.PatrolChance)
            {
                // 정찰 시작
                var patrolPosition = FindRandomPatrolPosition();
            }
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