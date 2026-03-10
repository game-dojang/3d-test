using UnityEngine;
using UnityEngine.AI;

public class ChaseEnemyState: EnemyState, ICharacterState
{
    public ChaseEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetBool(EnemyController.EnemyAniParamChase, true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamChase, false);
    }
}