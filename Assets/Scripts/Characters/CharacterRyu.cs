using UnityEngine;

/// <summary>
/// RYU - The Shoto Archetype
/// Balanced zoning and rushdown, fireball/DP/tatsu core
/// </summary>
public class CharacterRyu : CharacterBase
{
    // Move definitions
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightPunch, crouchMediumPunch, crouchHeavyPunch;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airLightPunch, airMediumPunch, airHeavyPunch;
    private AttackData airLightKick, airMediumKick, airHeavyKick;
    
    // Specials
    private AttackData hadoken, shoryuken, tatsumaki;
    private AttackData shinkuHadoken, shinShoryuken; // Supers
    
    // Unique: Collarbone Breaker (6HP), Solar Plexus (4MP)
    private AttackData collarboneBreaker, solarPlexus;
    
    protected override void InitializeMoveset()
    {
        characterName = "Ryu";
        
        // === NORMALS ===
        // Standing Light Punch - 4f startup, +1 on block
        lightPunch = CreateAttack("Stand LP", AttackType.Normal, 4, 2, 6, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightPunch.hitboxOffset = new Vector2(0.8f, 1.2f); lightPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        lightPunch.specialCancellable = true; lightPunch.cancelStartFrame = 4; lightPunch.cancelEndFrame = 6;
        
        // Standing Medium Punch - 6f startup, +2 on block
        mediumPunch = CreateAttack("Stand MP", AttackType.Normal, 6, 3, 10, 12, 8, 400, 100, 100, new Vector2(5, 0));
        mediumPunch.hitboxOffset = new Vector2(1.0f, 1.3f); mediumPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        mediumPunch.specialCancellable = true; mediumPunch.driveRushCancellable = true;
        
        // Standing Heavy Punch - 10f startup, -4 on block, crush counter
        heavyPunch = CreateAttack("Stand HP", AttackType.Normal, 10, 4, 18, 18, 10, 700, 200, 200, new Vector2(8, 2));
        heavyPunch.hitboxOffset = new Vector2(1.2f, 1.4f); heavyPunch.hitboxSize = new Vector2(0.7f, 0.6f);
        heavyPunch.crushCounter = true; heavyPunch.specialCancellable = true;
        
        // Standing Light Kick - 4f startup
        lightKick = CreateAttack("Stand LK", AttackType.Normal, 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightKick.hitboxOffset = new Vector2(0.9f, 0.6f); lightKick.hitboxSize = new Vector2(0.5f, 0.5f);
        lightKick.specialCancellable = true;
        
        // Standing Medium Kick - 7f startup, good range
        mediumKick = CreateAttack("Stand MK", AttackType.Normal, 7, 3, 12, 14, 10, 500, 150, 150, new Vector2(6, 0));
        mediumKick.hitboxOffset = new Vector2(1.3f, 0.8f); mediumKick.hitboxSize = new Vector2(0.8f, 0.5f);
        mediumKick.specialCancellable = true; mediumKick.driveRushCancellable = true;
        
        // Standing Heavy Kick - 12f startup, launches
        heavyKick = CreateAttack("Stand HK", AttackType.Normal, 12, 4, 20, 20, 12, 800, 250, 250, new Vector2(10, 8));
        heavyKick.hitboxOffset = new Vector2(1.5f, 1.0f); heavyKick.hitboxSize = new Vector2(0.9f, 0.6f);
        heavyKick.launches = true; heavyKick.launchAngle = 60f;
        
        // === CROUCHING ===
        crouchLightPunch = CreateAttack("Crouch LP", AttackType.Normal, 4, 2, 6, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightPunch.hitboxOffset = new Vector2(0.6f, 0.5f); crouchLightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        crouchLightPunch.specialCancellable = true; crouchLightPunch.isLow = true;
        
        crouchMediumPunch = CreateAttack("Crouch MP", AttackType.Normal, 6, 3, 9, 10, 8, 400, 100, 100, new Vector2(4, 0));
        crouchMediumPunch.hitboxOffset = new Vector2(0.9f, 0.6f); crouchMediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        crouchMediumPunch.specialCancellable = true; crouchMediumPunch.isLow = true;
        
        crouchHeavyPunch = CreateAttack("Crouch HP", AttackType.Normal, 9, 4, 15, 16, 10, 600, 200, 200, new Vector2(6, 6));
        crouchHeavyPunch.hitboxOffset = new Vector2(0.8f, 1.5f); crouchHeavyPunch.hitboxSize = new Vector2(0.5f, 0.8f);
        crouchHeavyPunch.launches = true; crouchHeavyPunch.launchAngle = 75f; crouchHeavyPunch.isLow = true;
        crouchHeavyPunch.specialCancellable = true;
        
        crouchLightKick = CreateAttack("Crouch LK", AttackType.Normal, 4, 2, 6, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.7f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.6f, 0.2f);
        crouchLightKick.specialCancellable = true; crouchLightKick.isLow = true;
        
        crouchMediumKick = CreateAttack("Crouch MK", AttackType.Normal, 7, 3, 10, 12, 10, 500, 150, 150, new Vector2(5, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.1f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.8f, 0.3f);
        crouchMediumKick.specialCancellable = true; crouchMediumKick.isLow = true;
        
        crouchHeavyKick = CreateAttack("Crouch HK", AttackType.Normal, 10, 4, 18, 20, 12, 700, 200, 200, new Vector2(8, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.4f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.0f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true; crouchHeavyKick.specialCancellable = true;
        
        // === AIR NORMALS ===
        airLightPunch = CreateAttack("Air LP", AttackType.Normal, 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightPunch.hitboxOffset = new Vector2(0.7f, 0.5f); airLightPunch.hitboxSize = new Vector2(0.4f, 0.4f);
        
        airMediumPunch = CreateAttack("Air MP", AttackType.Normal, 6, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumPunch.hitboxOffset = new Vector2(0.9f, 0.3f); airMediumPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        
        airHeavyPunch = CreateAttack("Air HP", AttackType.Normal, 8, 4, 14, 16, 10, 700, 200, 200, new Vector2(8, 0));
        airHeavyPunch.hitboxOffset = new Vector2(1.0f, 0.2f); airHeavyPunch.hitboxSize = new Vector2(0.7f, 0.6f);
        airHeavyPunch.launches = true; airHeavyPunch.launchAngle = 45f;
        
        airLightKick = CreateAttack("Air LK", AttackType.Normal, 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightKick.hitboxOffset = new Vector2(0.8f, -0.2f); airLightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        airMediumKick = CreateAttack("Air MK", AttackType.Normal, 6, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumKick.hitboxOffset = new Vector2(1.0f, -0.3f); airMediumKick.hitboxSize = new Vector2(0.7f, 0.4f);
        airMediumKick.isOverhead = true;
        
        airHeavyKick = CreateAttack("Air HK", AttackType.Normal, 9, 4, 14, 16, 10, 700, 200, 200, new Vector2(8, 2));
        airHeavyKick.hitboxOffset = new Vector2(1.2f, -0.2f); airHeavyKick.hitboxSize = new Vector2(0.8f, 0.5f);
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ATTACKS ===
        // Collarbone Breaker (6HP) - Overhead, 19f startup
        collarboneBreaker = CreateAttack("Collarbone Breaker", AttackType.Unique, 19, 4, 16, 20, 12, 800, 250, 250, new Vector2(10, 4));
        collarboneBreaker.hitboxOffset = new Vector2(1.3f, 1.5f); collarboneBreaker.hitboxSize = new Vector2(0.7f, 0.5f);
        collarboneBreaker.isOverhead = true; collarboneBreaker.specialCancellable = true;
        
        // Solar Plexus (4MP) - 14f startup, +2 on block
        solarPlexus = CreateAttack("Solar Plexus", AttackType.Unique, 14, 3, 12, 16, 10, 600, 200, 200, new Vector2(6, 0));
        solarPlexus.hitboxOffset = new Vector2(1.1f, 1.0f); solarPlexus.hitboxSize = new Vector2(0.6f, 0.4f);
        solarPlexus.specialCancellable = true; solarPlexus.driveRushCancellable = true;
        
        // === SPECIALS ===
        // Hadoken (QCF+P) - 13f startup, projectile
        hadoken = CreateAttack("Hadoken", AttackType.Special, 13, 2, 28, 14, 10, 500, 150, 300, new Vector2(5, 0));
        hadoken.hitboxOffset = new Vector2(1.5f, 1.0f); hadoken.hitboxSize = new Vector2(0.6f, 0.6f);
        hadoken.isProjectile = true; hadoken.superCancellable = true;
        
        // Shoryuken (DP+P) - 3f startup, invincible, DP motion
        shoryuken = CreateAttack("Shoryuken", AttackType.Special, 3, 6, 30, 20, -10, 900, 300, 500, new Vector2(8, 12));
        shoryuken.hitboxOffset = new Vector2(0.5f, 0.5f); shoryuken.hitboxSize = new Vector2(0.5f, 1.5f);
        shoryuken.isInvincible = true; shoryuken.invincibleFrames = 8; shoryuken.launches = true; shoryuken.launchAngle = 80f;
        shoryuken.superCancellable = true;
        
        // Tatsumaki (QCB+K) - 8f startup, multi-hit
        tatsumaki = CreateAttack("Tatsumaki", AttackType.Special, 8, 12, 22, 16, 8, 1000, 300, 400, new Vector2(6, 4));
        tatsumaki.hitboxOffset = new Vector2(1.0f, 0.8f); tatsumaki.hitboxSize = new Vector2(0.8f, 0.6f);
        tatsumaki.superCancellable = true;
        
        // === SUPER ARTS ===
        // Shinku Hadoken (QCFx2+P) - 10f startup, super
        shinkuHadoken = CreateAttack("Shinku Hadoken", AttackType.Super, 10, 20, 40, 30, 10, 2500, 500, 0, new Vector2(10, 0));
        shinkuHadoken.hitboxOffset = new Vector2(1.5f, 1.0f); shinkuHadoken.hitboxSize = new Vector2(1.0f, 0.8f);
        shinkuHadoken.isProjectile = true;
        
        // Shin Shoryuken (DPx2+P) - 4f startup, invincible
        shinShoryuken = CreateAttack("Shin Shoryuken", AttackType.Super, 4, 30, 50, 40, -20, 3500, 0, 0, new Vector2(12, 20));
        shinShoryuken.hitboxOffset = new Vector2(0.5f, 0.5f); shinShoryuken.hitboxSize = new Vector2(0.6f, 2.0f);
        shinShoryuken.isInvincible = true; shinShoryuken.invincibleFrames = 12; shinShoryuken.launches = true;
        
        // Setup hurtboxes
        SetupHurtboxes();
    }
    
    private AttackData CreateAttack(string name, AttackType type, int startup, int active, int recovery, 
        int hitStun, int blockStun, int damage, int driveGain, int superGain, Vector2 knockback)
    {
        return new AttackData
        {
            moveName = name,
            type = type,
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
            launchAngle = 0f,
            hurtboxOffset = new Vector2(0, 1),
            hurtboxSize = new Vector2(0.6f, 1.8f)
        };
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
        // Ryu - White gi, red headband
        CreateProceduralSprite("Idle", Color.white, new Color(0.8f, 0.1f, 0.1f), Color.black);
        CreateProceduralSprite("Walk", Color.white, new Color(0.8f, 0.1f, 0.1f), Color.black);
        CreateProceduralSprite("Crouch", Color.white, new Color(0.8f, 0.1f, 0.1f), Color.black);
        CreateProceduralSprite("Jump", Color.white, new Color(0.8f, 0.1f, 0.1f), Color.black);
        CreateProceduralSprite("Attack", new Color(1f, 0.9f, 0.7f), new Color(0.8f, 0.1f, 0.1f), Color.black);
        CreateProceduralSprite("Hadoken", Color.blue, Color.cyan, Color.white);
        CreateProceduralSprite("Shoryuken", new Color(1f, 0.5f, 0f), Color.red, Color.yellow);
    }
    
    private void CreateProceduralSprite(string name, Color body, Color accent, Color outline)
    {
        // In a real game, this would create actual sprites. For now, we'll use the sprite renderer color.
        // This is a placeholder for the visual system.
    }
    
    // === ATTACK EXECUTION ===
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        // Priority: Unique > Heavy > Medium > Light
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return collarboneBreaker; // 6HP
            if (!crouching && !jumping && (facingRight ? dir == Direction.Back : dir == Direction.Forward))
                return solarPlexus; // 4MP
            if (crouching) return crouchHeavyPunch;
            if (jumping) return airHeavyPunch;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
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
    
    protected override void DoLightAttack()
    {
        ChangeState(CharacterState.AttackStartup);
    }
    
    protected override void DoMediumAttack()
    {
        ChangeState(CharacterState.AttackStartup);
    }
    
    protected override void DoHeavyAttack()
    {
        ChangeState(CharacterState.AttackStartup);
    }
    
    protected override void DoSpecialMove(string motion)
    {
        AttackData special = null;
        
        switch (motion)
        {
            case "QCF": special = hadoken; break;
            case "DP": special = shoryuken; break;
            case "QCB": special = tatsumaki; break;
        }
        
        if (special != null && CanSpendDrive(0)) // Specials don't cost drive normally
        {
            currentAttack = special;
            ChangeState(CharacterState.AttackStartup);
        }
    }
}