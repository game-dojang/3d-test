using UnityEngine;
using UnityEngine.InputSystem;

public class IdlePlayerState : PlayerState, ICharacterState
{
    public IdlePlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
     : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        // Idle 애니메이션 실행
        _animator.SetBool(PlayerController.PlayerAniParamIdle, true);
        
        // 액션 할당
        _playerInput.actions["Jump"].performed += Jump;
        _playerInput.actions["Fire"].performed += Attack;
    }

    public void Update()
    {
        if (_playerInput.actions["Move"].IsPressed())
        {
            _playerController.SetState(PlayerController.EPlayerState.Move);
        }
    }

    public void Exit()
    {
        // Idle 애니메이션 중단
        _animator.SetBool(PlayerController.PlayerAniParamIdle, false);
        
        // 할당된 액션 해제
        _playerInput.actions["Jump"].performed -= Jump;
        _playerInput.actions["Fire"].performed -= Attack;
    }
}
