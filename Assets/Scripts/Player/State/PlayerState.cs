using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected Animator _animator;
    protected PlayerInput _playerInput;
    protected PlayerController _playerController;

    public PlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
    {
        _playerController = playerController;
        _animator = animator;
        _playerInput = playerInput;
    }
    
    // 캐릭터 회전
    protected void Rotate(float x, float z)
    {
        if (_playerInput.camera != null)
        {
            var cameraTransform = _playerInput.camera.transform;
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;
            
            cameraForward.y = 0;
            cameraRight.y = 0;

            var moveDirection = cameraForward * z + cameraRight * x;

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                _playerController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
    
    // 점프
    protected void Jump(InputAction.CallbackContext context)
    {
        _playerController.Jump();
        _playerController.SetState(PlayerController.EPlayerState.Jump);
    }
    
    // 공격
    protected void Attack(InputAction.CallbackContext context)
    {
        _playerController.SetState(PlayerController.EPlayerState.Attack);
    }
}
