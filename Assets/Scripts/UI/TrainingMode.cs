using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Training mode with frame data display, hitbox visualization, and recording
/// </summary>
public class TrainingMode : MonoBehaviour
{
    public static TrainingMode Instance { get; private set; }
    
    [Header("References")]
    public CharacterBase player, dummy;
    public GameManager gameManager;
    
    [Header("UI")]
    public GameObject trainingMenu;
    public Text frameDataText;
    public Text comboText;
    public Toggle showHitboxesToggle;
    public Toggle showFrameDataToggle;
    public Slider dummyHealthSlider;
    public Dropdown dummyBehaviorDropdown;
    
    [Header("Recording")]
    public float maxRecordTime = 10f;
    public Text recordStatusText;
    
    // State
    public bool isTrainingMode = false;
    public bool showHitboxes = false;
    public bool showFrameData = false;
    public bool isRecording = false;
    public bool isPlayingBack = false;
    
    private List<RecordedInput> recording = new List<RecordedInput>();
    private float recordTimer = 0f;
    private int playbackIndex = 0;
    
    private Dictionary<CharacterState, string> stateNames = new Dictionary<CharacterState, string>
    {
        { CharacterState.Idle, "Idle" },
        { CharacterState.WalkForward, "Walk F" },
        { CharacterState.WalkBackward, "Walk B" },
        { CharacterState.Crouch, "Crouch" },
        { CharacterState.JumpStartup, "Jump Start" },
        { CharacterState.JumpRising, "Jump Rise" },
        { CharacterState.JumpPeak, "Jump Peak" },
        { CharacterState.JumpFalling, "Jump Fall" },
        { CharacterState.JumpLanding, "Land" },
        { CharacterState.AttackStartup, "Atk Start" },
        { CharacterState.AttackActive, "Atk Active" },
        { CharacterState.AttackRecovery, "Atk Recov" },
        { CharacterState.HitStun, "Hit Stun" },
        { CharacterState.BlockStun, "Block Stun" },
        { CharacterState.Knockdown, "Down" },
        { CharacterState.GettingUp, "Get Up" },
        { CharacterState.ThrowStartup, "Throw Start" },
        { CharacterState.ThrowActive, "Throw Active" },
        { CharacterState.DriveRush, "Drive Rush" },
        { CharacterState.Parry, "Parry" },
        { CharacterState.DriveImpact, "Drv Impact" },
        { CharacterState.SuperFreeze, "Super Freeze" }
    };
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        if (trainingMenu) trainingMenu.SetActive(false);
        
        if (showHitboxesToggle) showHitboxesToggle.onValueChanged.AddListener(v => showHitboxes = v);
        if (showFrameDataToggle) showFrameDataToggle.onValueChanged.AddListener(v => showFrameData = v);
        if (dummyHealthSlider) dummyHealthSlider.onValueChanged.AddListener(SetDummyHealth);
        if (dummyBehaviorDropdown) dummyBehaviorDropdown.onValueChanged.AddListener(SetDummyBehavior);
    }
    
    private void Update()
    {
        if (!isTrainingMode) return;
        
        HandleTrainingInputs();
        UpdateFrameDataDisplay();
        UpdateRecording();
        UpdatePlayback();
    }
    
    private void HandleTrainingInputs()
    {
        // Toggle menu (Tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (trainingMenu) trainingMenu.SetActive(!trainingMenu.activeSelf);
            Time.timeScale = trainingMenu.activeSelf ? 0f : 1f;
        }
        
        // Reset positions (R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPositions();
        }
        
        // Toggle hitboxes (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            showHitboxes = !showHitboxes;
            if (showHitboxesToggle) showHitboxesToggle.isOn = showHitboxes;
        }
        
        // Toggle frame data (F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            showFrameData = !showFrameData;
            if (showFrameDataToggle) showFrameDataToggle.isOn = showFrameData;
        }
        
        // Record (Left Ctrl)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartRecording();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopRecording();
        }
        
        // Playback (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartPlayback();
        }
        
        // Step frame (Period)
        if (Input.GetKeyDown(KeyCode.Period))
        {
            StepFrame();
        }
    }
    
    private void UpdateFrameDataDisplay()
    {
        if (!showFrameData || frameDataText == null) return;
        
        string info = "";
        
        // Player info
        info += "=== PLAYER ===\n";
        info += $"State: {GetStateName(player.currentState)} (Frame: {player.stateFrame})\n";
        info += $"Health: {player.currentHealth}/{player.maxHealth}\n";
        info += $"Drive: {player.currentDriveGauge}/{player.maxDriveGauge}\n";
        info += $"Super: {player.currentSuperMeter}/{player.maxSuperMeter}\n";
        info += $"Pos: {player.transform.position.x:F1}, {player.transform.position.y:F1}\n";
        info += $"Vel: {player.rb.linearVelocity.x:F1}, {player.rb.linearVelocity.y:F1}\n";
        info += $"Grounded: {player.isGrounded} | Facing: {(player.facingRight ? "Right" : "Left")}\n";
        
        if (player.currentAttack != null)
        {
            info += $"\nCurrent Move: {player.currentAttack.moveName}\n";
            info += $"  Frame: {player.attackFrame}/{player.currentAttack.TotalFrames}\n";
            info += $"  Startup: {player.currentAttack.startupFrames} | Active: {player.currentAttack.activeFrames} | Recov: {player.currentAttack.recoveryFrames}\n";
            info += $"  On Hit: +{player.currentAttack.AdvantageOnHit} | On Block: {player.currentAttack.AdvantageOnBlock}\n";
            info += $"  Damage: {player.currentAttack.damage} | Cancel: {player.currentAttack.specialCancellable}\n";
        }
        
        info += $"\nCombo: {player.comboCount} hits | Scaling: {player.comboDamageScaling:P0}\n";
        
        // Dummy info
        info += "\n=== DUMMY ===\n";
        info += $"State: {GetStateName(dummy.currentState)} (Frame: {dummy.stateFrame})\n";
        info += $"Health: {dummy.currentHealth}/{dummy.maxHealth}\n";
        info += $"Hitstun: {dummy.hitStunRemaining} | Blockstun: {dummy.blockStunRemaining}\n";
        
        // Frame advantage calc
        if (player.currentAttack != null && dummy.blockStunRemaining > 0)
        {
            int frameAdv = dummy.blockStunRemaining - (player.currentAttack.TotalFrames - player.attackFrame);
            info += $"\nFrame Advantage: {frameAdv:+#;-#;0}\n";
        }
        
        frameDataText.text = info;
    }
    
    private string GetStateName(CharacterState state)
    {
        return stateNames.ContainsKey(state) ? stateNames[state] : state.ToString();
    }
    
    private void UpdateRecording()
    {
        if (!isRecording) return;
        
        recordTimer += Time.deltaTime;
        if (recordTimer >= maxRecordTime)
        {
            StopRecording();
            return;
        }
        
        // Record current input state
        var input = InputManager.Instance;
        if (input != null)
        {
            recording.Add(new RecordedInput
            {
                time = recordTimer,
                buttons = new bool[4]
                {
                    input.GetButton(0, Button.Light),
                    input.GetButton(0, Button.Medium),
                    input.GetButton(0, Button.Heavy),
                    input.GetButton(0, Button.Special)
                },
                direction = input.GetDirection(0)
            });
        }
        
        if (recordStatusText) recordStatusText.text = $"RECORDING: {recordTimer:F1}s / {maxRecordTime}s";
    }
    
    private void UpdatePlayback()
    {
        if (!isPlayingBack || recording.Count == 0) return;
        
        if (playbackIndex < recording.Count)
        {
            var input = recording[playbackIndex];
            // Apply to dummy (player 2)
            ApplyInputToDummy(input);
            playbackIndex++;
        }
        else
        {
            StopPlayback();
        }
    }
    
    private void ApplyInputToDummy(RecordedInput input)
    {
        // This would feed inputs to the dummy character
        // For now, just a placeholder
    }
    
    public void StartRecording()
    {
        if (isPlayingBack) return;
        
        isRecording = true;
        recording.Clear();
        recordTimer = 0f;
        if (recordStatusText) recordStatusText.text = "RECORDING...";
    }
    
    public void StopRecording()
    {
        isRecording = false;
        if (recordStatusText) recordStatusText.text = $"Recorded {recording.Count} frames";
    }
    
    public void StartPlayback()
    {
        if (isRecording || recording.Count == 0) return;
        
        isPlayingBack = true;
        playbackIndex = 0;
        if (recordStatusText) recordStatusText.text = "PLAYING BACK...";
    }
    
    public void StopPlayback()
    {
        isPlayingBack = false;
        if (recordStatusText) recordStatusText.text = "Playback complete";
    }
    
    public void StepFrame()
    {
        if (gameManager) gameManager.isPaused = true;
        // Single frame step would require custom time stepping
    }
    
    public void ResetPositions()
    {
        var gm = GameManager.Instance;
        if (gm && gm.p1Spawn && gm.p2Spawn)
        {
            player.ResetCharacter(gm.p1Spawn.position, true);
            dummy.ResetCharacter(gm.p2Spawn.position, false);
        }
    }
    
    public void SetDummyHealth(float value)
    {
        if (dummy)
        {
            dummy.currentHealth = Mathf.RoundToInt(value * dummy.maxHealth);
        }
    }
    
    public void SetDummyBehavior(int index)
    {
        // 0 = Stand, 1 = Block All, 2 = Block Random, 3 = Jump, 4 = Record/Playback
        // Implementation would set dummy AI behavior
    }
    
    public void ToggleTrainingMode()
    {
        isTrainingMode = !isTrainingMode;
        if (trainingMenu) trainingMenu.SetActive(isTrainingMode);
        
        if (isTrainingMode)
        {
            gameManager.isPaused = false;
            gameManager.currentRoundTime = 999f; // Infinite time
            ResetPositions();
        }
    }
    
    private void OnGUI()
    {
        if (!showHitboxes) return;
        
        // Draw hitboxes for both characters
        DrawCharacterHitboxes(player, Color.red);
        DrawCharacterHitboxes(dummy, Color.blue);
    }
    
    private void DrawCharacterHitboxes(CharacterBase character, Color color)
    {
        if (character == null) return;
        
        // Active hitboxes
        foreach (var hb in character.GetActiveHitboxes())
        {
            if (!hb.isActive) continue;
            DrawHitbox(character, hb, Color.red);
        }
        
        // Hurtboxes
        foreach (var hb in character.GetHurtboxes())
        {
            if (!hb.isActive) continue;
            Color c = hb.type == HitboxType.ThrowHurtbox ? Color.green : Color.blue;
            DrawHitbox(character, hb, c);
        }
    }
    
    private void DrawHitbox(CharacterBase character, Hitbox hb, Color color)
    {
        Vector2 center = (Vector2)character.transform.position + 
            new Vector2(hb.offset.x * (character.facingRight ? 1 : -1), hb.offset.y);
        Vector2 size = hb.size;
        
        // Draw in GUI (for debugging)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(center);
        screenPos.y = Screen.height - screenPos.y;
        
        // This is a simplified debug draw - in practice use Gizmos or LineRenderer
    }
    
    private void OnDrawGizmos()
    {
        if (!showHitboxes) return;
        
        if (player) DrawGizmoHitboxes(player, Color.red);
        if (dummy) DrawGizmoHitboxes(dummy, Color.blue);
    }
    
    private void DrawGizmoHitboxes(CharacterBase character, Color baseColor)
    {
        Gizmos.matrix = Matrix4x4.identity;
        
        foreach (var hb in character.GetActiveHitboxes())
        {
            if (!hb.isActive) continue;
            Gizmos.color = Color.red;
            DrawGizmoBox(character, hb);
        }
        
        foreach (var hb in character.GetHurtboxes())
        {
            if (!hb.isActive) continue;
            Gizmos.color = hb.type == HitboxType.ThrowHurtbox ? Color.green : Color.blue;
            DrawGizmoBox(character, hb);
        }
    }
    
    private void DrawGizmoBox(CharacterBase character, Hitbox hb)
    {
        Vector3 center = character.transform.position + 
            new Vector3(hb.offset.x * (character.facingRight ? 1 : -1), hb.offset.y, 0);
        Vector3 size = new Vector3(hb.size.x, hb.size.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}

[System.Serializable]
public class RecordedInput
{
    public float time;
    public bool[] buttons;
    public Direction direction;
}