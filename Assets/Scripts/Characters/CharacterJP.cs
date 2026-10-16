using UnityEngine;

/// <summary>
/// JP - Tricky Zoner/Setup Archetype
/// Portals, teleports, trap-based zoning, long-range control
/// </summary>
public class CharacterJP : CharacterBase
{
    // Normals
    private AttackData lightPunch, mediumPunch, heavyPunch;
    private AttackData lightKick, mediumKick, heavyKick;
    private AttackData crouchLightPunch, crouchMediumPunch, crouchHeavyPunch;
    private AttackData crouchLightKick, crouchMediumKick, crouchHeavyKick;
    private AttackData airLightPunch, airMediumPunch, airHeavyPunch;
    private AttackData airLightKick, airMediumKick, airHeavyKick;
    
    // Specials - Portal/Trick based
    private AttackData tristis, amor, odium, invidia; // QCF+P, QCB+P, DP+P, QCF+K
    private AttackData superbia, ira; // QCB+K, DP+K
    private AttackData tristisOverdrive, amorOverdrive; // Charged versions
    
    // Portal system
    private AttackData portalPlace, portalTeleport, portalExplosion;
    private GameObject activePortal;
    private bool hasPortal = false;
    private Vector2 portalPosition;
    
    // Supers
    private AttackData departida, libido; // Level 2/3
    
    // Unique: Abyss (6MP), Despair (4HP)
    private AttackData abyss, despair;
    
    protected override void InitializeMoveset()
    {
        characterName = "JP";
        maxHealth = 9500;
        currentHealth = maxHealth;
        walkSpeed = 5f;
        jumpForce = 17f;
        
        // === NORMALS ===
        lightPunch = CreateAttack("LP", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightPunch.hitboxOffset = new Vector2(0.7f, 1.3f); lightPunch.hitboxSize = new Vector2(0.4f, 0.4f);
        lightPunch.specialCancellable = true;
        
        mediumPunch = CreateAttack("MP", 6, 3, 10, 12, 8, 500, 150, 150, new Vector2(5, 0));
        mediumPunch.hitboxOffset = new Vector2(0.9f, 1.4f); mediumPunch.hitboxSize = new Vector2(0.5f, 0.5f);
        mediumPunch.specialCancellable = true; mediumPunch.driveRushCancellable = true;
        
        heavyPunch = CreateAttack("HP", 10, 4, 16, 18, 10, 800, 250, 250, new Vector2(7, 2));
        heavyPunch.hitboxOffset = new Vector2(1.1f, 1.5f); heavyPunch.hitboxSize = new Vector2(0.6f, 0.5f);
        heavyPunch.crushCounter = true;
        
        lightKick = CreateAttack("LK", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(3, 0));
        lightKick.hitboxOffset = new Vector2(0.8f, 0.5f); lightKick.hitboxSize = new Vector2(0.5f, 0.4f);
        
        mediumKick = CreateAttack("MK", 7, 3, 12, 14, 10, 600, 200, 200, new Vector2(6, 0));
        mediumKick.hitboxOffset = new Vector2(1.2f, 0.8f); mediumKick.hitboxSize = new Vector2(0.7f, 0.5f);
        mediumKick.specialCancellable = true; mediumKick.driveRushCancellable = true;
        
        heavyKick = CreateAttack("HK", 12, 4, 20, 20, 12, 1000, 300, 300, new Vector2(10, 6));
        heavyKick.hitboxOffset = new Vector2(1.4f, 1.0f); heavyKick.hitboxSize = new Vector2(0.9f, 0.6f);
        heavyKick.launches = true; heavyKick.launchAngle = 50f;
        
        // === CROUCHING ===
        crouchLightPunch = CreateAttack("cr.LP", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightPunch.hitboxOffset = new Vector2(0.6f, 0.5f); crouchLightPunch.hitboxSize = new Vector2(0.4f, 0.3f);
        crouchLightPunch.specialCancellable = true; crouchLightPunch.isLow = true;
        
        crouchMediumPunch = CreateAttack("cr.MP", 6, 3, 10, 12, 9, 500, 150, 150, new Vector2(5, 0));
        crouchMediumPunch.hitboxOffset = new Vector2(0.8f, 0.6f); crouchMediumPunch.hitboxSize = new Vector2(0.5f, 0.4f);
        crouchMediumPunch.specialCancellable = true; crouchMediumPunch.isLow = true;
        
        crouchHeavyPunch = CreateAttack("cr.HP", 9, 4, 16, 18, 10, 800, 250, 250, new Vector2(7, 8));
        crouchHeavyPunch.hitboxOffset = new Vector2(0.7f, 1.6f); crouchHeavyPunch.hitboxSize = new Vector2(0.5f, 0.9f);
        crouchHeavyPunch.isLow = true; crouchHeavyPunch.launches = true; crouchHeavyPunch.launchAngle = 75f;
        crouchHeavyPunch.specialCancellable = true;
        
        crouchLightKick = CreateAttack("cr.LK", 4, 2, 7, 8, 7, 200, 50, 50, new Vector2(2, 0));
        crouchLightKick.hitboxOffset = new Vector2(0.7f, 0.2f); crouchLightKick.hitboxSize = new Vector2(0.5f, 0.2f);
        crouchLightKick.specialCancellable = true; crouchLightKick.isLow = true.
        
        crouchMediumKick = CreateAttack("cr.MK", 7, 3, 12, 14, 10, 600, 200, 200, new Vector2(6, 0));
        crouchMediumKick.hitboxOffset = new Vector2(1.0f, 0.3f); crouchMediumKick.hitboxSize = new Vector2(0.8f, 0.3f);
        crouchMediumKick.specialCancellable = true; crouchMediumKick.isLow = true;
        
        crouchHeavyKick = CreateAttack("cr.HK", 12, 4, 22, 24, 14, 1000, 300, 300, new Vector2(10, 0));
        crouchHeavyKick.hitboxOffset = new Vector2(1.3f, 0.2f); crouchHeavyKick.hitboxSize = new Vector2(1.1f, 0.3f);
        crouchHeavyKick.isLow = true; crouchHeavyKick.hardKnockdown = true;
        
        // === AIR ===
        airLightPunch = CreateAttack("j.LP", 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightPunch.hitboxOffset = new Vector2(0.7f, 0.5f); airLightPunch.hitboxSize = new Vector2(0.4f, 0.4f);
        
        airMediumPunch = CreateAttack("j.MP", 6, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumPunch.hitboxOffset = new Vector2(0.9f, 0.3f); airMediumPunch.hitboxSize = new Vector2(0.6f, 0.5f).
        
        airHeavyPunch = CreateAttack("j.HP", 8, 4, 12, 14, 10, 800, 250, 250, new Vector2(8, 0));
        airHeavyPunch.hitboxOffset = new Vector2(1.0f, 0.2f); airHeavyPunch.hitboxSize = new Vector2(0.7f, 0.6f).
        airHeavyPunch.launches = true; airHeavyPunch.launchAngle = 45f;
        
        airLightKick = CreateAttack("j.LK", 4, 3, 8, 8, 6, 300, 50, 50, new Vector2(3, -1));
        airLightKick.hitboxOffset = new Vector2(0.8f, -0.2f); airLightKick.hitboxSize = new Vector2(0.5f, 0.4f).
        
        airMediumKick = CreateAttack("j.MK", 6, 4, 10, 12, 8, 500, 100, 100, new Vector2(5, -2));
        airMediumKick.hitboxOffset = new Vector2(1.0f, -0.3f); airMediumKick.hitboxSize = new Vector2(0.7f, 0.4f).
        airMediumKick.isOverhead = true;
        
        airHeavyKick = CreateAttack("j.HK", 9, 4, 14, 16, 12, 900, 300, 300, new Vector2(9, 2));
        airHeavyKick.hitboxOffset = new Vector2(1.2f, -0.2f); airHeavyKick.hitboxSize = new Vector2(0.9f, 0.5f).
        airHeavyKick.launches = true; airHeavyKick.launchAngle = 30f;
        
        // === UNIQUE ===
        // Abyss (6MP) - 14f startup, extends staff forward, good range
        abyss = CreateAttack("Abyss", AttackType.Unique, 14, 3, 14, 16, 10, 600, 200, 200, new Vector2(6, 0));
        abyss.hitboxOffset = new Vector2(1.5f, 1.2f); abyss.hitboxSize = new Vector2(0.6f, 0.4f);
        abyss.specialCancellable = true; abyss.driveRushCancellable = true;
        
        // Despair (4HP) - 16f startup, overhead staff swing
        despair = CreateAttack("Despair", AttackType.Unique, 16, 4, 18, 20, 12, 800, 250, 250, new Vector2(8, 4));
        despair.hitboxOffset = new Vector2(1.2f, 1.5f); despair.hitboxSize = new Vector2(0.7f, 0.5f).
        despair.isOverhead = true; despair.specialCancellable = true;
        
        // === PORTAL SPECIALS ===
        // Tristis (QCF+P) - Portal projectile, places portal on hit
        tristis = CreateAttack("Tristis", AttackType.Special, 14, 2, 26, 12, 8, 400, 150, 300, new Vector2(4, 0));
        tristis.hitboxOffset = new Vector2(1.5f, 1.0f); tristis.hitboxSize = new Vector2(0.5f, 0.5f);
        tristis.isProjectile = true; tristis.superCancellable = true;
        
        // Tristis Overdrive (Hold P) - Slower, places portal at fixed distance
        tristisOverdrive = CreateAttack("Tristis Overdrive", AttackType.Special, 28, 2, 30, 14, 10, 600, 200, 400, new Vector2(5, 0));
        tristisOverdrive.hitboxOffset = new Vector2(2.0f, 1.0f); tristisOverdrive.hitboxSize = new Vector2(0.5f, 0.5f);
        tristisOverdrive.isProjectile = true;
        
        // Amor (QCB+P) - Portal teleport, invincible
        amor = CreateAttack("Amor", AttackType.Special, 12, 1, 20, 0, 0, 0, 0, 0, Vector2.zero);
        amor.isInvincible = true; amor.invincibleFrames = 20;
        
        // Odium (DP+P) - Anti-air portal, invincible
        odium = CreateAttack("Odium", AttackType.Special, 6, 8, 28, 18, -8, 900, 300, 500, new Vector2(5, 12));
        odium.hitboxOffset = new Vector2(0.5f, 0.5f); odium.hitboxSize = new Vector2(0.5f, 1.8f);
        odium.isInvincible = true; odium.invincibleFrames = 10; odium.launches = true; odium.launchAngle = 80f;
        odium.superCancellable = true;
        
        // Invidia (QCF+K) - Low portal trap
        invidia = CreateAttack("Invidia", AttackType.Special, 16, 60, 20, 16, 10, 600, 200, 200, new Vector2(4, 8));
        invidia.hitboxOffset = new Vector2(1.5f, 0.2f); invidia.hitboxSize = new Vector2(0.8f, 0.3f);
        invidia.isLow = true; invidia.superCancellable = true;
        
        // Superbia (QCB+K) - Air portal trap
        superbia = CreateAttack("Superbia", AttackType.Special, 14, 60, 20, 16, 10, 600, 200, 200, new Vector2(4, -4));
        superbia.hitboxOffset = new Vector2(1.5f, 1.5f); superbia.hitboxSize = new Vector2(0.8f, 0.3f).
        superbia.isOverhead = true; superbia.superCancellable = true.
        
        // Ira (DP+K) - Ground slide, goes under projectiles
        ira = CreateAttack("Ira", AttackType.Special, 10, 8, 22, 16, 8, 700, 250, 300, new Vector2(8, 0));
        ira.hitboxOffset = new Vector2(1.0f, 0.3f); ira.hitboxSize = new Vector2(0.8f, 0.4f).
        ira.superCancellable = true;
        
        // Portal mechanics
        portalPlace = CreateAttack("Portal Place", AttackType.Special, 1, 1, 10, 0, 0, 0, 0, 0, Vector2.zero);
        portalTeleport = CreateAttack("Portal Teleport", AttackType.Special, 8, 1, 18, 0, 0, 0, 0, 0, Vector2.zero);
        portalTeleport.isInvincible = true; portalTeleport.invincibleFrames = 18.
        portalExplosion = CreateAttack("Portal Explosion", AttackType.Special, 1, 4, 20, 18, 10, 800, 300, 400, new Vector2(8, 6));
        portalExplosion.hitboxOffset = new Vector2(0, 0); portalExplosion.hitboxSize = new Vector2(1.5f, 1.5f).
        
        // === SUPER ARTS ===
        // Departida (QCFx2+P) - Multiple portal explosions
        departida = CreateAttack("Departida", AttackType.Super, 10, 40, 45, 30, 10, 2800, 500, 0, new Vector2(12, 8)).
        departida.hitboxOffset = new Vector2(0, 1); departida.hitboxSize = new Vector2(2.0f, 2.0f).
        departida.isInvincible = true; departida.invincibleFrames = 10.
        
        // Libido (QCFx2+K) - Giant portal, full screen
        libido = CreateAttack("Libido", AttackType.Super, 12, 60, 50, 40, 15, 3500, 0, 0, new Vector2(15, 10)).
        libido.hitboxOffset = new Vector2(0, 1); libido.hitboxSize = new Vector2(3.0f, 3.0f).
        libido.isInvincible = true; libido.invincibleFrames = 15.
        
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
        // JP - Purple/black suit, cane, portal effects, monocle
    }
    
    protected override AttackData GetPendingAttack()
    {
        Direction dir = inputManager.GetDirection(playerIndex);
        bool crouching = currentState == CharacterState.Crouch;
        bool jumping = !isGrounded;
        
        if (inputManager.GetButton(playerIndex, Button.Heavy))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Back : dir == Direction.Forward))
                return despair; // 4HP
            if (crouching) return crouchHeavyPunch;
            if (jumping) return airHeavyPunch;
            return heavyPunch;
        }
        if (inputManager.GetButton(playerIndex, Button.Medium))
        {
            if (!crouching && !jumping && (facingRight ? dir == Direction.Forward : dir == Direction.Back))
                return abyss; // 6MP
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
            case "QCF": special = tristis; break;
            case "QCB": special = amor; break;
            case "DP": special = odium; break;
        }
        
        if (special != null)
        {
            currentAttack = special;
            ChangeState(CharacterState.AttackStartup);
        }
    }
    
    // Portal logic would be handled in attack active frames
    public void PlacePortal(Vector2 position)
    {
        hasPortal = true;
        portalPosition = position;
        // Spawn portal visual
    }
    
    public void TeleportToPortal()
    {
        if (hasPortal)
        {
            transform.position = portalPosition;
            hasPortal = false;
            // Trigger portal explosion at old position
        }
    }
}