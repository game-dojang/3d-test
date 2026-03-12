

using UnityEngine;
using UnityEngine.InputSystem;

public class HitPlayerState: PlayerState, ICharacterState
{
    public HitPlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerController.PlayerAniParamHit);
    }

    public void Update() { }

    public void Exit() { }
}