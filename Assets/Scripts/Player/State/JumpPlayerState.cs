using UnityEngine;
using UnityEngine.InputSystem;

public class JumpPlayerState : PlayerState, ICharacterState
{
    public JumpPlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }


    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamJump);
    }

    public void Update()
    {
        // 점프 중에도 카메라의 방향으로 캐릭터 회전
        var moveVector = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (moveVector != Vector2.zero)
        {
            Rotate(moveVector.x, moveVector.y);
        }
        
        // Ground Distance 업데이트
        var playerPosition = _playerController.transform.position;
        var distance = CharacterUtility.GetDistanceToGround(playerPosition, Constants.GroundLayerMask,
            10f);
        
        _animator.SetFloat(PlayerController.PlayerAniParamGroundDistance, distance);

        Debug.DrawRay(playerPosition, Vector3.down * distance, Color.red);
    }

    public void Exit()
    {
    }
}
