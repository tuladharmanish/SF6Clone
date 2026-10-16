using UnityEngine;

/// <summary>
/// ZANGIEF - Grappler Archetype
/// Command grabs, armor, lariat, high health, slow but devastating
/// </summary>
public class CharacterZangief : CharacterBase
{
    // Normals
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightPunch, crouchMediumPunch, crouchHeavyPunch;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airHeavyPunch, airHeavyKick;
    
    // Specials - Command Grabs
    private AttackData screwPiledriver, borschtDynamite, cycloneLariat;
    private AttackData siberianExpress, siberianBlizzard; // Level 2/3 supers
    
    // Unique: Double Lariat (PPP/KKK), Banishing Flat (Green Hand)
    private AttackData doubleLariatP, doubleLariatK, banishingFlat;
    
    // V-Skill: Tundra Storm (armor)
    private AttackData tundraStorm;
    
    protected override void InitializeMoveset()
    {
        characterName = "Zangief";
        maxHealth = 12000; // Higher health
        currentHealth = maxHealth;
        walkSpeed = 3.5f; // Slowest walk
        jumpForce = 16f; // Lower jump
        
        // === NORMALS ===
        lightPunch = CreateAttack("LP", 4, 2, 8, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightPunch.hitboxOffset = new Vector2(0.7f, 1.4f); lightPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        lightPunch.specialCancellable = true;
        
        mediumPunch = CreateAttack("MP", 6, 3, 12, 12, 8, 500, 150, 150, new Vector2(6, 0));
        mediumPunch.hitboxOffset = new Vector2(1.0f, 1.5f); mediumPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        mediumPunch.driveRushCancellable = true;
        
        heavyPunch = CreateAttack("HP", 12, 4, 20, 20, 10, 1000, 300, 300, new Vector2(10, 4));
        heavyPunch.hitboxOffset = new Vector2(1.3f, 1.6f); heavyPunch.hitboxSize = new Vector2(0.8f, 0.6f);
        heavyPunch.crushCounter = true; heavyPunch.armor = true; heavyPunch.armorHits = 1;
        
        lightKick = CreateAttack("LK", 4, 2, 8, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightKick.hitboxOffset = new Vector2(0.8f, 0.5f); lightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        mediumKick = CreateAttack("MK", 7, 3, 14, 14, 10, 600, 200, 200, new Vector2(7, 0));
        mediumKick.hitboxOffset = new Vector2(1.2f, 0.8f); mediumKick.hitboxSize = new Vector2(0.7f, 0.5f);
        
        heavyKick = CreateAttack("HK", 14, 5, 24, 22, 12, 1200, 400, 400, new Vector2(12, 8));
        heavyKick.hitboxOffset = new Vector2(1.5f, 1.0f); heavyKick.hitboxSize = new Vector2(1.0f, 0.6f);
        heavyKick.launches = true; heavyKick.launchAngle = 45f; heavyKick.crushCounter = true;
        
        // === CROUCHING ===
        crouchLightPunch = CreateAttack("cr.LP", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightPunch.hitboxOffset = new Vector2(0.6f, 0.6f); crouchLightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        crouchLightPunch.isLow = true;
        
        crouchMediumPunch = CreateAttack("cr.MP", 6, 3, 10, 11, 8, 400, 100, 100, new Vector2(4, 0));
        crouchMediumPunch.hitboxOffset = new Vector2(0.9f, 0.7f); crouchMediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        crouchMediumPunch.isLow = true;
        
        crouchHeavyPunch = CreateAttack("cr.HP", 10, 4, 18, 18, 10, 800, 250, 250, new Vector2(8, 8));
        crouchHeavyPunch.hitboxOffset = new Vector2(0.8f, 1.8f); crouchHeavyPunch.hitboxSize = new Vector2(0.6f, 1.0f);
        crouchHeavyPunch.isLow = true; crouchHeavyPunch.launches = true; crouchHeavyPunch.launchAngle = 70f;
        crouchHeavyPunch.armor = true; crouchHeavyPunch.armorHits = 1;
        
        crouchLightKick = CreateAttack("cr.LK", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.7f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.5f, 0.2f);
        crouchLightKick.isLow = true;
        
        crouchMediumKick = CreateAttack("cr.MK", 7, 3, 12, 12, 9, 500, 150, 150, new Vector2(5, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.1f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.8f, 0.3f);
        crouchMediumKick.isLow = true;
        
        crouchHeavyKick = CreateAttack("cr.HK", 12, 4, 20, 22, 12, 1000, 300, 300, new Vector2(10, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.4f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.2f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true;
        
        // === AIR ===
        airHeavyPunch = CreateAttack("j.HP", 10, 6, 16, 18, 10, 900, 250, 250, new Vector2(10, 4));
        airHeavyPunch.hitboxOffset = new Vector2(1.0f, 0.0f); airHeavyPunch.hitboxSize = new Vector2(0.8f, 0.6f);
        airHeavyPunch.launches = true; airHeavyPunch.launchAngle = 40f;
        
        airHeavyKick = CreateAttack("j.HK", 10, 6, 16, 18, 10, 900, 250, 250, new Vector2(10, 2));
        airHeavyKick.hitboxOffset = new Vector2(1.3f, -0.2f); airHeavyKick.hitboxSize = new Vector2(1.0f, 0.5f);
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ===
        // Double Lariat (LP+MP+HP or LK+MK+HK) - 5f startup, full body armor, 360 hitbox
        doubleLariatP = CreateAttack("Double Lariat (P)", AttackType.Unique, 5, 40, 30, 20, 10, 1500, 500, 500, new Vector2(8, 6));
        doubleLariatP.hitboxOffset = new Vector2(0, 1.2f); doubleLariatP.hitboxSize = new Vector2(2.0f, 2.0f);
        doubleLariatP.armor = true; doubleLariatP.armorHits = 3;
        
        doubleLariatK = CreateAttack("Double Lariat (K)", AttackType.Unique, 5, 40, 30, 20, 10, 1500, 500, 500, new Vector2(8, 6));
        doubleLariatK.hitboxOffset = new Vector2(0, 1.2f); doubleLariatK.hitboxSize = new Vector2(2.0f, 2.0f);
        doubleLariatK.armor = true; doubleLariatK.armorHits = 3;
        
        // Banishing Flat / Green Hand (QCF+P) - 18f startup, projectile invincible, advances
        banishingFlat = CreateAttack("Banishing Flat", AttackType.Special, 18, 4, 22, 20, 10, 800, 250, 250, new Vector2(10, 0));
        banishingFlat.hitboxOffset = new Vector2(1.5f, 1.2f); banishingFlat.hitboxSize = new Vector2(0.8f, 0.6f);
        banishingFlat.superCancellable = true;
        
        // === COMMAND GRABS ===
        // Screw Piledriver (360+P) - 5f startup, 0f active (instant), massive damage
        screwPiledriver = CreateAttack("Screw Piledriver", AttackType.Special, 5, 1, 40, 60, 0, 2200, 500, 500, new Vector2(0, 15));
        screwPiledriver.hitboxOffset = new Vector2(0.8f, 1.0f); screwPiledriver.hitboxSize = new Vector2(0.8f, 1.2f);
        screwPiledriver.isThrow = true; screwPiledriver.hardKnockdown = true;
        
        // Borscht Dynamite (360+K) - 7f startup, longer range, less damage
        borschtDynamite = CreateAttack("Borscht Dynamite", AttackType.Special, 7, 1, 45, 60, 0, 1800, 400, 400, new Vector2(0, 12));
        borschtDynamite.hitboxOffset = new Vector2(1.2f, 1.0f); borschtDynamite.hitboxSize = new Vector2(1.0f, 1.2f);
        borschtDynamite.isThrow = true; borschtDynamite.hardKnockdown = true;
        
        // Cyclone Lariat (QCB+P) - 12f startup, advancing multi-hit
        cycloneLariat = CreateAttack("Cyclone Lariat", AttackType.Special, 12, 24, 28, 18, 8, 1600, 500, 500, new Vector2(10, 6));
        cycloneLariat.hitboxOffset = new Vector2(1.0f, 1.2f); cycloneLariat.hitboxSize = new Vector2(1.2f, 1.2f);
        cycloneLariat.armor = true; cycloneLariat.armorHits = 2; cycloneLariat.superCancellable = true;
        
        // Tundra Storm (V-Skill) - Armor stance
        tundraStorm = CreateAttack("Tundra Storm", AttackType.Unique, 1, 60, 20, 0, 0, 0, 0, 0, Vector2.zero);
        tundraStorm.armor = true; tundraStorm.armorHits = 5;
        
        // === SUPER ARTS ===
        // Siberian Express (360x2+P) - Level 2, 5f startup, cinematic
        siberianExpress = CreateAttack("Siberian Express", AttackType.Super, 5, 1, 60, 80, 0, 3500, 0, 0, new Vector2(0, 20));
        siberianExpress.hitboxOffset = new Vector2(1.0f, 1.0f); siberianExpress.hitboxSize = new Vector2(1.2f, 1.5f);
        siberianExpress.isThrow = true; siberianExpress.isInvincible = true; siberianExpress.invincibleFrames = 10;
        
        // Siberian Blizzard (360x2+K) - Level 3, 7f startup, range
        siberianBlizzard = CreateAttack("Siberian Blizzard", AttackType.Super, 7, 1, 60, 80, 0, 4000, 0, 0, new Vector2(0, 25));
        siberianBlizzard.hitboxOffset = new Vector2(1.5f, 1.0f); siberianBlizzard.hitboxSize = new Vector2(1.5f, 1.5f);
        siberianBlizzard.isThrow = true; siberianBlizzard.isInvincible = true; siberianBlizzard.invincibleFrames = 12;
        
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
            hurtboxOffset = new Vector2(0, 1.2f),
            hurtboxSize = new Vector2(0.8f, 2.0f)
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
        hurtboxes.Add(new Hitbox { type = HitboxType.Hurtbox, offset = new Vector2(0, 1.2f), size = new Vector2(0.8f, 2.0f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.ThrowHurtbox, offset = new Vector2(0, 1.2f), size = new Vector2(1.0f, 2.0f), isActive = true });
        hurtboxes.Add(new Hitbox { type = HitboxType.Pushbox, offset = new Vector2(0, 1.2f), size = new Vector2(0.7f, 2.0f), isActive = true });
    }
    
    protected override void GenerateProceduralSprites()
    {
        // Zangief - Red skin, brown hair, green pants, scars
    }
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (crouching) return crouchHeavyPunch;
            if (jumping) return airHeavyPunch;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
            if (crouching) return crouchMediumPunch;
            return mediumPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Light))
        {
            if (crouching) return crouchLightPunch;
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
            case "360": special = screwPiledriver; break;
            case "QCF": special = banishingFlat; break;
            case "QCB": special = cycloneLariat; break;
        }
        if (special != null) { currentAttack = special; ChangeState(CharacterState.AttackStartup); }
    }
    
    // Override throw to use command grab range
    protected override void TryThrow()
    {
        float dist = Vector2.Distance(transform.position, opponent.transform.position);
        if (dist < 1.8f) // Wider throw range
        {
            currentAttack = screwPiledriver;
            ChangeState(CharacterState.ThrowStartup);
        }
    }
}