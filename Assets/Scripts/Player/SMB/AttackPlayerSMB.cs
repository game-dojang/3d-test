using UnityEngine;

public class AttackPlayerSMB : StateMachineBehaviour
{
    private PlayerController _playerController;
    
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController == null) _playerController = animator.GetComponent<PlayerController>();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerController.SetState(PlayerController.EPlayerState.Idle);
    }
}
