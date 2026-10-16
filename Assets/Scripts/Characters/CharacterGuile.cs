using UnityEngine;

/// <summary>
/// GUILE - Charge/Zoner Archetype
/// Sonic Boom, Flash Kick, strong normals, defensive playstyle
/// </summary>
public class CharacterGuile : CharacterBase
{
    // Normals
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airHeavyPunch, airHeavyKick;
    
    // Specials (Charge moves)
    private AttackData sonicBoom, flashKick, sonicCross, somersaultStrike;
    private AttackData sonicHurricane, flashExplosion; // Supers
    
    // Unique: Rolling Sobat (4HK), Spinning Backfist (6HP)
    private AttackData rollingSobat, spinningBackfist;
    
    // Charge tracking
    private float backChargeTime = 0f;
    private float downChargeTime = 0f;
    private const float CHARGE_TIME_REQUIRED = 35f; // ~35 frames at 60fps
    
    protected override void InitializeMoveset()
    {
        characterName = "Guile";
        walkSpeed = 4.5f;
        
        // === NORMALS ===
        lightPunch = CreateAttack("LP", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightPunch.hitboxOffset = new Vector2(0.7f, 1.4f); lightPunch.hitboxSize = new Vector2(0.4f, 0.4f);
        lightPunch.specialCancellable = true;
        
        mediumPunch = CreateAttack("MP", 6, 3, 10, 12, 8, 500, 150, 150, new Vector2(5, 0));
        mediumPunch.hitboxOffset = new Vector2(0.9f, 1.5f); mediumPunch.hitboxSize = new Vector2(0.5f, 0.5f);
        mediumPunch.specialCancellable = true; mediumPunch.driveRushCancellable = true;
        
        heavyPunch = CreateAttack("HP", 9, 4, 16, 16, 10, 800, 250, 250, new Vector2(8, 2));
        heavyPunch.hitboxOffset = new Vector2(1.1f, 1.6f); heavyPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        heavyPunch.crushCounter = true;
        
        lightKick = CreateAttack("LK", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightKick.hitboxOffset = new Vector2(0.8f, 0.5f); lightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        mediumKick = CreateAttack("MK", 7, 3, 11, 14, 10, 600, 200, 200, new Vector2(6, 0));
        mediumKick.hitboxOffset = new Vector2(1.2f, 0.8f); mediumKick.hitboxSize = new Vector2(0.7f, 0.5f);
        mediumKick.specialCancellable = true; mediumKick.driveRushCancellable = true;
        
        heavyKick = CreateAttack("HK", 12, 4, 18, 18, 10, 900, 300, 300, new Vector2(10, 6));
        heavyKick.hitboxOffset = new Vector2(1.4f, 1.0f); heavyKick.hitboxSize = new Vector2(0.9f, 0.6f);
        heavyKick.launches = true; heavyKick.launchAngle = 50f;
        
        // === CROUCHING ===
        crouchLightKick = CreateAttack("cr.LK", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.6f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.5f, 0.2f);
        crouchLightKick.isLow = true; crouchLightKick.specialCancellable = true;
        
        crouchMediumKick = CreateAttack("cr.MK", 6, 3, 10, 12, 9, 500, 150, 150, new Vector2(5, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.0f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.8f, 0.3f);
        crouchMediumKick.isLow = true; crouchMediumKick.specialCancellable = true;
        
        crouchHeavyKick = CreateAttack("cr.HK", 10, 4, 18, 20, 12, 900, 300, 300, new Vector2(8, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.3f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.1f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true;
        
        // === AIR ===
        airHeavyPunch = CreateAttack("j.HP", 8, 4, 12, 16, 10, 800, 250, 250, new Vector2(8, 2));
        airHeavyPunch.hitboxOffset = new Vector2(1.0f, 0.0f); airHeavyPunch.hitboxSize = new Vector2(0.7f, 0.5f);
        airHeavyPunch.launches = true; airHeavyPunch.launchAngle = 40f;
        
        airHeavyKick = CreateAttack("j.HK", 8, 4, 12, 16, 10, 800, 250, 250, new Vector2(8, 0));
        airHeavyKick.hitboxOffset = new Vector2(1.3f, -0.2f); airHeavyKick.hitboxSize = new Vector2(1.0f, 0.4f);
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ATTACKS ===
        // Spinning Backfist (6HP) - 16f startup, moves forward, crush counter
        spinningBackfist = CreateAttack("Spinning Backfist", AttackType.Unique, 16, 4, 18, 20, 10, 900, 300, 300, new Vector2(10, 2));
        spinningBackfist.hitboxOffset = new Vector2(1.3f, 1.5f); spinningBackfist.hitboxSize = new Vector2(0.8f, 0.5f);
        spinningBackfist.crushCounter = true; spinningBackfist.specialCancellable = true;
        
        // Rolling Sobat (4HK) - 14f startup, low profile, goes over lows
        rollingSobat = CreateAttack("Rolling Sobat", AttackType.Unique, 14, 6, 16, 18, 8, 700, 200, 200, new Vector2(8, 0));
        rollingSobat.hitboxOffset = new Vector2(1.2f, 0.6f); rollingSobat.hitboxSize = new Vector2(0.8f, 0.4f);
        rollingSobat.specialCancellable = true;
        
        // === SPECIALS (CHARGE) ===
        // Sonic Boom (Charge Back, Forward + P) - 15f startup
        sonicBoom = CreateAttack("Sonic Boom", AttackType.Special, 15, 2, 28, 12, 8, 400, 150, 300, new Vector2(4, 0));
        sonicBoom.hitboxOffset = new Vector2(1.5f, 1.1f); sonicBoom.hitboxSize = new Vector2(0.6f, 0.6f);
        sonicBoom.isProjectile = true; sonicBoom.superCancellable = true;
        
        // Flash Kick (Charge Down, Up + K) - 5f startup, invincible
        flashKick = CreateAttack("Flash Kick", AttackType.Special, 5, 8, 30, 20, -10, 1000, 300, 500, new Vector2(8, 14));
        flashKick.hitboxOffset = new Vector2(0.5f, 0.5f); flashKick.hitboxSize = new Vector2(0.6f, 1.8f);
        flashKick.isInvincible = true; flashKick.invincibleFrames = 10; flashKick.launches = true; flashKick.launchAngle = 80f;
        flashKick.superCancellable = true;
        
        // Sonic Cross (QCF+P) - 14f startup, two hits, crosses up
        sonicCross = CreateAttack("Sonic Cross", AttackType.Special, 14, 6, 26, 14, 8, 600, 200, 300, new Vector2(5, 2));
        sonicCross.hitboxOffset = new Vector2(1.3f, 1.1f); sonicCross.hitboxSize = new Vector2(0.7f, 0.7f);
        sonicCross.isProjectile = true; sonicCross.superCancellable = true;
        
        // Somersault Strike (QCB+K) - 9f startup, anti-air
        somersaultStrike = CreateAttack("Somersault Strike", AttackType.Special, 9, 6, 24, 18, 8, 800, 250, 400, new Vector2(6, 10));
        somersaultStrike.hitboxOffset = new Vector2(0.6f, 0.8f); somersaultStrike.hitboxSize = new Vector2(0.6f, 1.5f);
        somersaultStrike.isInvincible = true; somersaultStrike.invincibleFrames = 6; somersaultStrike.launches = true;
        somersaultStrike.superCancellable = true;
        
        // === SUPER ARTS ===
        // Sonic Hurricane (Charge Back, Forward, Back, Forward + P) - 10f startup
        sonicHurricane = CreateAttack("Sonic Hurricane", AttackType.Super, 10, 30, 40, 25, 10, 2500, 500, 0, new Vector2(10, 0));
        sonicHurricane.hitboxOffset = new Vector2(1.5f, 1.1f); sonicHurricane.hitboxSize = new Vector2(1.2f, 0.8f);
        sonicHurricane.isProjectile = true;
        
        // Flash Explosion (Charge Down, Up, Down, Up + K) - 4f startup, invincible
        flashExplosion = CreateAttack("Flash Explosion", AttackType.Super, 4, 20, 45, 30, -15, 3200, 0, 0, new Vector2(12, 18));
        flashExplosion.hitboxOffset = new Vector2(0.5f, 0.5f); flashExplosion.hitboxSize = new Vector2(0.8f, 2.0f);
        flashExplosion.isInvincible = true; flashExplosion.invincibleFrames = 12; flashExplosion.launches = true;
        
        SetupHurtboxes();
    }
    
    private AttackData CreateAttack(string name, int startup, int active, int recovery, 
        int hitStun, int blockStun, int damage, int driveGain, int superGain, Vector2 knockback)
    {
        return new AttackData
        {
            moveName = name,
            type = AttackType.Normal,
            startupFrames = startup,
            activeFrames = active,
            recoveryFrames = recovery,
            hitStunFrames = hitStun,
            blockStunFrames = blockStun,
            damage = damage,
            chipDamage = damage / 10,
            driveMeterGain = driveGain,
            superMeterGain = superGain,
            knockback = knockback,
            hurtboxOffset = new Vector2(0, 1),
            hurtboxSize = new Vector2(0.6f, 1.8f)
        };
    }
    
    private AttackData CreateAttack(string name, AttackType type, int startup, int active, int recovery, 
        int hitStun, int blockStun, int damage, int driveGain, int superGain, Vector2 knockback)
    {
        var a = CreateAttack(name, startup, active, recovery, hitStun, blockStun, damage, driveGain, superGain, knockback);
        a.type = type;
        return a;
    }
    
    protected override void SetupHurtboxes()
    {
        hurtboxes.Clear();
        hurtboxes.Add(new Hitbox { type = HitboxType.Hurtbox, offset = new Vector2(0, 1), size = new Vector2(0.6f, 1.8f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.ThrowHurtbox, offset = new Vector2(0, 1), size = new Vector2(0.8f, 1.8f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.Pushbox, offset = new Vector2(0, 1), size = new Vector2(0.5f, 1.8f), isActive = true });
    }
    
    protected override void GenerateProceduralSprites()
    {
        // Guile - Green military outfit, blonde flat top, dog tags
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateCharge();
    }
    
    private void UpdateCharge()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool holdingBack = facingRight ? dir == Direction.Back : dir == Direction.Forward;
        bool holdingDown = dir == Direction.Down || dir == Direction.DownBack || dir == Direction.DownForward;
        
        if (holdingBack) backChargeTime += Time.fixedDeltaTime;
        else backChargeTime = 0f;
        
        if (holdingDown) downChargeTime += Time.fixedDeltaTime;
        else downChargeTime = 0f;
    }
    
    public bool HasBackCharge() => backChargeTime >= CHARGE_TIME_REQUIRED / 60f;
    public bool HasDownCharge() => downChargeTime >= CHARGE_TIME_REQUIRED / 60f;
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return spinningBackfist; // 6HP
            if (!crouching && !jumping && (facingRight ? dir == Direction.Back : dir == Direction.Forward))
                return rollingSobat; // 4HK
            if (crouching) return crouchHeavyKick;
            if (jumping) return airHeavyPunch;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
            if (crouching) return crouchMediumKick;
            return mediumKick;
        }
        if (inputManager.GetButton(playerIndex, Button.Light))
        {
            if (crouching) return crouchLightKick;
            return lightPunch;
        }
        return null;
    }
    
    protected override void DoLightAttack() => ChangeState(CharacterState.AttackStartup);
    protected override void DoMediumAttack() => ChangeState(CharacterState.AttackStartup);
    protected override void DoHeavyAttack() => ChangeState(CharacterState.AttackStartup);
    
    protected override void DoSpecialMove(string motion)
    {
        AttackData special = null;
        bool chargeOK = false;
        
        switch (motion)
        {
            case "ChargeBackForward":
                if (HasBackCharge()) { special = sonicBoom; chargeOK = true; }
                break;
            case "ChargeDownUp":
                if (HasDownCharge()) { special = flashKick; chargeOK = true; }
                break;
            case "QCF": special = sonicCross; chargeOK = true; break;
            case "QCB": special = somersaultStrike; chargeOK = true; break;
        }
        
        if (special != null && chargeOK)
        {
            // Consume charge
            if (motion == "ChargeBackForward") backChargeTime = 0f;
            if (motion == "ChargeDownUp") downChargeTime = 0f;
            
            currentAttack = special;
            ChangeState(CharacterState.AttackStartup);
        }
    }
}