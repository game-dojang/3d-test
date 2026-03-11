using UnityEngine;
using UnityEngine.AI;

public class AttackEnemyState: EnemyState, ICharacterState
{
    public AttackEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetTrigger(EnemyController.EnemyAniParamAttack);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}