# SF6 Clone - 2D Fighting Game in Unity

A Street Fighter 6-inspired 2D fighting game built in Unity with 6 unique characters, SF6 mechanics (Drive System, Parry, Drive Rush, Drive Impact), and procedural graphics.

## Features

### Core Mechanics (SF6-Accurate)
- **4-Button Layout**: Light, Medium, Heavy, Special (Drive/Unique)
- **Drive Gauge System**: 6 bars for Drive Rush, Drive Impact, Drive Reversal, Parry
- **Super Meter**: 3 bars for Super Arts
- **Frame-Perfect Input Buffering**: 15-frame motion windows
- **Counter Hit / Crush Counter System**
- **Hitstop / Hitpause** on impact
- **Combo System** with damage scaling (10% per hit, 10% minimum)
- **Throw Tech / Throw Invulnerability**

### SF6 Drive Mechanics
- **Drive Rush** (2 bars): Fast forward dash, cancellable from normals
- **Drive Impact** (2 bars): Armored attack, wall splat on counter hit
- **Drive Reversal** (2 bars): Invincible reversal during blockstun
- **Parry** (Free): 8-frame window, builds Drive Gauge on success
- **Burnout State**: No Drive Gauge regen when depleted

### 6 Unique Characters

| Character | Archetype | Key Mechanics |
|-----------|-----------|---------------|
| **Ryu** | Shoto | Hadoken (QCF+P), Shoryuken (DP+P), Tatsumaki (QCB+K) |
| **Chun-Li** | Rushdown/Pokes | Lightning Legs (Kick mash), Spinning Bird Kick (Charge D,U+K), Kikoken (QCF+P) |
| **Zangief** | Grappler | SPD (360+P), Borscht Dynamite (360+K), Double Lariat (PPP/KKK), Armor |
| **Guile** | Charge Zoner | Sonic Boom (Charge B,F+P), Flash Kick (Charge D,U+K), Sonic Cross (QCF+P) |
| **Luke** | Modern Rushdown | Sand Blaster (QCF+P), Flash Knuckle (QCB+P), Rising Rocket (DP+K), Sand Stocks |
| **JP** | Tricky Zoner | Tristis/Amor/Invidia (Portal projectiles), Teleports, Trap-based zoning |

### Technical Features
- **Procedural Graphics**: All sprites generated at runtime (no external assets needed)
- **Training Mode**: Frame data display, hitbox visualization, input recording/playback
- **Rollback-Ready Architecture**: Deterministic fixed-timestep simulation
- **Modular Character System**: Easy to add new characters
- **State Machine Architecture**: Clean separation of states

## Play in the Browser (no Unity needed)
Open `web/index.html` in any modern browser (double-click it). It's a single self-contained file:
VS CPU, VS Player 2 (same keyboard or two gamepads), and Training mode, with all 6 characters.
Controls are listed in-game under **CONTROLS** and match the tables below (Special `;` = S).
Training: `H` hitboxes, `F` frame data, `B` dummy mode, `M` infinite meter, `R` reset, `Ctrl` record dummy, `Shift` playback.

## Quick Start (Unity)

### Requirements
- Unity 2022.3 LTS or 6000.0+
- .NET Standard 2.1 / .NET 6+

### Setup
1. Open the `SF6Clone` folder as a Unity project
2. Open `Assets/Scenes/CharacterSelect.unity` (create if needed)
3. Press Play

### Controls

#### Player 1 (Keyboard)
| Action | Key |
|--------|-----|
| Move | WASD |
| Light | J |
| Medium | K |
| Heavy | L |
| Special | ; |
| Throw | J + K |
| Parry | K + L |
| Drive Impact | L + ; |
| Drive Rush | 6 (Forward) + ; |

#### Player 2 (Numpad)
| Action | Key |
|--------|-----|
| Move | Arrow Keys |
| Light | Num 4 |
| Medium | Num 5 |
| Heavy | Num 6 |
| Special | Num + |
| Throw | Num 4 + Num 5 |
| Parry | Num 5 + Num 6 |
| Drive Impact | Num 6 + Num + |
| Drive Rush | 6 (Forward) + Num + |

#### Training Mode
- **Tab**: Toggle training menu
- **H**: Toggle hitbox display
- **F**: Toggle frame data display
- **R**: Reset positions
- **Left Ctrl**: Record inputs (hold)
- **Left Shift**: Playback recording
- **Period (.)**: Step frame (when paused)

## Architecture

### Core Systems
```
Scripts/
├── Core/
│   ├── GameManager.cs          # Round/Match management
│   ├── GameEntryPoint.cs       # Scene flow
│   ├── BattleSceneSetup.cs     # Runtime scene construction
│   └── CharacterSelectManager.cs
├── Input/
│   ├── InputManager.cs         # Input buffering, motion detection
│   └── InputDefinitions.cs     # Button/Direction enums
├── Combat/
│   ├── FrameData.cs            # AttackData, Hitbox, HitResult
│   └── HitboxSystem.cs         # Collision detection
├── Characters/
│   ├── CharacterBase.cs        # Base class with state machine
│   ├── CharacterRyu.cs
│   ├── CharacterChunLi.cs
│   ├── CharacterZangief.cs
│   ├── CharacterGuile.cs
│   ├── CharacterLuke.cs
│   └── CharacterJP.cs
└── UI/
    ├── ProceduralSpriteGenerator.cs
    └── TrainingMode.cs
```

### Character State Machine
```
Idle ↔ WalkForward/WalkBackward
  ↓
Crouch
  ↓
JumpStartup → JumpRising → JumpPeak → JumpFalling → JumpLanding → Idle
  ↓
AttackStartup → AttackActive → AttackRecovery → Idle
  ↓ (on hit)
HitStun / BlockStun → Idle
  ↓ (on knockdown)
Knockdown → GettingUp → Idle
  ↓ (throw)
ThrowStartup → ThrowActive → Idle
  
SF6 States:
DriveRush, Parry, DriveImpact, SuperFreeze
```

### Adding a New Character
1. Create new script inheriting from `CharacterBase`
2. Implement `InitializeMoveset()` with all `AttackData`
3. Implement `GetPendingAttack()` for normal attack selection
4. Implement `DoLight/Medium/HeavyAttack()` and `DoSpecialMove()`
5. Add to `CharacterSelectManager.allCharacters`
6. Create prefab (or use procedural generation)

## Frame Data Reference (SF6 Standards)

| Move Type | Startup | Active | Recovery | Notes |
|-----------|---------|--------|----------|-------|
| Light | 3-4f | 2-3f | 5-7f | +1 to +3 on block, chainable |
| Medium | 5-7f | 3-4f | 8-12f | +1 to +4 on block, special cancel |
| Heavy | 8-12f | 4-5f | 14-20f | -2 to -8 on block, crush counter |
| Special | 8-20f | 2-10f | 15-30f | Varies by move |
| Super | 4-12f | 20-40f | 30-50f | Invincible startup |

## Drive Gauge Costs
| Action | Cost | Frames |
|--------|------|--------|
| Drive Rush | 2 bars (20,000) | 30f duration |
| Drive Impact | 2 bars (20,000) | 20f armor, 60f total |
| Drive Reversal | 2 bars (20,000) | 10f invincible |
| Parry | Free | 8f active |

## Project Structure
```
SF6Clone/
├── Assets/
│   ├── Scripts/          # All C# scripts
│   ├── Scenes/           # Unity scenes (create via BattleSceneSetup)
│   ├── Resources/        # Runtime resources
│   └── Prefabs/          # Character prefabs (optional)
├── ProjectSettings/      # Unity settings
└── README.md
```

## Extending the Game

### New Move Properties
Add to `AttackData` in `FrameData.cs`:
```csharp
public bool wallBounce;
public bool groundBounce;
public bool hardKnockdown;
public int armorHits;
public float launchAngle;
```

### New Character States
1. Add to `CharacterState` enum
2. Add enter/update/exit actions in `InitializeStateMachine()`
3. Handle transitions in `UpdateIdle()` or relevant state

### Online Multiplayer (Rollback)
The deterministic fixed-timestep architecture supports rollback netcode:
- All gameplay in `FixedUpdate`
- Inputs captured in `InputManager`
- State is fully serializable
- Add `RollbackManager` to snapshot/restore state

## Credits
- Inspired by Street Fighter 6 (Capcom)
- Built with Unity
- Procedural graphics system

## License
MIT License - Feel free to use for learning or commercial projects.