using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base character controller with SF6 mechanics
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Identity")]
    public string characterName;
    public int playerIndex; // 0 or 1
    public bool facingRight = true;
    
    [Header("Stats (SF6 Standard)")]
    public int maxHealth = 10000;
    public int currentHealth;
    public int maxDriveGauge = 60000; // 6 bars * 10000
    public int currentDriveGauge;
    public int maxSuperMeter = 30000; // 3 bars
    public int currentSuperMeter;
    
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 18f;
    public float gravity = 40f;
    public float airDrag = 0.95f;
    public float pushbackResistance = 0.5f;
    
    [Header("Drive System (SF6)")]
    public int driveRushCost = 20000;      // 2 bars
    public int driveImpactCost = 20000;    // 2 bars
    public int driveReversalCost = 20000;  // 2 bars
    public int parryCost = 0;              // Free but has recovery
    public float driveRushSpeed = 15f;
    public float driveRushDuration = 0.5f; // seconds
    
    [Header("State")]
    public CharacterState currentState = CharacterState.Idle;
    public CharacterState previousState = CharacterState.Idle;
    public int stateFrame = 0;
    public bool isGrounded = true;
    public bool isBlocking = false;
    public bool blockLow = false;
    public bool justParried = false;
    public int parryWindowFrames = 8; // Parry active frames
    
    [Header("Combat")]
    public AttackData currentAttack;
    public int attackFrame = 0;
    public List<Hitbox> activeHitboxes = new List<Hitbox>();
    public List<Hitbox> hurtboxes = new List<Hitbox>();
    public int hitStunRemaining = 0;
    public int blockStunRemaining = 0;
    public int throwInvulnFrames = 0;
    public int hitPauseFrames = 0;
    public bool counterHit = false;
    public bool crushCounter = false;
    public int comboCount = 0;
    public float comboDamageScaling = 1f;
    
    [Header("Knockdown")]
    public bool isKnockedDown = false;
    public float knockdownTime = 0f;
    public bool hardKnockdown = false;
    public Vector2 knockdownVelocity;
    
    [Header("References")]
    public Rigidbody2D rb;
    public BoxCollider2D pushbox;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Transform hitboxContainer;
    public Transform graphicsContainer;
    
    // Components
    protected InputManager inputManager;
    protected CharacterBase opponent;
    protected GameManager gameManager;
    
    // State machine
    protected Dictionary<CharacterState, System.Action> stateEnters = new Dictionary<CharacterState, System.Action>();
    protected Dictionary<CharacterState, System.Action> stateUpdates = new Dictionary<CharacterState, System.Action>();
    protected Dictionary<CharacterState, System.Action> stateExits = new Dictionary<CharacterState, System.Action>();
    
    // Input buffer
    protected Queue<BufferedCommand> commandBuffer = new Queue<BufferedCommand>();
    protected const int COMMAND_BUFFER_SIZE = 10;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pushbox = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        hitboxContainer = transform.Find("Hitboxes");
        graphicsContainer = transform.Find("Graphics");
        
        inputManager = InputManager.Instance;
        gameManager = GameManager.Instance;
        
        currentHealth = maxHealth;
        currentDriveGauge = maxDriveGauge;
        currentSuperMeter = 0;
        
        InitializeStateMachine();
        InitializeMoveset();
        GenerateProceduralSprites();
    }
    
    protected virtual void Start()
    {
        // Find opponent
        var characters = FindObjectsOfType<CharacterBase>();
        foreach (var c in characters)
        {
            if (c != this) opponent = c;
        }
    }
    
    protected virtual void FixedUpdate()
    {
        if (gameManager == null || gameManager.isPaused) return;
        
        // Handle hitpause
        if (hitPauseFrames > 0)
        {
            hitPauseFrames--;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        stateFrame++;
        UpdateFacing();
        StateMachineUpdate();
        PhysicsUpdate();
        UpdateMeters();
        ProcessCommandBuffer();
    }
    
    private void UpdateFacing()
    {
        if (opponent == null) return;
        facingRight = transform.position.x < opponent.transform.position.x;
        Vector3 scale = graphicsContainer.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        graphicsContainer.localScale = scale;
    }
    
    private void StateMachineUpdate()
    {
        if (stateUpdates.ContainsKey(currentState))
        {
            stateUpdates[currentState]();
        }
    }
    
    private void PhysicsUpdate()
    {
        if (!isGrounded)
        {
            rb.linearVelocity += Vector2.down * gravity * Time.fixedDeltaTime;
            rb.linearVelocity *= airDrag;
        }
        
        // Apply knockdown velocity
        if (isKnockedDown)
        {
            rb.linearVelocity = knockdownVelocity;
            knockdownVelocity.y -= gravity * Time.fixedDeltaTime;
            knockdownTime -= Time.fixedDeltaTime;
            
            if (knockdownTime <= 0 && isGrounded)
            {
                ExitKnockdown();
            }
        }
    }
    
    private void UpdateMeters()
    {
        // Drive gauge passive regen (SF6: ~1 bar per 8 seconds when not burned out)
        if (currentDriveGauge < maxDriveGauge && !IsBurnedOut())
        {
            currentDriveGauge = Mathf.Min(maxDriveGauge, currentDriveGauge + 125); // ~1 bar/8s at 60fps
        }
        
        // Super meter gain on hit/block handled in OnHit/OnBlock
    }
    
    private void ProcessCommandBuffer()
    {
        while (commandBuffer.Count > 0)
        {
            var cmd = commandBuffer.Peek();
            if (CanExecuteCommand(cmd))
            {
                commandBuffer.Dequeue();
                ExecuteBufferedCommand(cmd);
            }
            else break;
        }
    }
    
    /// <summary>
    /// Initialize state machine with all SF6 states
    /// </summary>
    protected virtual void InitializeStateMachine()
    {
        // Idle
        stateEnters[CharacterState.Idle] = () => { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); };
        stateUpdates[CharacterState.Idle] = UpdateIdle;
        
        // Walk
        stateEnters[CharacterState.WalkForward] = () => { };
        stateUpdates[CharacterState.WalkForward] = () => MoveHorizontal(walkSpeed);
        stateEnters[CharacterState.WalkBackward] = () => { };
        stateUpdates[CharacterState.WalkBackward] = () => MoveHorizontal(-walkSpeed);
        
        // Crouch
        stateEnters[CharacterState.Crouch] = () => { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); };
        stateUpdates[CharacterState.Crouch] = UpdateCrouch;
        
        // Jump
        stateEnters[CharacterState.JumpStartup] = () => { };
        stateUpdates[CharacterState.JumpStartup] = UpdateJumpStartup;
        stateEnters[CharacterState.JumpRising] = () => { rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); };
        stateUpdates[CharacterState.JumpRising] = UpdateJumpRising;
        stateUpdates[CharacterState.JumpPeak] = UpdateJumpPeak;
        stateUpdates[CharacterState.JumpFalling] = UpdateJumpFalling;
        stateEnters[CharacterState.JumpLanding] = () => { isGrounded = true; rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); };
        stateUpdates[CharacterState.JumpLanding] = UpdateJumpLanding;
        
        // Attack
        stateEnters[CharacterState.AttackStartup] = OnAttackStartup;
        stateUpdates[CharacterState.AttackStartup] = UpdateAttackStartup;
        stateEnters[CharacterState.AttackActive] = OnAttackActive;
        stateUpdates[CharacterState.AttackActive] = UpdateAttackActive;
        stateEnters[CharacterState.AttackRecovery] = OnAttackRecovery;
        stateUpdates[CharacterState.AttackRecovery] = UpdateAttackRecovery;
        
        // Hit/Block stun
        stateEnters[CharacterState.HitStun] = () => { rb.linearVelocity = Vector2.zero; };
        stateUpdates[CharacterState.HitStun] = UpdateHitStun;
        stateEnters[CharacterState.BlockStun] = () => { rb.linearVelocity = Vector2.zero; isBlocking = true; };
        stateUpdates[CharacterState.BlockStun] = UpdateBlockStun;
        
        // Knockdown
        stateEnters[CharacterState.Knockdown] = OnKnockdown;
        stateUpdates[CharacterState.Knockdown] = UpdateKnockdown;
        stateEnters[CharacterState.GettingUp] = OnGettingUp;
        stateUpdates[CharacterState.GettingUp] = UpdateGettingUp;
        
        // Throw
        stateEnters[CharacterState.ThrowStartup] = OnThrowStartup;
        stateUpdates[CharacterState.ThrowStartup] = UpdateThrowStartup;
        stateEnters[CharacterState.ThrowActive] = OnThrowActive;
        stateUpdates[CharacterState.ThrowActive] = UpdateThrowActive;
        
        // SF6 Specific
        stateEnters[CharacterState.DriveRush] = OnDriveRushStart;
        stateUpdates[CharacterState.DriveRush] = UpdateDriveRush;
        stateEnters[CharacterState.Parry] = OnParryStart;
        stateUpdates[CharacterState.Parry] = UpdateParry;
        stateEnters[CharacterState.DriveImpact] = OnDriveImpactStart;
        stateUpdates[CharacterState.DriveImpact] = UpdateDriveImpact;
        stateEnters[CharacterState.SuperFreeze] = OnSuperFreeze;
        stateUpdates[CharacterState.SuperFreeze] = UpdateSuperFreeze;
    }
    
    /// <summary>
    /// Abstract - each character defines their moveset
    /// </summary>
    protected abstract void InitializeMoveset();
    
    /// <summary>
    /// Generate procedural sprites for visualization
    /// </summary>
    protected virtual void GenerateProceduralSprites()
    {
        // Create simple colored rectangles for each state
        // This will be overridden by characters for unique colors
    }
    
    #region State Updates
    
    protected virtual void UpdateIdle()
    {
        if (!isGrounded) { ChangeState(CharacterState.JumpFalling); return; }
        
        // Check for stun
        if (hitStunRemaining > 0) { ChangeState(CharacterState.HitStun); return; }
        if (blockStunRemaining > 0) { ChangeState(CharacterState.BlockStun); return; }
        
        // Input handling
        Direction dir = inputManager.GetDirection(playerIndex);
        bool down = (dir == Direction.Down || dir == Direction.DownBack || dir == Direction.DownForward);
        
        if (down) { ChangeState(CharacterState.Crouch); return; }
        
        bool forward = facingRight ? dir == Direction.Forward : dir == Direction.Back;
        bool backward = facingRight ? dir == Direction.Back : dir == Direction.Forward;
        
        if (forward) { ChangeState(CharacterState.WalkForward); return; }
        if (backward) { ChangeState(CharacterState.WalkBackward); return; }
        
        // Jump
        if (dir == Direction.Up || dir == Direction.UpForward || dir == Direction.UpBack)
        {
            ChangeState(CharacterState.JumpStartup);
            return;
        }
        
        // Attacks
        CheckAttackInputs();
        
        // Specials
        CheckSpecialInputs();
        
        // Drive System
        CheckDriveInputs();
        
        // Throw
        if (inputManager.GetButtonDown(playerIndex, Button.Light) && inputManager.GetButtonDown(playerIndex, Button.Medium))
        {
            TryThrow();
        }
    }
    
    protected virtual void UpdateCrouch()
    {
        if (!isGrounded) { ChangeState(CharacterState.JumpFalling); return; }
        
        Direction dir = inputManager.GetDirection(playerIndex);
        bool down = (dir == Direction.Down || dir == Direction.DownBack || dir == Direction.DownForward);
        
        if (!down) { ChangeState(CharacterState.Idle); return; }
        
        // Crouch attacks
        CheckAttackInputs();
        CheckSpecialInputs();
        CheckDriveInputs();
    }
    
    protected virtual void UpdateJumpStartup()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool forward = facingRight ? (dir == Direction.UpForward) : (dir == Direction.UpBack);
        bool backward = facingRight ? (dir == Direction.UpBack) : (dir == Direction.UpForward);
        
        if (forward) rb.linearVelocity = new Vector2(walkSpeed * 0.8f, rb.linearVelocity.y);
        else if (backward) rb.linearVelocity = new Vector2(-walkSpeed * 0.8f, rb.linearVelocity.y);
        
        if (stateFrame >= 4) // 4 frame jump startup
        {
            ChangeState(CharacterState.JumpRising);
        }
    }
    
    protected virtual void UpdateJumpRising()
    {
        if (rb.linearVelocity.y <= 0) ChangeState(CharacterState.JumpPeak);
        
        // Air attacks
        CheckAirAttackInputs();
        CheckAirSpecialInputs();
    }
    
    protected virtual void UpdateJumpPeak()
    {
        if (rb.linearVelocity.y < -0.1f) ChangeState(CharacterState.JumpFalling);
        CheckAirAttackInputs();
    }
    
    protected virtual void UpdateJumpFalling()
    {
        if (isGrounded)
        {
            ChangeState(CharacterState.JumpLanding);
        }
        CheckAirAttackInputs();
    }
    
    protected virtual void UpdateJumpLanding()
    {
        if (stateFrame >= 4) // 4 frame landing recovery
        {
            ChangeState(CharacterState.Idle);
        }
    }
    
    protected virtual void OnAttackStartup()
    {
        attackFrame = 0;
        currentAttack = GetPendingAttack();
        if (currentAttack != null && currentAttack.isInvincible)
        {
            // Invincibility frames handled in hurtbox logic
        }
    }
    
    protected virtual void UpdateAttackStartup()
    {
        attackFrame++;
        if (attackFrame >= currentAttack.startupFrames)
        {
            ChangeState(CharacterState.AttackActive);
        }
    }
    
    protected virtual void OnAttackActive()
    {
        attackFrame = currentAttack.startupFrames;
        ActivateHitboxes(currentAttack);
    }
    
    protected virtual void UpdateAttackActive()
    {
        attackFrame++;
        if (attackFrame >= currentAttack.startupFrames + currentAttack.activeFrames)
        {
            DeactivateHitboxes();
            ChangeState(CharacterState.AttackRecovery);
        }
        else
        {
            // Check for cancels
            CheckCancelInputs();
        }
    }
    
    protected virtual void OnAttackRecovery()
    {
        attackFrame = currentAttack.startupFrames + currentAttack.activeFrames;
    }
    
    protected virtual void UpdateAttackRecovery()
    {
        attackFrame++;
        if (attackFrame >= currentAttack.TotalFrames)
        {
            ChangeState(CharacterState.Idle);
        }
        else
        {
            // Late cancels
            CheckCancelInputs();
        }
    }
    
    protected virtual void UpdateHitStun()
    {
        hitStunRemaining--;
        if (hitStunRemaining <= 0)
        {
            if (isKnockedDown) ChangeState(CharacterState.Knockdown);
            else ChangeState(CharacterState.Idle);
        }
    }
    
    protected virtual void UpdateBlockStun()
    {
        blockStunRemaining--;
        if (blockStunRemaining <= 0)
        {
            isBlocking = false;
            ChangeState(CharacterState.Idle);
        }
    }
    
    protected virtual void OnKnockdown()
    {
        isKnockedDown = true;
        knockdownTime = hardKnockdown ? 2f : 1f;
        comboCount = 0;
        comboDamageScaling = 1f;
    }
    
    protected virtual void UpdateKnockdown()
    {
        // Can quick rise if not hard knockdown
        if (!hardKnockdown && inputManager.GetButtonDown(playerIndex, Button.Special))
        {
            ChangeState(CharacterState.GettingUp);
        }
    }
    
    protected virtual void OnGettingUp()
    {
        // Invincible during getup
    }
    
    protected virtual void UpdateGettingUp()
    {
        if (stateFrame >= 30) ChangeState(CharacterState.Idle);
    }
    
    protected virtual void OnThrowStartup() { }
    protected virtual void UpdateThrowStartup() 
    {
        if (stateFrame >= 5) ChangeState(CharacterState.ThrowActive);
    }
    protected virtual void OnThrowActive() { }
    protected virtual void UpdateThrowActive() 
    {
        if (stateFrame >= 10) ChangeState(CharacterState.Idle);
    }
    
    // SF6 Drive System
    protected virtual void OnDriveRushStart()
    {
        currentDriveGauge -= driveRushCost;
        rb.linearVelocity = new Vector2(facingRight ? driveRushSpeed : -driveRushSpeed, rb.linearVelocity.y);
    }
    
    protected virtual void UpdateDriveRush()
    {
        rb.linearVelocity = new Vector2(facingRight ? driveRushSpeed : -driveRushSpeed, rb.linearVelocity.y);
        if (stateFrame >= driveRushDuration * 60)
        {
            ChangeState(CharacterState.Idle);
        }
    }
    
    protected virtual void OnParryStart()
    {
        justParried = false;
        // Parry has no startup, active immediately
    }
    
    protected virtual void UpdateParry()
    {
        if (stateFrame >= parryWindowFrames)
        {
            ChangeState(CharacterState.Idle);
        }
    }
    
    protected virtual void OnDriveImpactStart()
    {
        currentDriveGauge -= driveImpactCost;
        // Armor frames start immediately
    }
    
    protected virtual void UpdateDriveImpact()
    {
        // Armor active for first 20 frames
        if (stateFrame >= 60) ChangeState(CharacterState.Idle);
    }
    
    protected virtual void OnSuperFreeze()
    {
        // Freeze opponent
        if (opponent) opponent.Freeze(true);
    }
    
    protected virtual void UpdateSuperFreeze()
    {
        if (stateFrame >= 60) // 1 second freeze
        {
            if (opponent) opponent.Freeze(false);
            ChangeState(CharacterState.Idle);
        }
    }
    
    #endregion
    
    #region Movement & State Management
    
    public void ChangeState(CharacterState newState)
    {
        if (stateExits.ContainsKey(currentState))
            stateExits[currentState]();
        
        previousState = currentState;
        currentState = newState;
        stateFrame = 0;
        
        if (stateEnters.ContainsKey(newState))
            stateEnters[newState]();
    }
    
    protected void MoveHorizontal(float speed)
    {
        if (!isGrounded) speed *= 0.5f;
        float dir = facingRight ? speed : -speed;
        rb.linearVelocity = new Vector2(dir, rb.linearVelocity.y);
    }
    
    protected void MoveHorizontalRaw(float speed)
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }
    
    #endregion
    
    #region Attack System
    
    protected abstract AttackData GetPendingAttack();
    
    protected virtual void CheckAttackInputs()
    {
        if (inputManager.GetButtonDown(playerIndex, Button.Light)) BufferCommand(CommandType.Light);
        if (inputManager.GetButtonDown(playerIndex, Button.Medium)) BufferCommand(CommandType.Medium);
        if (inputManager.GetButtonDown(playerIndex, Button.Heavy)) BufferCommand(CommandType.Heavy);
    }
    
    protected virtual void CheckAirAttackInputs()
    {
        CheckAttackInputs();
    }
    
    protected virtual void CheckSpecialInputs()
    {
        if (inputManager.GetButtonDown(playerIndex, Button.Special))
        {
            string motion = inputManager.CheckAnySpecialMotion(playerIndex, facingRight, Button.Special);
            if (motion != null)
            {
                BufferCommand(CommandType.Special, motion);
            }
        }
    }
    
    protected virtual void CheckAirSpecialInputs()
    {
        CheckSpecialInputs();
    }
    
    protected virtual void CheckDriveInputs()
    {
        // Drive Rush: 66 + Special (forward forward + special)
        // Drive Impact: Heavy + Special
        // Drive Reversal: Forward + Heavy + Special during blockstun
        // Parry: Medium + Heavy
        
        if (inputManager.GetButtonDown(playerIndex, Button.Medium) && inputManager.GetButtonDown(playerIndex, Button.Heavy))
        {
            if (CanSpendDrive(0)) // Parry is free
                TryParry();
        }
        
        if (inputManager.GetButtonDown(playerIndex, Button.Heavy) && inputManager.GetButtonDown(playerIndex, Button.Special))
        {
            if (CanSpendDrive(driveImpactCost))
                TryDriveImpact();
        }
        
        // Drive Rush detection (double tap forward)
        // Simplified: Forward + Special while grounded
        Direction dir = inputManager.GetDirection(playerIndex);
        bool forward = facingRight ? dir == Direction.Forward : dir == Direction.Back;
        if (forward && inputManager.GetButtonDown(playerIndex, Button.Special) && isGrounded)
        {
            if (CanSpendDrive(driveRushCost))
                TryDriveRush();
        }
    }
    
    protected virtual void CheckCancelInputs()
    {
        // Special cancel
        if (currentAttack.specialCancellable && attackFrame >= currentAttack.cancelStartFrame && attackFrame <= currentAttack.cancelEndFrame)
        {
            CheckSpecialInputs();
        }
        
        // Drive Rush cancel
        if (currentAttack.driveRushCancellable && attackFrame >= currentAttack.cancelStartFrame && attackFrame <= currentAttack.cancelEndFrame)
        {
            Direction dir = inputManager.GetDirection(playerIndex);
            bool forward = facingRight ? dir == Direction.Forward : dir == Direction.Back;
            if (forward && inputManager.GetButtonDown(playerIndex, Button.Special))
            {
                if (CanSpendDrive(driveRushCost))
                {
                    BufferCommand(CommandType.DriveRushCancel);
                }
            }
        }
    }
    
    protected void ActivateHitboxes(AttackData attack)
    {
        activeHitboxes.Clear();
        // Create hitbox from attack data
        var hb = new Hitbox
        {
            type = HitboxType.Hitbox,
            offset = attack.hitboxOffset,
            size = attack.hitboxSize,
            damage = attack.damage,
            hitStun = attack.hitStunFrames,
            blockStun = attack.blockStunFrames,
            knockback = attack.knockback,
            launches = attack.launches,
            isActive = true,
            activeFrameStart = attack.startupFrames,
            activeFrameEnd = attack.startupFrames + attack.activeFrames,
            attackData = attack
        };
        activeHitboxes.Add(hb);
    }
    
    protected void DeactivateHitboxes()
    {
        foreach (var hb in activeHitboxes) hb.isActive = false;
        activeHitboxes.Clear();
    }
    
    #endregion
    
    #region Drive System
    
    public bool CanSpendDrive(int cost)
    {
        return currentDriveGauge >= cost && !IsBurnedOut();
    }
    
    public bool IsBurnedOut()
    {
        return currentDriveGauge <= 0;
    }
    
    public void SpendDrive(int cost)
    {
        currentDriveGauge = Mathf.Max(0, currentDriveGauge - cost);
        if (currentDriveGauge <= 0)
        {
            OnBurnout();
        }
    }
    
    protected virtual void OnBurnout()
    {
        // Burnout state - no drive gauge regen for a while
        Debug.Log($"{characterName} burned out!");
    }
    
    protected virtual void TryParry()
    {
        ChangeState(CharacterState.Parry);
    }
    
    protected virtual void TryDriveImpact()
    {
        ChangeState(CharacterState.DriveImpact);
    }
    
    protected virtual void TryDriveRush()
    {
        ChangeState(CharacterState.DriveRush);
    }
    
    protected virtual void TryThrow()
    {
        if (Vector2.Distance(transform.position, opponent.transform.position) < 1.5f)
        {
            ChangeState(CharacterState.ThrowStartup);
        }
    }
    
    #endregion
    
    #region Hit/Block/Damage
    
    public virtual void OnHit(HitResult result)
    {
        if (currentState == CharacterState.Parry && result.attack.type != AttackType.Throw)
        {
            OnParrySuccess(result);
            return;
        }
        
        if (currentState == CharacterState.DriveImpact && stateFrame < 20)
        {
            OnArmoredHit(result);
            return;
        }
        
        // Calculate damage with scaling
        int damage = Mathf.RoundToInt(result.attack.damage * comboDamageScaling);
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        // Hit effects
        ApplyHitPause(result.attack.hitPauseFrames);
        ApplyKnockback(result.attack.knockback, result.attack.launchAngle, result.attack.launches);
        
        // Stun
        hitStunRemaining = result.attack.hitStunFrames;
        if (result.crushCounter) hitStunRemaining = Mathf.RoundToInt(hitStunRemaining * 1.5f);
        
        // Combo system
        comboCount++;
        comboDamageScaling *= 0.9f; // 10% scaling per hit
        
        // Meter gain
        GainSuperMeter(result.attack.superMeterGain);
        
        ChangeState(CharacterState.HitStun);
        
        // Check KO
        if (currentHealth <= 0) OnKO();
    }
    
    public virtual void OnBlock(HitResult result)
    {
        if (currentState == CharacterState.Parry)
        {
            OnParrySuccess(result);
            return;
        }
        
        // Chip damage
        if (result.attack.chipDamage > 0)
        {
            currentHealth = Mathf.Max(1, currentHealth - result.attack.chipDamage);
            if (currentHealth <= 1) OnKO();
        }
        
        ApplyHitPause(result.attack.hitPauseFrames * 0.5f);
        ApplyPushback(result.attack.knockback.x * 0.3f);
        
        blockStunRemaining = result.attack.blockStunFrames;
        isBlocking = true;
        
        // Meter gain on block
        GainSuperMeter(result.attack.superMeterGain / 2);
        
        ChangeState(CharacterState.BlockStun);
    }
    
    protected virtual void OnParrySuccess(HitResult result)
    {
        justParried = true;
        // Parry grants frame advantage and drive gauge
        currentDriveGauge = Mathf.Min(maxDriveGauge, currentDriveGauge + 5000);
        hitPauseFrames = 10; // Parry freeze
        // Opponent gets extra recovery
        if (opponent) opponent.AddHitPause(20);
    }
    
    protected virtual void OnArmoredHit(HitResult result)
    {
        // Armor absorbs one hit
        hitPauseFrames = 5;
        currentDriveGauge = Mathf.Max(0, currentDriveGauge - 5000); // Armor costs drive
    }
    
    protected void ApplyHitPause(float frames)
    {
        hitPauseFrames = Mathf.RoundToInt(frames);
        if (opponent) opponent.hitPauseFrames = Mathf.RoundToInt(frames);
    }
    
    public void AddHitPause(float frames)
    {
        hitPauseFrames = Mathf.Max(hitPauseFrames, Mathf.RoundToInt(frames));
    }
    
    protected void ApplyKnockback(Vector2 knockback, float angle, bool launches)
    {
        float dir = facingRight ? 1 : -1;
        Vector2 kb = new Vector2(knockback.x * dir, knockback.y);
        
        if (launches)
        {
            float rad = angle * Mathf.Deg2Rad;
            kb = new Vector2(Mathf.Cos(rad) * knockback.magnitude * dir, Mathf.Sin(rad) * knockback.magnitude);
            isGrounded = false;
        }
        
        rb.linearVelocity = kb;
        knockdownVelocity = kb;
        
        if (launches || knockback.y > 5)
        {
            isKnockedDown = true;
            hardKnockdown = false;
        }
    }
    
    protected void ApplyPushback(float amount)
    {
        float dir = facingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(amount * dir * pushbackResistance, rb.linearVelocity.y);
    }
    
    protected void GainSuperMeter(int amount)
    {
        currentSuperMeter = Mathf.Min(maxSuperMeter, currentSuperMeter + amount);
    }
    
    protected void ExitKnockdown()
    {
        isKnockedDown = false;
        hardKnockdown = false;
        knockdownVelocity = Vector2.zero;
        ChangeState(CharacterState.GettingUp);
    }
    
    protected virtual void OnKO()
    {
        Debug.Log($"{characterName} KO'd!");
        gameManager?.OnCharacterKO(this);
    }
    
    public void Freeze(bool freeze)
    {
        rb.simulated = !freeze;
        enabled = !freeze;
    }
    
    #endregion
    
    #region Command Buffer
    
    protected enum CommandType
    {
        Light, Medium, Heavy, Special,
        DriveRush, DriveImpact, Parry, DriveRushCancel,
        Throw, Super
    }
    
    protected struct BufferedCommand
    {
        public CommandType type;
        public string motionName;
        public int bufferFrame;
    }
    
    protected void BufferCommand(CommandType type, string motion = null)
    {
        if (commandBuffer.Count >= COMMAND_BUFFER_SIZE) return;
        commandBuffer.Enqueue(new BufferedCommand { type = type, motionName = motion, bufferFrame = currentFrame });
    }
    
    protected virtual bool CanExecuteCommand(BufferedCommand cmd)
    {
        // Can execute if in neutral or cancellable state
        return currentState == CharacterState.Idle || 
               currentState == CharacterState.WalkForward || 
               currentState == CharacterState.WalkBackward ||
               currentState == CharacterState.Crouch ||
               (currentAttack != null && currentAttack.specialCancellable && attackFrame >= currentAttack.cancelStartFrame);
    }
    
    protected virtual void ExecuteBufferedCommand(BufferedCommand cmd)
    {
        switch (cmd.type)
        {
            case CommandType.Light: DoLightAttack(); break;
            case CommandType.Medium: DoMediumAttack(); break;
            case CommandType.Heavy: DoHeavyAttack(); break;
            case CommandType.Special: DoSpecialMove(cmd.motionName); break;
            case CommandType.DriveRush: TryDriveRush(); break;
            case CommandType.DriveImpact: TryDriveImpact(); break;
            case CommandType.Parry: TryParry(); break;
            case CommandType.DriveRushCancel: TryDriveRush(); break;
            case CommandType.Throw: TryThrow(); break;
        }
    }
    
    protected abstract void DoLightAttack();
    protected abstract void DoMediumAttack();
    protected abstract void DoHeavyAttack();
    protected abstract void DoSpecialMove(string motion);
    
    #endregion
    
    #region Collision Detection
    
    public List<Hitbox> GetActiveHitboxes() => activeHitboxes;
    public List<Hitbox> GetHurtboxes() => hurtboxes;
    
    public virtual void SetupHurtboxes()
    {
        // Override per character
    }
    
    #endregion
}