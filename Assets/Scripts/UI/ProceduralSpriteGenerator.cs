using UnityEngine;

/// <summary>
/// Generates procedural sprites for characters, effects, and UI
/// </summary>
public static class ProceduralSpriteGenerator
{
    private static Texture2D CreateTexture(int width, int height, System.Func<int, int, Color> pixelFunc)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = pixelFunc(x, y);
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
    
    public static Sprite CreateCharacterSprite(string characterName, CharacterState state, bool facingRight)
    {
        int width = 64;
        int height = 128;
        
        Color bodyColor, accentColor, outlineColor;
        GetCharacterColors(characterName, out bodyColor, out accentColor, out outlineColor);
        
        var tex = CreateTexture(width, height, (x, y) => {
            // Normalized coordinates
            float ux = x / (float)width;
            float uy = y / (float)height;
            
            // Character silhouette (capsule shape)
            float centerX = 0.5f;
            float bodyTop = 0.85f;
            float bodyBottom = 0.15f;
            float headRadius = 0.15f;
            float bodyWidth = 0.3f;
            
            float dist = 0f;
            
            // Head
            if (uy > bodyTop)
            {
                float headCenterY = bodyTop + headRadius;
                dist = Mathf.Sqrt((ux - centerX) * (ux - centerX) + (uy - headCenterY) * (uy - headCenterY));
                if (dist < headRadius) return bodyColor;
            }
            // Body
            else if (uy > bodyBottom)
            {
                if (Mathf.Abs(ux - centerX) < bodyWidth * 0.5f) return bodyColor;
            }
            // Legs
            else
            {
                float legWidth = 0.12f;
                float legGap = 0.05f;
                // Left leg
                if (ux > centerX - legWidth - legGap && ux < centerX - legGap) return bodyColor;
                // Right leg
                if (ux > centerX + legGap && ux < centerX + legWidth + legGap) return bodyColor;
            }
            
            // State-specific overlays
            Color stateColor = GetStateColor(state);
            if (state != CharacterState.Idle && state != CharacterState.WalkForward && state != CharacterState.WalkBackward)
            {
                // Add state indicator
                if (uy > 0.9f && ux > 0.8f)
                {
                    return Color.Lerp(bodyColor, stateColor, 0.5f);
                }
            }
            
            // Outline
            if (IsOutlinePixel(x, y, width, height, centerX, bodyTop, bodyBottom, headRadius, bodyWidth))
            {
                return outlineColor;
            }
            
            return Color.clear;
        });
        
        // Flip if facing left
        if (!facingRight)
        {
            // Note: In Unity, flipping is done via transform.scale.x = -1
            // This is handled at runtime
        }
        
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32);
    }
    
    private static bool IsOutlinePixel(int x, int y, int w, int h, float cx, float bodyTop, float bodyBottom, float headR, float bodyW)
    {
        float ux = x / (float)w;
        float uy = y / (float)h;
        
        // Check neighbors for transparency
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                {
                    float nux = nx / (float)w;
                    float nuy = ny / (float)h;
                    
                    bool current = IsInside(ux, uy, cx, bodyTop, bodyBottom, headR, bodyW);
                    bool neighbor = IsInside(nux, nuy, cx, bodyTop, bodyBottom, headR, bodyW);
                    
                    if (current != neighbor) return true;
                }
            }
        }
        return false;
    }
    
    private static bool IsInside(float ux, float uy, float cx, float bodyTop, float bodyBottom, float headR, float bodyW)
    {
        if (uy > bodyTop)
        {
            float headCenterY = bodyTop + headR;
            float dist = Mathf.Sqrt((ux - cx) * (ux - cx) + (uy - headCenterY) * (uy - headCenterY));
            return dist < headR;
        }
        else if (uy > bodyBottom)
        {
            return Mathf.Abs(ux - cx) < bodyW * 0.5f;
        }
        else
        {
            float legW = 0.12f;
            float legGap = 0.05f;
            return (ux > cx - legW - legGap && ux < cx - legGap) ||
                   (ux > cx + legGap && ux < cx + legW + legGap);
        }
    }
    
    private static void GetCharacterColors(string name, out Color body, out Color accent, out Color outline)
    {
        switch (name.ToLower())
        {
            case "ryu":
                body = new Color(0.9f, 0.9f, 0.9f); // White gi
                accent = new Color(0.8f, 0.1f, 0.1f); // Red headband
                outline = new Color(0.2f, 0.2f, 0.2f);
                break;
            case "chun-li":
                body = new Color(0.2f, 0.4f, 0.8f); // Blue qipao
                accent = new Color(0.9f, 0.7f, 0.2f); // Gold accents
                outline = new Color(0.1f, 0.1f, 0.3f);
                break;
            case "zangief":
                body = new Color(0.9f, 0.6f, 0.4f); // Red skin
                accent = new Color(0.2f, 0.5f, 0.2f); // Green pants
                outline = new Color(0.4f, 0.2f, 0.1f);
                break;
            case "guile":
                body = new Color(0.3f, 0.5f, 0.3f); // Green military
                accent = new Color(0.9f, 0.8f, 0.3f); // Blonde hair
                outline = new Color(0.1f, 0.3f, 0.1f);
                break;
            case "luke":
                body = new Color(0.2f, 0.5f, 0.9f); // Blue MMA
                accent = new Color(0.9f, 0.8f, 0.2f); // Yellow/Gold
                outline = new Color(0.1f, 0.2f, 0.4f);
                break;
            case "jp":
                body = new Color(0.3f, 0.1f, 0.4f); // Purple suit
                accent = new Color(0.8f, 0.6f, 0.9f); // Pink/Purple
                outline = new Color(0.15f, 0.05f, 0.2f);
                break;
            default:
                body = Color.white;
                accent = Color.red;
                outline = Color.black;
                break;
        }
    }
    
    private static Color GetStateColor(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.AttackStartup: return Color.yellow;
            case CharacterState.AttackActive: return Color.red;
            case CharacterState.AttackRecovery: return Color.orange;
            case CharacterState.HitStun: return Color.magenta;
            case CharacterState.BlockStun: return Color.cyan;
            case CharacterState.DriveRush: return new Color(0, 1, 1); // Cyan
            case CharacterState.Parry: return new Color(1, 1, 0); // Yellow
            case CharacterState.DriveImpact: return new Color(1, 0.5f, 0); // Orange
            case CharacterState.SuperFreeze: return new Color(1, 0, 1); // Magenta
            default: return Color.white;
        }
    }
    
    public static Sprite CreateProjectileSprite(string type, Color color)
    {
        int size = 32;
        var tex = CreateTexture(size, size, (x, y) => {
            float ux = (x - size * 0.5f) / (size * 0.5f);
            float uy = (y - size * 0.5f) / (size * 0.5f);
            float dist = Mathf.Sqrt(ux * ux + uy * uy);
            
            if (dist < 1f)
            {
                float alpha = 1f - dist;
                return new Color(color.r, color.g, color.b, alpha);
            }
            return Color.clear;
        });
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
    }
    
    public static Sprite CreateHitEffectSprite(HitEffectType type)
    {
        int size = 64;
        Color c = type == HitEffectType.Hit ? Color.red : 
                  type == HitEffectType.Block ? Color.blue : Color.yellow;
        
        var tex = CreateTexture(size, size, (x, y) => {
            float ux = (x - size * 0.5f) / (size * 0.5f);
            float uy = (y - size * 0.5f) / (size * 0.5f);
            float dist = Mathf.Sqrt(ux * ux + uy * uy);
            
            if (dist < 1f)
            {
                // Star burst pattern
                float angle = Mathf.Atan2(uy, ux);
                float spikes = Mathf.Sin(angle * 8 + Time.time * 20) * 0.1f;
                float alpha = (1f - dist + spikes) * 0.8f;
                return new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
            }
            return Color.clear;
        });
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
    }
    
    public static Sprite CreateUISprite(UISpriteType type, Color color)
    {
        switch (type)
        {
            case UISpriteType.HealthBar:
                return CreateBarSprite(200, 20, color, Color.black);
            case UISpriteType.DriveBar:
                return CreateSegmentedBarSprite(200, 15, 6, color, Color.gray);
            case UISpriteType.SuperBar:
                return CreateSegmentedBarSprite(150, 12, 3, color, Color.gray);
            case UISpriteType.Portrait:
                return CreatePortraitFrame(128, 128, color);
            default:
                return CreateBarSprite(100, 10, color, Color.black);
        }
    }
    
    private static Sprite CreateBarSprite(int w, int h, Color fill, Color bg)
    {
        var tex = CreateTexture(w, h, (x, y) => {
            float ux = x / (float)w;
            if (ux < 0.02f || ux > 0.98f || y < 1 || y > h - 2) return bg;
            return fill;
        });
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0, 0.5f), 100);
    }
    
    private static Sprite CreateSegmentedBarSprite(int w, int h, int segments, Color fill, Color empty)
    {
        var tex = CreateTexture(w, h, (x, y) => {
            float ux = x / (float)w;
            int seg = Mathf.FloorToInt(ux * segments);
            float segUx = (ux * segments) % 1f;
            
            if (segUx < 0.05f || segUx > 0.95f || y < 1 || y > h - 2) return Color.black;
            
            // Alternate filled/empty based on segment index
            // This would be controlled at runtime via material
            return fill;
        });
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0, 0.5f), 100);
    }
    
    private static Sprite CreatePortraitFrame(int w, int h, Color border)
    {
        var tex = CreateTexture(w, h, (x, y) => {
            if (x < 3 || x > w - 4 || y < 3 || y > h - 4) return border;
            return new Color(0.1f, 0.1f, 0.15f);
        });
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100);
    }
}

public enum HitEffectType
{
    Hit,
    Block,
    Counter
}

public enum UISpriteType
{
    HealthBar,
    DriveBar,
    SuperBar,
    Portrait
}