using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all hitbox/hurtbox collision detection
/// </summary>
public class HitboxSystem : MonoBehaviour
{
    public static HitboxSystem Instance { get; private set; }
    
    private List<CharacterBase> characters = new List<CharacterBase>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    
    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;
        
        // Find all characters
        characters.Clear();
        characters.AddRange(FindObjectsOfType<CharacterBase>());
        
        // Check all pair combinations
        for (int i = 0; i < characters.Count; i++)
        {
            for (int j = i + 1; j < characters.Count; j++)
            {
                CheckCollision(characters[i], characters[j]);
            }
        }
    }
    
    private void CheckCollision(CharacterBase attacker, CharacterBase defender)
    {
        var hitboxes = attacker.GetActiveHitboxes();
        var hurtboxes = defender.GetHurtboxes();
        
        foreach (var hb in hitboxes)
        {
            if (!hb.isActive) continue;
            
            foreach (var hurt in hurtboxes)
            {
                if (!hurt.isActive) continue;
                
                // Skip throw hurtboxes for non-throw attacks
                if (hurt.type == HitboxType.ThrowHurtbox && hb.type != HitboxType.GrabBox) continue;
                // Skip regular hurtboxes for throws
                if (hurt.type == HitboxType.Hurtbox && hb.type == HitboxType.GrabBox) continue;
                
                if (CheckOverlap(attacker, hb, defender, hurt))
                {
                    ProcessHit(attacker, defender, hb, hurt);
                    return; // Only one hit per frame per attacker
                }
            }
        }
    }
    
    private bool CheckOverlap(CharacterBase attacker, Hitbox hb, CharacterBase defender, Hitbox hurt)
    {
        Vector2 hbCenter = (Vector2)attacker.transform.position + 
            new Vector2(hb.offset.x * (attacker.facingRight ? 1 : -1), hb.offset.y);
        Vector2 hurtCenter = (Vector2)defender.transform.position + 
            new Vector2(hurt.offset.x * (defender.facingRight ? 1 : -1), hurt.offset.y);
        
        Vector2 hbSize = hb.size;
        Vector2 hurtSize = hurt.size;
        
        // AABB check
        return Mathf.Abs(hbCenter.x - hurtCenter.x) < (hbSize.x + hurtSize.x) * 0.5f &&
               Mathf.Abs(hbCenter.y - hurtCenter.y) < (hbSize.y + hurtSize.y) * 0.5f;
    }
    
    private void ProcessHit(CharacterBase attacker, CharacterBase defender, Hitbox hb, Hitbox hurt)
    {
        AttackData attack = hb.attackData;
        if (attack == null) return;
        
        // Check invincibility
        if (defender.currentState == CharacterState.Parry)
        {
            // Parry logic handled in defender
            var result = new HitResult
            {
                hit = true,
                attack = attack,
                hitbox = hb,
                hurtbox = hurt,
                contactPoint = (Vector2)attacker.transform.position + hb.offset,
                damageDealt = 0
            };
            defender.OnHit(result); // Will trigger parry
            return;
        }
        
        if (defender.currentState == CharacterState.DriveImpact && defender.stateFrame < 20)
        {
            // Armor
            var result = new HitResult
            {
                hit = true,
                attack = attack,
                hitbox = hb,
                hurtbox = hurt,
                contactPoint = (Vector2)attacker.transform.position + hb.offset,
                damageDealt = 0
            };
            defender.OnHit(result); // Will trigger armor
            return;
        }
        
        // Check counter hit (attacker in startup/active, defender in attack)
        bool counterHit = IsCounterHitState(defender.currentState);
        
        // Check crush counter
        bool crushCounter = attack.crushCounter && counterHit;
        
        // Calculate damage with scaling
        float scaling = attacker.comboDamageScaling;
        int finalDamage = Mathf.RoundToInt(attack.damage * scaling);
        
        // Minimum damage (10%)
        finalDamage = Mathf.Max(finalDamage, Mathf.RoundToInt(attack.damage * 0.1f));
        
        var hitResult = new HitResult
        {
            hit = true,
            counterHit = counterHit,
            crushCounter = crushCounter,
            attack = attack,
            hitbox = hb,
            hurtbox = hurt,
            contactPoint = (Vector2)attacker.transform.position + 
                new Vector2(hb.offset.x * (attacker.facingRight ? 1 : -1), hb.offset.y),
            damageDealt = finalDamage
        };
        
        // Apply hit
        if (attack.isThrow || hb.type == HitboxType.GrabBox)
        {
            defender.OnThrow(hitResult);
        }
        else if (defender.isBlocking && CanBlock(attack, defender))
        {
            defender.OnBlock(hitResult);
            // Attacker gets meter on block
            attacker.GainSuperMeter(attack.superMeterGain / 2);
        }
        else
        {
            defender.OnHit(hitResult);
            // Attacker gets meter on hit
            attacker.GainSuperMeter(attack.superMeterGain);
        }
        
        // Hit effects
        SpawnHitEffect(attack, hitResult.contactPoint, defender.isBlocking);
        
        // Camera shake
        if (attack.screenShake > 0) CameraShake.Shake(attack.screenShake);
    }
    
    private bool IsCounterHitState(CharacterState state)
    {
        return state == CharacterState.AttackStartup || 
               state == CharacterState.AttackActive || 
               state == CharacterState.AttackRecovery ||
               state == CharacterState.ThrowStartup ||
               state == CharacterState.ThrowActive;
    }
    
    private bool CanBlock(AttackData attack, CharacterBase defender)
    {
        // Overheads must be blocked standing
        if (attack.isOverhead && defender.blockLow) return false;
        // Lows must be blocked crouching
        if (attack.isLow && !defender.blockLow) return false;
        // Throws cannot be blocked
        if (attack.isThrow) return false;
        // Unblockable
        if (attack.type == AttackType.DriveImpact && defender.stateFrame > 20) return false; // Late DI
        return true;
    }
    
    private void SpawnHitEffect(AttackData attack, Vector2 position, bool blocked)
    {
        // In a real game, instantiate hit effect prefabs
        // For now, just debug log
        Debug.Log($"Hit: {attack.moveName} at {position} | Blocked: {blocked}");
    }
}

/// <summary>
/// Simple camera shake
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    
    private Vector3 originalPos;
    private float shakeTime = 0f;
    private float shakeIntensity = 0f;
    
    private void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }
    
    private void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }
    
    public static void Shake(float intensity)
    {
        if (Instance != null)
        {
            Instance.shakeIntensity = intensity;
            Instance.shakeTime = 0.1f;
        }
    }
}