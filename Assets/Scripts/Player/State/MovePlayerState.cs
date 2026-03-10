using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerState : PlayerState, ICharacterState
{
    private float _moveSpeed;
    
    public MovePlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        // Move 애니메이션 실행
        _animator.SetBool(PlayerController.PlayerAniParamMove, true);
        
        // _moveSpeed 초기화
        _moveSpeed = 0;
        
        // 액션 할당
        _playerInput.actions["Jump"].performed += Jump;
    }

    public void Update()
    {
        var moveVector = _playerInput.actions["Move"].ReadValue<Vector2>();
        
        if (moveVector != Vector2.zero)
        {
            Rotate(moveVector.x, moveVector.y);
        }
        else
        {
            _playerController.SetState(PlayerController.EPlayerState.Idle);
        }
        
        // 이동 스피드 설정
        var isRun = _playerInput.actions["Run"].IsPressed();
        if (isRun && _moveSpeed < 1f)
        {
            _moveSpeed += Time.deltaTime;
            _moveSpeed = Mathf.Clamp01(_moveSpeed);
        }
        else if (!isRun && _moveSpeed > 0)
        {
            _moveSpeed -= Time.deltaTime * _playerController.BreakForce;
            _moveSpeed = Mathf.Clamp01(_moveSpeed);
        }
        _animator.SetFloat(PlayerController.PlayerAniParamMoveSpeed, _moveSpeed);
    }

    public void Exit()
    {
        // 할당된 액션 해제
        _playerInput.actions["Jump"].performed -= Jump;
        
        // Move 애니메이션 중단
        _animator.SetBool(PlayerController.PlayerAniParamMove, false);
    }
}
