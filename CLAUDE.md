# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (v6000.3.10f1) third-person action game project featuring the Ellen character from "3D Game Kit - Character Pack". Includes player combat, enemy AI (Chomper), and melee weapon system. Written in C# 9.0.

## Development Environment

- **Engine:** Unity 6.0.3 — open the project in Unity Hub
- **IDE:** Visual Studio, Rider, or VSCode (solution: `3d-test.slnx`)
- **Build/Run:** Play mode via Unity Editor (File → Build Settings for standalone builds)
- **No CLI build/test commands** — all building, running, and testing is done through the Unity Editor

## Architecture

### Unified State Machine Pattern (core design)

Both player and enemy controllers share the same pattern using the `ICharacterState` interface (Enter/Update/Exit):

**Player states** (`EPlayerState`): None, Idle, Move, Jump, Attack, Hit
```
PlayerController (state machine hub, manages transitions + physics)
├── IdlePlayerState    — listens for Jump/Move/Fire inputs
├── MovePlayerState    — speed interpolation, walk/run blend
├── JumpPlayerState    — airborne physics, ground distance tracking
├── AttackPlayerState  — triggers attack animation
└── HitPlayerState     — knockback from enemy hits
```

**Enemy states** (`EEnemyState`): None, Idle, Patrol, Chase, Attack, Hit, Dead
```
EnemyController (state machine hub, NavMesh + physics)
├── IdleEnemyState     — waits, random patrol chance
├── PatrolEnemyState   — navigates to random NavMesh points
├── ChaseEnemyState    — pursues player, sight angle detection
└── AttackEnemyState   — plays attack animation
```

- States stored in `Dictionary<EPlayerState/EEnemyState, ICharacterState>`
- Transitions via `SetState(enum)` on the controller
- `EllenPlayerController` extends `PlayerController` (loads Staff weapon from Resources)
- `ChomperEnemyController` extends `EnemyController` (implements weapon observer for hit delivery)

### Weapon System (Observer Pattern)

```
MeleeWeaponController : IWeaponObservable<GameObject>
├── SphereCast hit detection between animation frames
├── WeaponTriggerZone[] defines cast positions + radii
└── Notifies observers on swing completion

ChomperEnemyController : IWeaponObserver<GameObject>
└── OnNext() → calls PlayerController.SetHit() on hit target
```

Hit detection flow: Animation event → `StartTrigger()` → SphereCast each FixedUpdate → `EndTrigger()` → `Notify()` observers

### Movement & Physics

- **Player**: `CharacterController` component, gravity accumulation + animator root motion in `OnAnimatorMove()`
- **Enemy**: `NavMeshAgent` with `updatePosition = false`, manual position sync in `OnAnimatorMove()`
- Ground detection via raycasting in `CharacterUtility`

### Input System

- Unity's **new InputSystem** (not legacy `Input` class)
- Actions in `Player.inputactions`: Move, Jump, Run, Look, Fire, Cursor
- States subscribe/unsubscribe to input actions in their Enter/Exit methods

### Animation Integration

- `StateMachineBehaviour` subclasses trigger state transitions when animations end:
  - Player: `JumpPlayerSMB`, `SpawnPlayerSMB`, `AttackPlayerSMB`, `HitPlayerSMB` → SetState(Idle)
  - Enemy: `AttackEnemySMB` → SetState(Chase)
- Player animator params: `idle`, `move` (bool), `jump`, `attack`, `hit` (trigger), `move_speed`, `ground_distance`, `hit_x`, `hit_z` (float)
- Enemy animator params: `idle`, `patrol`, `chase` (bool), `attack`, `hit`, `dead` (trigger), `move_speed` (float)
- Parameter hashes cached as `static readonly int` fields on controllers

### Camera

- `CameraController` — spherical third-person camera following the character's head transform
- Azimuth/polar rotation with obstacle raycast avoidance

### Utilities

- `GameManager` — Singleton for cursor lock management
- `Singleton<T>` — generic MonoBehaviour singleton base class
- `PlayerControllerEditor` — custom Inspector showing current state with color-coded labels

## Key Source Paths

- `Assets/Scripts/Player/PlayerController.cs` — player state machine hub
- `Assets/Scripts/Player/State/` — player state implementations
- `Assets/Scripts/Player/SMB/` — player animator StateMachineBehaviours
- `Assets/Scripts/Enemy/EnemyController.cs` — enemy state machine hub
- `Assets/Scripts/Enemy/State/` — enemy state implementations
- `Assets/Scripts/Enemy/SMB/` — enemy animator StateMachineBehaviours
- `Assets/Scripts/Weapon/` — MeleeWeaponController + observer interfaces
- `Assets/Scripts/Common/` — ICharacterState, Constants, CameraController, CharacterUtility, GameManager
- `Assets/Editor/` — custom editor inspectors
- `Assets/Scenes/Map.unity` — main gameplay scene

## Conventions

- Comments and commit messages are in **Korean**
- `[SerializeField]` for inspector-exposed private fields
- Character-specific controllers extend base controllers (Ellen→Player, Chomper→Enemy)
