using UnityEngine;
using UnityEngine.InputSystem;

public class JumpPlayerState : PlayerState, IPlayerState
{
    public JumpPlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }


    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamJump);
    }

    public void Update()
    {
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
