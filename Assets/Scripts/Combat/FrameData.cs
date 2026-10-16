using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single attack's frame data (SF6 style)
/// </summary>
[System.Serializable]
public class AttackData
{
    public string moveName;
    public AttackType type;
    
    [Header("Frame Data")]
    public int startupFrames;      // Frames before active
    public int activeFrames;       // Frames hitbox is active
    public int recoveryFrames;     // Frames after active before neutral
    public int hitStunFrames;      // Frames opponent is stunned on hit
    public int blockStunFrames;    // Frames opponent is stuck in block
    
    [Header("Damage & Meter")]
    public int damage;
    public int chipDamage;
    public int driveMeterGain;     // Meter gained on hit/block
    public int superMeterGain;
    
    [Header("Knockback & Launch")]
    public Vector2 knockback;      // X = horizontal, Y = vertical
    public float launchAngle;      // Degrees (0 = horizontal, 90 = straight up)
    public bool launches;          // Launches for juggle
    public bool wallBounce;        // Bounces off wall
    public bool groundBounce;      // Bounces off ground
    public bool hardKnockdown;     // Hard knockdown (no quick rise)
    
    [Header("Properties")]
    public bool isProjectile;
    public bool isOverhead;
    public bool isLow;
    public bool isThrow;
    public bool isInvincible;      // Frame 1 invincibility
    public int invincibleFrames;   // Number of invincible frames
    public bool armor;             // Armor frames
    public int armorHits;          // Hits of armor
    public bool crushCounter;      // Crush Counter property
    
    [Header("Cancel Windows")]
    public bool specialCancellable;
    public bool superCancellable;
    public bool driveRushCancellable; // SF6 Drive Rush cancel
    public int cancelStartFrame;   // Earliest frame to cancel
    public int cancelEndFrame;     // Latest frame to cancel
    
    [Header("Hitbox")]
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;
    public Vector2 hurtboxOffset;
    public Vector2 hurtboxSize;
    
    [Header("Visual/Audio")]
    public string hitEffect;
    public string blockEffect;
    public float hitPauseFrames;   // Hitstop frames
    public float screenShake;
    
    public int TotalFrames => startupFrames + activeFrames + recoveryFrames;
    public int AdvantageOnHit => hitStunFrames - recoveryFrames;
    public int AdvantageOnBlock => blockStunFrames - recoveryFrames;
}

public enum AttackType
{
    Normal,
    Special,
    Super,
    Throw,
    DriveImpact,      // SF6 Drive Impact (armored attack)
    DriveRush,        // SF6 Drive Rush
    DriveReversal,    // SF6 Drive Reversal
    Parry,            // SF6 Parry
    Unique            // Character-specific
}

/// <summary>
/// Character state machine states
/// </summary>
public enum CharacterState
{
    Idle,
    WalkForward,
    WalkBackward,
    Crouch,
    JumpStartup,
    JumpRising,
    JumpPeak,
    JumpFalling,
    JumpLanding,
    AttackStartup,
    AttackActive,
    AttackRecovery,
    HitStun,
    BlockStun,
    Knockdown,
    GettingUp,
    ThrowStartup,
    ThrowActive,
    ThrowBroken,
    Thrown,
    Stunned,           // Dizzy
    SuperFreeze,       // Super animation freeze
    DriveRush,         // SF6 Drive Rush state
    Parry,             // SF6 Parry state
    DriveImpact,       // SF6 Drive Impact state
    WallSplat,
    GroundBounce,
    Custom
}

/// <summary>
/// Hitbox data for collision detection
/// </summary>
[System.Serializable]
public class Hitbox
{
    public HitboxType type;
    public Vector2 offset;
    public Vector2 size;
    public int damage;
    public int hitStun;
    public int blockStun;
    public Vector2 knockback;
    public bool launches;
    public bool isActive;
    public int activeFrameStart;
    public int activeFrameEnd;
    public AttackData attackData; // Reference to full attack data
}

public enum HitboxType
{
    Hurtbox,      // Can be hit
    Hitbox,       // Hits opponent
    GrabBox,      // Throw range
    ThrowHurtbox, // Can be thrown
    Pushbox,      // Collision with other pushboxes
    Clashbox      // For clash detection
}

/// <summary>
/// Result of a hit detection
/// </summary>
public struct HitResult
{
    public bool hit;
    public bool counterHit;
    public bool crushCounter;
    public AttackData attack;
    public Hitbox hitbox;
    public Hitbox hurtbox;
    public Vector2 contactPoint;
    public int damageDealt;
}