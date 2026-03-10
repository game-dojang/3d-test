# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (v6000.3.10f1) third-person character controller project featuring the Ellen character from "3D Game Kit - Character Pack". Written in C# 9.0.

## Development Environment

- **Engine:** Unity 6.0.3 — open the project in Unity Hub
- **IDE:** Visual Studio, Rider, or VSCode (solution: `3d-test.slnx`)
- **Build/Run:** Play mode via Unity Editor (File → Build Settings for standalone builds)
- **No CLI build/test commands** — all building, running, and testing is done through the Unity Editor

## Architecture

### State Machine Pattern (core design)

The player controller uses a **state machine** with `IPlayerState` (Enter/Update/Exit) interface:

```
PlayerController (state machine hub, manages transitions + physics)
├── IdlePlayerState   — listens for Jump/Move inputs
├── MovePlayerState   — speed interpolation, walk/run blend
└── JumpPlayerState   — airborne physics, ground distance tracking
```

- States are stored in `Dictionary<EPlayerState, IPlayerState>` on `PlayerController`
- Transitions happen via `PlayerController.SetState(EPlayerState)`
- States receive `PlayerController`, `Animator`, and `PlayerInput` references at construction

### Movement & Physics

- Uses Unity's `CharacterController` component (not Rigidbody)
- Physics applied in `OnAnimatorMove()`: gravity accumulation + animator root motion blend
- Ground detection via raycasting in `CharacterUtility`

### Input System

- Uses Unity's **new InputSystem** (not legacy `Input` class)
- Actions defined in `Player.inputactions`: Move, Jump, Run, Look
- `PlayerInput` component routes callbacks to state instances

### Animation Integration

- `StateMachineBehaviour` subclasses (`JumpPlayerSMB`, `SpawnPlayerSMB`) trigger state transitions when animations end
- Animator parameters: `idle` (bool), `move` (bool), `jump` (trigger), `move_speed` (float 0-1), `ground_distance` (float)
- Parameter hashes cached as `static readonly int` fields on `PlayerController`

### Camera

- `CameraController` — spherical third-person camera following the character's head transform
- Azimuth/polar rotation with obstacle raycast avoidance

### Editor Tools

- `PlayerControllerEditor` — custom Inspector showing current state with color-coded labels

## Key Source Paths

- `Assets/Scripts/Player/PlayerController.cs` — state machine hub, jump mechanics, physics
- `Assets/Scripts/Player/State/` — concrete state implementations
- `Assets/Scripts/Player/SMB/` — animator StateMachineBehaviours
- `Assets/Scripts/Common/` — IPlayerState interface, Constants, CameraController, CharacterUtility
- `Assets/Editor/` — custom editor inspectors
- `Assets/Scenes/Map.unity` — main gameplay scene

## Conventions

- Comments and commit messages are in **Korean**
- `[SerializeField]` for inspector-exposed private fields
- `EllenPlayerController` extends `PlayerController` for character-specific overrides
