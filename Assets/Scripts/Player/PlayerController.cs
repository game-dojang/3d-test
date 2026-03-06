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
    
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    
    // 애니메이션 키
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");

    public enum EPlayerState
    {
        None, Idle, Move
    }
    
    // 현재 상태에 대한 정보
    public EPlayerState PlayerState { get; private set; }
    
    // 상태와 상태 객체를 담고있는 Dictionary
    private Dictionary<EPlayerState, IPlayerState> _playerStates;
    
    private void Awake()
    {
        // 컴포넌트 초기화
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        
        // 상태 객체 초기화
        var idlePlayerState = new IdlePlayerState(this, _animator, _playerInput);
        var movePlayerState = new MovePlayerState(this, _animator, _playerInput);

        _playerStates = new Dictionary<EPlayerState, IPlayerState>
        {
            { EPlayerState.Idle, idlePlayerState },
            { EPlayerState.Move, movePlayerState },
        };
        
        // 카메라 할당
        var playerCamera = Camera.main;
        if (playerCamera != null)
        {
            _playerInput.camera = playerCamera;
            playerCamera.GetComponent<CameraController>().SetTarget(headTransform, _playerInput);
        }
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
}
