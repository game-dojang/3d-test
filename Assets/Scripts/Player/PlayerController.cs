using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    
    [Header("이동")]
    [SerializeField] [Range(0, 5)] private float breakForce = 1f;
    [SerializeField] private float jumpHeight = 2f;

    public float BreakForce => breakForce;
    
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    
    // 애니메이션 키
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamJump = Animator.StringToHash("jump");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamGroundDistance = Animator.StringToHash("ground_distance");
    public static readonly int PlayerAniParamAttack = Animator.StringToHash("attack");

    public enum EPlayerState
    {
        None, Idle, Move, Jump, Attack
    }
    
    // 물리
    private float _velocityY;
    
    // 현재 상태에 대한 정보
    public EPlayerState PlayerState { get; private set; }
    
    // 상태와 상태 객체를 담고있는 Dictionary
    private Dictionary<EPlayerState, ICharacterState> _playerStates;
    
    private void Awake()
    {
        // 컴포넌트 초기화
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        
        // 상태 객체 초기화
        var idlePlayerState = new IdlePlayerState(this, _animator, _playerInput);
        var movePlayerState = new MovePlayerState(this, _animator, _playerInput);
        var jumpPlayerState = new JumpPlayerState(this, _animator, _playerInput);
        var attackPlayerState = new AttackPlayerState(this, _animator, _playerInput);

        _playerStates = new Dictionary<EPlayerState, ICharacterState>
        {
            { EPlayerState.Idle, idlePlayerState },
            { EPlayerState.Move, movePlayerState },
            { EPlayerState.Jump, jumpPlayerState },
            { EPlayerState.Attack, attackPlayerState },
        };
        
        // 카메라 할당
        var playerCamera = Camera.main;
        if (playerCamera != null)
        {
            _playerInput.camera = playerCamera;
            playerCamera.GetComponent<CameraController>().SetTarget(headTransform, _playerInput);
        }
        
        // 커서 숨기기
        _playerInput.actions["Cursor"].performed += _ => GameManager.Instance.SetCursorLock();
    }

    private void Start()
    {
        PlayerState = EPlayerState.None;
    }

    private void Update()
    {
        if (PlayerState != EPlayerState.None) _playerStates[PlayerState].Update();
    }

    // 새로운 상태를 할당하는 함수
    public void SetState(EPlayerState state)
    {
        if (PlayerState == state) return;
        if (PlayerState != EPlayerState.None) _playerStates[PlayerState].Exit();
        PlayerState = state;
        if (PlayerState != EPlayerState.None) _playerStates[PlayerState].Enter();
    }
    
    // 점프
    public void Jump()
    {
        if (!_characterController.isGrounded) return;
        _velocityY = Mathf.Sqrt(jumpHeight * -2f * Constants.Gravity);
    }

    private void OnAnimatorMove()
    {
        Vector3 movePosition;
        if (_characterController.isGrounded)
        {
            movePosition = _animator.deltaPosition;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }
        
        _velocityY += Constants.Gravity * Time.deltaTime;
        movePosition.y = _velocityY * Time.deltaTime;
        _characterController.Move(movePosition);
    }
}