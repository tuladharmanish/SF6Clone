using UnityEngine;

/// <summary>
/// LUKE - Modern Rushdown/Glass Cannon Archetype
/// Fast, high damage, Sand Blaster, Flash Knuckle, Rising Rocket
/// </summary>
public class CharacterLuke : CharacterBase
{
    // Normals
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightPunch, crouchMediumPunch, crouchHeavyPunch;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airLightPunch, airMediumPunch, airHeavyPunch;
    private AttackData airLightKick, airMediumKick, airHeavyKick;
    
    // Specials
    private AttackData sandBlaster, flashKnuckle, risingRocket, chargedFlashKnuckle;
    private AttackData apexKick, apexKickFollowup;
    private AttackData hadoKenLuke, shoryukenLuke; // V-Trigger style
    
    // Super
    private AttackData vermillionSandstorm, criticalArt; // Level 2/3
    
    // Unique: Thrust Strike (6MP), Rising Upper (6HP)
    private AttackData thrustStrike, risingUpper;
    
    // Sand meter mechanic
    private int sandStocks = 0;
    private const int MAX_SAND_STOCKS = 2;
    private float sandRegenTimer = 0f;
    
    protected override void InitializeMoveset()
    {
        characterName = "Luke";
        maxHealth = 9000; // Lower health (glass cannon)
        currentHealth = maxHealth;
        walkSpeed = 6.5f; // Fastest walk
        jumpForce = 19f;
        
        // === NORMALS ===
        lightPunch = CreateAttack("LP", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        lightPunch.hitboxOffset = new Vector2(0.7f, 1.2f); lightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        lightPunch.specialCancellable = true; lightPunch.cancelStartFrame = 3; lightPunch.cancelEndFrame = 5;
        
        mediumPunch = CreateAttack("MP", 5, 3, 8, 10, 8, 400, 100, 100, new Vector2(4, 0));
        mediumPunch.hitboxOffset = new Vector2(0.9f, 1.3f); mediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        mediumPunch.specialCancellable = true; mediumPunch.driveRushCancellable = true;
        
        heavyPunch = CreateAttack("HP", 8, 4, 14, 16, 10, 800, 250, 250, new Vector2(7, 4));
        heavyPunch.hitboxOffset = new Vector2(1.1f, 1.4f); heavyPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        heavyPunch.crushCounter = true;
        
        lightKick = CreateAttack("LK", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        lightKick.hitboxOffset = new Vector2(0.7f, 0.5f); lightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        lightKick.specialCancellable = true;
        
        mediumKick = CreateAttack("MK", 6, 3, 10, 12, 8, 500, 150, 150, new Vector2(5, 0));
        mediumKick.hitboxOffset = new Vector2(1.1f, 0.8f); mediumKick.hitboxSize = new Vector2(0.7f, 0.4f);
        mediumKick.specialCancellable = true; mediumKick.driveRushCancellable = true;
        
        heavyKick = CreateAttack("HK", 10, 4, 16, 18, 10, 900, 300, 300, new Vector2(9, 6));
        heavyKick.hitboxOffset = new Vector2(1.3f, 1.0f); heavyKick.hitboxSize = new Vector2(0.9f, 0.5f);
        heavyKick.launches = true; heavyKick.launchAngle = 55f;
        
        // === CROUCHING ===
        crouchLightPunch = CreateAttack("cr.LP", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        crouchLightPunch.hitboxOffset = new Vector2(0.6f, 0.5f); crouchLightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        crouchLightPunch.specialCancellable = true; crouchLightPunch.isLow = true;
        
        crouchMediumPunch = CreateAttack("cr.MP", 5, 3, 8, 10, 8, 400, 100, 100, new Vector2(4, 0));
        crouchMediumPunch.hitboxOffset = new Vector2(0.8f, 0.6f); crouchMediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        crouchMediumPunch.specialCancellable = true; crouchMediumPunch.isLow = true;
        
        crouchHeavyPunch = CreateAttack("cr.HP", 8, 4, 14, 16, 10, 700, 200, 200, new Vector2(6, 8));
        crouchHeavyPunch.hitboxOffset = new Vector2(0.7f, 1.5f); crouchHeavyPunch.hitboxSize = new Vector2(0.5f, 0.8f);
        crouchHeavyPunch.isLow = true; crouchHeavyPunch.launches = true; crouchHeavyPunch.launchAngle = 75f;
        crouchHeavyPunch.specialCancellable = true;
        
        crouchLightKick = CreateAttack("cr.LK", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.7f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.5f, 0.2f);
        crouchLightKick.specialCancellable = true; crouchLightKick.isLow = true;
        
        crouchMediumKick = CreateAttack("cr.MK", 6, 3, 10, 12, 9, 500, 150, 150, new Vector2(5, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.0f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.8f, 0.3f);
        crouchMediumKick.specialCancellable = true; crouchMediumKick.isLow = true;
        
        crouchHeavyKick = CreateAttack("cr.HK", 10, 4, 18, 20, 12, 800, 250, 250, new Vector2(8, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.3f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.0f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true;
        
        // === AIR ===
        airLightPunch = CreateAttack("j.LP", 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightPunch.hitboxOffset = new Vector2(0.7f, 0.5f); airLightPunch.hitboxSize = new Vector2(0.4f, 0.4f);
        
        airMediumPunch = CreateAttack("j.MP", 5, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumPunch.hitboxOffset = new Vector2(0.9f, 0.3f); airMediumPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        
        airHeavyPunch = CreateAttack("j.HP", 7, 4, 12, 14, 10, 700, 200, 200, new Vector2(7, 0));
        airHeavyPunch.hitboxOffset = new Vector2(1.0f, 0.2f); airHeavyPunch.hitboxSize = new Vector2(0.7f, 0.6f);
        airHeavyPunch.launches = true; airHeavyPunch.launchAngle = 45f;
        
        airLightKick = CreateAttack("j.LK", 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightKick.hitboxOffset = new Vector2(0.8f, -0.2f); airLightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        airMediumKick = CreateAttack("j.MK", 5, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumKick.hitboxOffset = new Vector2(1.0f, -0.3f); airMediumKick.hitboxSize = new Vector2(0.7f, 0.4f);
        airMediumKick.isOverhead = true;
        
        airHeavyKick = CreateAttack("j.HK", 8, 4, 12, 14, 10, 700, 200, 200, new Vector2(7, 2));
        airHeavyKick.hitboxOffset = new Vector2(1.2f, -0.2f); airHeavyKick.hitboxSize = new Vector2(0.8f, 0.5f);
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ===
        // Thrust Strike (6MP) - 13f startup, advances, +2 on block
        thrustStrike = CreateAttack("Thrust Strike", AttackType.Unique, 13, 3, 12, 14, 10, 500, 150, 150, new Vector2(5, 0));
        thrustStrike.hitboxOffset = new Vector2(1.3f, 1.2f); thrustStrike.hitboxSize = new Vector2(0.6f, 0.4f);
        thrustStrike.specialCancellable = true; thrustStrike.driveRushCancellable = true;
        
        // Rising Upper (6HP) - 15f startup, anti-air, launches
        risingUpper = CreateAttack("Rising Upper", AttackType.Unique, 15, 4, 18, 20, 12, 900, 300, 300, new Vector2(8, 10));
        risingUpper.hitboxOffset = new Vector2(0.9f, 1.5f); risingUpper.hitboxSize = new Vector2(0.5f, 1.0f);
        risingUpper.launches = true; risingUpper.launchAngle = 70f; risingUpper.specialCancellable = true;
        
        // === SPECIALS ===
        // Sand Blaster (QCF+P) - 13f startup, projectile, builds sand stock on hit
        sandBlaster = CreateAttack("Sand Blaster", AttackType.Special, 13, 2, 24, 12, 8, 400, 150, 300, new Vector2(4, 0));
        sandBlaster.hitboxOffset = new Vector2(1.4f, 1.0f); sandBlaster.hitboxSize = new Vector2(0.5f, 0.5f);
        sandBlaster.isProjectile = true; sandBlaster.superCancellable = true;
        
        // Flash Knuckle (QCB+P) - 10f startup, advancing punch, 3 versions
        flashKnuckle = CreateAttack("Flash Knuckle", AttackType.Special, 10, 4, 20, 16, 8, 700, 200, 300, new Vector2(8, 2));
        flashKnuckle.hitboxOffset = new Vector2(1.2f, 1.1f); flashKnuckle.hitboxSize = new Vector2(0.6f, 0.5f);
        flashKnuckle.superCancellable = true;
        
        // Charged Flash Knuckle (Hold P) - 25f startup, armor, wall bounce
        chargedFlashKnuckle = CreateAttack("Charged Flash Knuckle", AttackType.Special, 25, 4, 22, 22, 10, 1200, 400, 500, new Vector2(12, 4));
        chargedFlashKnuckle.hitboxOffset = new Vector2(1.5f, 1.1f); chargedFlashKnuckle.hitboxSize = new Vector2(0.7f, 0.6f);
        chargedFlashKnuckle.armor = true; chargedFlashKnuckle.armorHits = 2; chargedFlashKnuckle.wallBounce = true;
        
        // Rising Rocket (DP+K) - 5f startup, invincible, multi-hit
        risingRocket = CreateAttack("Rising Rocket", AttackType.Special, 5, 12, 28, 20, -8, 1100, 350, 500, new Vector2(6, 14));
        risingRocket.hitboxOffset = new Vector2(0.5f, 0.5f); risingRocket.hitboxSize = new Vector2(0.5f, 1.8f);
        risingRocket.isInvincible = true; risingRocket.invincibleFrames = 8; risingRocket.launches = true; risingRocket.launchAngle = 80f;
        risingRocket.superCancellable = true;
        
        // Apex Kick (QCF+K) - 14f startup, overhead kick, followup
        apexKick = CreateAttack("Apex Kick", AttackType.Special, 14, 4, 18, 20, 10, 700, 250, 250, new Vector2(8, 6));
        apexKick.hitboxOffset = new Vector2(1.2f, 1.3f); apexKick.hitboxSize = new Vector2(0.7f, 0.5f);
        apexKick.isOverhead = true; apexKick.superCancellable = true;
        
        // === SUPER ARTS ===
        // Vermillion Sandstorm (QCFx2+P) - 9f startup, giant sand blast
        vermillionSandstorm = CreateAttack("Vermillion Sandstorm", AttackType.Super, 9, 24, 38, 30, 10, 2800, 500, 0, new Vector2(14, 4));
        vermillionSandstorm.hitboxOffset = new Vector2(1.5f, 1.0f); vermillionSandstorm.hitboxSize = new Vector2(1.5f, 1.0f);
        vermillionSandstorm.isProjectile = true; vermillionSandstorm.isInvincible = true; vermillionSandstorm.invincibleFrames = 8;
        
        // Critical Art (DPx2+P) - 4f startup, cinematic
        criticalArt = CreateAttack("Critical Art", AttackType.Super, 4, 30, 50, 40, -20, 3800, 0, 0, new Vector2(15, 15));
        criticalArt.hitboxOffset = new Vector2(0.5f, 0.5f); criticalArt.hitboxSize = new Vector2(0.8f, 2.0f);
        criticalArt.isInvincible = true; criticalArt.invincibleFrames = 12; criticalArt.launches = true;
        
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
            hurtboxSize = new Vector2(0.55f, 1.75f)
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
        hurtboxes.Add(new Hitbox { type = HitboxType.Hurtbox, offset = new Vector2(0, 1), size = new Vector2(0.55f, 1.75f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.ThrowHurtbox, offset = new Vector2(0, 1), size = new Vector2(0.7f, 1.75f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.Pushbox, offset = new Vector2(0, 1), size = new Vector2(0.45f, 1.75f), isActive = true });
    }
    
    protected override void GenerateProceduralSprites()
    {
        // Luke - Blue/white MMA outfit, blonde hair, sand effects
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // Sand stock regen
        if (sandStocks < MAX_SAND_STOCKS)
        {
            sandRegenTimer += Time.fixedDeltaTime;
            if (sandRegenTimer >= 10f) // 10 seconds per stock
            {
                sandStocks++;
                sandRegenTimer = 0f;
            }
        }
    }
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return risingUpper; // 6HP
            if (!crouching && !jumping && (facingRight ? dir == Direction.Back : dir == Direction.Forward))
                return thrustStrike; // 6MP (using back+medium)
            if (crouching) return crouchHeavyPunch;
            if (jumping) return airHeavyPunch;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return thrustStrike; // 6MP
            if (crouching) return crouchMediumPunch;
            if (jumping) return airMediumPunch;
            return mediumPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Light))
        {
            if (crouching) return crouchLightPunch;
            if (jumping) return airLightPunch;
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
        
        switch (motion)
        {
            case "QCF": special = sandBlaster; break;
            case "QCB": special = flashKnuckle; break;
            case "DP": special = risingRocket; break;
        }
        
        if (special != null)
        {
            currentAttack = special;
            ChangeState(CharacterState.AttackStartup);
        }
    }
}