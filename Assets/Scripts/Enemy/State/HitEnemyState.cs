using UnityEngine;
using UnityEngine.AI;

public class HitEnemyState: EnemyState, ICharacterState
{
    public HitEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetTrigger(EnemyController.EnemyAniParamHit);
    }

    public void Update() { }

    public void Exit() { }
}