using UnityEngine;

public class HitEnemySMB : StateMachineBehaviour
{
    private EnemyController _enemyController;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemyController) _enemyController = animator.GetComponent<EnemyController>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController.SetState(EnemyController.EEnemyState.Idle);
    }
}
