
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackPlayerState: PlayerState, IPlayerState
{
    public AttackPlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamAttack);
        _playerInput.actions["Fire"].performed += AttackTrigger;
    }

    public void Update()
    {

    }

    public void Exit()
    {
        _playerInput.actions["Fire"].performed -= AttackTrigger;
    }

    private void AttackTrigger(InputAction.CallbackContext context)
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamAttack);
    }
}