using UnityEngine;

/// <summary>
/// CHUN-LI - Rushdown/Poking Archetype
/// Fast normals, Lightning Legs, Spinning Bird Kick, air mobility
/// </summary>
public class CharacterChunLi : CharacterBase
{
    // Normals
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airLightKick, airMediumKick, airHeavyKick;
    
    // Specials
    private AttackData kikoken, spinningBirdKick, lightningLegs, hazanshu;
    private AttackData kikoshou, hoyokusen; // Supers
    
    // Unique: Kakuritsu Ren (6MK), Yosokyaku (4HK)
    private AttackData kakuritsuRen, yosokyaku;
    
    // Target combo: LP > MP > HP
    private AttackData targetCombo2, targetCombo3;
    
    protected override void InitializeMoveset()
    {
        characterName = "Chun-Li";
        walkSpeed = 6f; // Faster walk speed
        
        // === NORMALS ===
        lightPunch = CreateAttack("LP", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        lightPunch.hitboxOffset = new Vector2(0.6f, 1.3f); lightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        lightPunch.specialCancellable = true; lightPunch.cancelStartFrame = 3; lightPunch.cancelEndFrame = 5;
        
        mediumPunch = CreateAttack("MP", 5, 3, 8, 10, 8, 400, 100, 100, new Vector2(4, 0));
        mediumPunch.hitboxOffset = new Vector2(0.8f, 1.4f); mediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        mediumPunch.specialCancellable = true; mediumPunch.driveRushCancellable = true;
        
        heavyPunch = CreateAttack("HP", 8, 4, 14, 16, 8, 700, 200, 200, new Vector2(6, 2));
        heavyPunch.hitboxOffset = new Vector2(1.0f, 1.5f); heavyPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        heavyPunch.crushCounter = true;
        
        lightKick = CreateAttack("LK", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        lightKick.hitboxOffset = new Vector2(0.7f, 0.5f); lightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        lightKick.specialCancellable = true;
        
        mediumKick = CreateAttack("MK", 6, 3, 10, 12, 8, 500, 150, 150, new Vector2(5, 0));
        mediumKick.hitboxOffset = new Vector2(1.2f, 0.8f); mediumKick.hitboxSize = new Vector2(0.8f, 0.4f);
        mediumKick.specialCancellable = true; mediumKick.driveRushCancellable = true;
        
        heavyKick = CreateAttack("HK", 10, 4, 16, 18, 10, 800, 250, 250, new Vector2(8, 6));
        heavyKick.hitboxOffset = new Vector2(1.4f, 1.0f); heavyKick.hitboxSize = new Vector2(0.9f, 0.5f);
        heavyKick.launches = true; heavyKick.launchAngle = 50f;
        
        // === CROUCHING ===
        crouchLightKick = CreateAttack("cr.LK", 3, 2, 5, 7, 6, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.6f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.5f, 0.2f);
        crouchLightKick.specialCancellable = true; crouchLightKick.isLow = true;
        
        crouchMediumKick = CreateAttack("cr.MK", 6, 3, 9, 11, 9, 400, 100, 100, new Vector2(4, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.0f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.7f, 0.3f);
        crouchMediumKick.specialCancellable = true; crouchMediumKick.isLow = true;
        
        crouchHeavyKick = CreateAttack("cr.HK", 9, 4, 16, 18, 10, 700, 200, 200, new Vector2(7, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.3f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.0f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true;
        
        // === AIR NORMALS ===
        airLightKick = CreateAttack("j.LK", 4, 4, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightKick.hitboxOffset = new Vector2(0.7f, -0.2f); airLightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        airMediumKick = CreateAttack("j.MK", 5, 5, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumKick.hitboxOffset = new Vector2(1.1f, -0.3f); airMediumKick.hitboxSize = new Vector2(0.8f, 0.4f);
        airMediumKick.isOverhead = true;
        
        airHeavyKick = CreateAttack("j.HK", 7, 5, 12, 14, 10, 700, 200, 200, new Vector2(7, 0));
        airHeavyKick.hitboxOffset = new Vector2(1.3f, -0.2f); airHeavyKick.hitboxSize = new Vector2(1.0f, 0.4f);
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ATTACKS ===
        // Kakuritsu Ren (6MK) - Two-hit overhead/low mixup
        kakuritsuRen = CreateAttack("Kakuritsu Ren", AttackType.Unique, 16, 6, 14, 18, 10, 800, 250, 250, new Vector2(6, 2));
        kakuritsuRen.hitboxOffset = new Vector2(1.2f, 0.8f); kakuritsuRen.hitboxSize = new Vector2(0.8f, 0.4f);
        kakuritsuRen.isOverhead = true; // First hit overhead
        
        // Yosokyaku (4HK) - Command overhead kick
        yosokyaku = CreateAttack("Yosokyaku", AttackType.Unique, 18, 4, 16, 20, 12, 800, 250, 250, new Vector2(8, 4));
        yosokyaku.hitboxOffset = new Vector2(1.3f, 1.2f); yosokyaku.hitboxSize = new Vector2(0.7f, 0.5f);
        yosokyaku.isOverhead = true; yosokyaku.specialCancellable = true;
        
        // Target combo LP > MP > HP
        targetCombo2 = CreateAttack("LP>MP", AttackType.Normal, 0, 3, 8, 10, 8, 400, 100, 100, new Vector2(4, 0));
        targetCombo3 = CreateAttack("LP>MP>HP", AttackType.Normal, 0, 4, 14, 16, 8, 700, 200, 200, new Vector2(6, 2));
        
        // === SPECIALS ===
        // Kikoken (QCF+P) - 14f startup, slower than hadoken but recovers faster
        kikoken = CreateAttack("Kikoken", AttackType.Special, 14, 2, 24, 12, 8, 400, 150, 300, new Vector2(4, 0));
        kikoken.hitboxOffset = new Vector2(1.3f, 1.1f); kikoken.hitboxSize = new Vector2(0.5f, 0.5f);
        kikoken.isProjectile = true; kikoken.superCancellable = true;
        
        // Spinning Bird Kick (Charge Down, Up + K) - 10f startup, airborne
        spinningBirdKick = CreateAttack("Spinning Bird Kick", AttackType.Special, 10, 20, 20, 16, 6, 1000, 300, 400, new Vector2(6, 6));
        spinningBirdKick.hitboxOffset = new Vector2(0.8f, 1.2f); spinningBirdKick.hitboxSize = new Vector2(1.0f, 1.0f);
        spinningBirdKick.superCancellable = true;
        
        // Lightning Legs (Kick rapidly) - 6f startup, multi-hit
        lightningLegs = CreateAttack("Lightning Legs", AttackType.Special, 6, 30, 18, 14, 6, 1200, 400, 500, new Vector2(4, 2));
        lightningLegs.hitboxOffset = new Vector2(0.8f, 0.5f); lightningLegs.hitboxSize = new Vector2(0.7f, 0.8f);
        lightningLegs.superCancellable = true;
        
        // Hazanshu (QCB+K) - Overhead kick, 15f startup
        hazanshu = CreateAttack("Hazanshu", AttackType.Special, 15, 4, 18, 20, 10, 700, 250, 250, new Vector2(8, 6));
        hazanshu.hitboxOffset = new Vector2(1.2f, 1.3f); hazanshu.hitboxSize = new Vector2(0.7f, 0.5f);
        hazanshu.isOverhead = true; hazanshu.superCancellable = true;
        
        // === SUPER ARTS ===
        // Kikoshou (QCFx2+P) - Giant energy palm
        kikoshou = CreateAttack("Kikoshou", AttackType.Super, 8, 20, 35, 30, 10, 2800, 500, 0, new Vector2(12, 10));
        kikoshou.hitboxOffset = new Vector2(1.0f, 1.0f); kikoshou.hitboxSize = new Vector2(1.5f, 1.5f);
        kikoshou.isInvincible = true; kikoshou.invincibleFrames = 10; kikoshou.launches = true;
        
        // Hoyokusen (QCFx2+K) - Multi-hit kick super
        hoyokusen = CreateAttack("Hoyokusen", AttackType.Super, 6, 40, 30, 30, 10, 3000, 500, 0, new Vector2(10, 8));
        hoyokusen.hitboxOffset = new Vector2(1.2f, 0.8f); hoyokusen.hitboxSize = new Vector2(1.0f, 1.0f);
        hoyokusen.isInvincible = true; hoyokusen.invincibleFrames = 8;
        
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
            hurtboxSize = new Vector2(0.55f, 1.7f)
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
        hurtboxes.Add(new Hitbox { type = HitboxType.Hurtbox, offset = new Vector2(0, 1), size = new Vector2(0.55f, 1.7f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.ThrowHurtbox, offset = new Vector2(0, 1), size = new Vector2(0.7f, 1.7f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.Pushbox, offset = new Vector2(0, 1), size = new Vector2(0.45f, 1.7f), isActive = true });
    }
    
    protected override void GenerateProceduralSprites()
    {
        // Chun-Li - Blue qipao, brown hair, spiked bracelets
    }
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Back : dir == Direction.Forward))
                return yosokyaku; // 4HK
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return kakuritsuRen; // 6MK
            if (crouching) return crouchHeavyKick;
            if (jumping) return airHeavyKick;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
            if (crouching) return crouchMediumKick;
            if (jumping) return airMediumKick;
            return mediumKick;
        }
        if (inputManager.GetButton(playerIndex, Button.Light))
        {
            if (crouching) return crouchLightKick;
            if (jumping) return airLightKick;
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
            case "QCF": special = kikoken; break;
            case "QCB": special = hazanshu; break;
            case "ChargeDownUp": special = spinningBirdKick; break;
        }
        if (special != null) { currentAttack = special; ChangeState(CharacterState.AttackStartup); }
    }
}