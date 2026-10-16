using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles input buffering, motion detection (QCF, DP, etc.), and 4-button scheme
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Settings")]
    public int bufferSize = 60; // 1 second at 60fps
    public int motionWindowFrames = 15;

    [Header("Player 1 Keys")]
    public KeyCode p1Light = KeyCode.J;
    public KeyCode p1Medium = KeyCode.K;
    public KeyCode p1Heavy = KeyCode.L;
    public KeyCode p1Special = KeyCode.Semicolon;
    public KeyCode p1Up = KeyCode.W;
    public KeyCode p1Down = KeyCode.S;
    public KeyCode p1Left = KeyCode.A;
    public KeyCode p1Right = KeyCode.D;

    [Header("Player 2 Keys")]
    public KeyCode p2Light = KeyCode.Keypad4;
    public KeyCode p2Medium = KeyCode.Keypad5;
    public KeyCode p2Heavy = KeyCode.Keypad6;
    public KeyCode p2Special = KeyCode.KeypadPlus;
    public KeyCode p2Up = KeyCode.UpArrow;
    public KeyCode p2Down = KeyCode.DownArrow;
    public KeyCode p2Left = KeyCode.LeftArrow;
    public KeyCode p2Right = KeyCode.RightArrow;

    private Queue<InputEntry>[] inputBuffers = new Queue<InputEntry>[2];
    private FrameInput[] currentInput = new FrameInput[2];
    private FrameInput[] previousInput = new FrameInput[2];
    private int currentFrame = 0;

    // Motion definitions for SF6-style moves
    private static readonly MotionDefinition[] StandardMotions = new MotionDefinition[]
    {
        new MotionDefinition { moveName = "QCF", sequence = new Direction[] { Direction.Down, Direction.DownForward, Direction.Forward }, requiredButton = Button.Special },
        new MotionDefinition { moveName = "QCB", sequence = new Direction[] { Direction.Down, Direction.DownBack, Direction.Back }, requiredButton = Button.Special },
        new MotionDefinition { moveName = "DP", sequence = new Direction[] { Direction.Forward, Direction.Down, Direction.DownForward }, requiredButton = Button.Special },
        new MotionDefinition { moveName = "RDP", sequence = new Direction[] { Direction.Back, Direction.Down, Direction.DownBack }, requiredButton = Button.Special },
        new MotionDefinition { moveName = "HCF", sequence = new Direction[] { Direction.Back, Direction.DownBack, Direction.Down, Direction.DownForward, Direction.Forward }, requiredButton = Button.Special },
        new MotionDefinition { moveName = "ChargeBackForward", sequence = new Direction[] { Direction.Back, Direction.Back, Direction.Forward }, requiredButton = Button.Special, maxFrames = 40 },
        new MotionDefinition { moveName = "ChargeDownUp", sequence = new Direction[] { Direction.Down, Direction.Down, Direction.Up }, requiredButton = Button.Special, maxFrames = 40 },
        new MotionDefinition { moveName = "360", sequence = new Direction[] { Direction.Forward, Direction.DownForward, Direction.Down, Direction.DownBack, Direction.Back, Direction.UpBack, Direction.Up, Direction.UpForward }, requiredButton = Button.Special, maxFrames = 30 },
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < 2; i++)
        {
            inputBuffers[i] = new Queue<InputEntry>(bufferSize);
            currentInput[i] = new FrameInput
            {
                buttons = new bool[4],
                buttonsPrev = new bool[4]
            };
            previousInput[i] = new FrameInput
            {
                buttons = new bool[4],
                buttonsPrev = new bool[4]
            };
        }
    }

    private void Update()
    {
        currentFrame++;
        ReadInputs(0);
        ReadInputs(1);
        UpdateBuffers();
    }

    private void ReadInputs(int playerIndex)
    {
        previousInput[playerIndex] = currentInput[playerIndex].Clone();
        var input = currentInput[playerIndex];

        KeyCode[] buttons = playerIndex == 0 
            ? new KeyCode[] { p1Light, p1Medium, p1Heavy, p1Special }
            : new KeyCode[] { p2Light, p2Medium, p2Heavy, p2Special };
        KeyCode up = playerIndex == 0 ? p1Up : p2Up;
        KeyCode down = playerIndex == 0 ? p1Down : p2Down;
        KeyCode left = playerIndex == 0 ? p1Left : p2Left;
        KeyCode right = playerIndex == 0 ? p1Right : p2Right;

        // Read buttons
        for (int i = 0; i < 4; i++)
        {
            input.buttons[i] = Input.GetKey(buttons[i]);
        }

        // Read direction (digital)
        bool u = Input.GetKey(up);
        bool d = Input.GetKey(down);
        bool l = Input.GetKey(left);
        bool r = Input.GetKey(right);

        if (u && !d)
        {
            if (l && !r) input.direction = Direction.UpBack;
            else if (r && !l) input.direction = Direction.UpForward;
            else input.direction = Direction.Up;
        }
        else if (d && !u)
        {
            if (l && !r) input.direction = Direction.DownBack;
            else if (r && !l) input.direction = Direction.DownForward;
            else input.direction = Direction.Down;
        }
        else
        {
            if (l && !r) input.direction = Direction.Back;
            else if (r && !l) input.direction = Direction.Forward;
            else input.direction = Direction.Neutral;
        }

        currentInput[playerIndex] = input;
    }

    private void UpdateBuffers()
    {
        for (int p = 0; p < 2; p++)
        {
            var buffer = inputBuffers[p];
            var input = currentInput[p];
            var prev = previousInput[p];

            // Add directional input
            if (input.direction != prev.direction)
            {
                buffer.Enqueue(new InputEntry(input.direction, currentFrame));
            }

            // Add button inputs
            for (int i = 0; i < 4; i++)
            {
                Button btn = (Button)i;
                if (input.GetButtonDown(btn))
                {
                    buffer.Enqueue(new InputEntry(btn, currentFrame, true));
                }
                else if (input.GetButtonUp(btn))
                {
                    buffer.Enqueue(new InputEntry(btn, currentFrame, false));
                }
            }

            // Trim old entries
            while (buffer.Count > 0 && currentFrame - buffer.Peek().frame > bufferSize)
            {
                buffer.Dequeue();
            }
        }
    }

    /// <summary>
    /// Check if a specific motion was performed recently
    /// </summary>
    public bool CheckMotion(int playerIndex, MotionDefinition motion, bool facingRight)
    {
        var buffer = inputBuffers[playerIndex];
        if (buffer.Count == 0) return false;

        var entries = buffer.ToArray();
        int seqIndex = motion.sequence.Length - 1;
        int startFrame = -1;

        // Look for the final direction in the sequence
        for (int i = entries.Length - 1; i >= 0; i--)
        {
            var entry = entries[i];
            if (entry.button.HasValue) continue;

            Direction dir = entry.direction;
            if (!facingRight) dir = MirrorDirection(dir);

            if (dir == motion.sequence[seqIndex])
            {
                if (seqIndex == motion.sequence.Length - 1)
                {
                    startFrame = entry.frame;
                }
                seqIndex--;
                if (seqIndex < 0) break;
            }
        }

        if (seqIndex >= 0) return false; // Sequence not found
        if (currentFrame - startFrame > motion.maxFrames) return false; // Too slow

        // Check button press at the end
        for (int i = entries.Length - 1; i >= 0; i--)
        {
            var entry = entries[i];
            if (entry.button == motion.requiredButton && entry.pressed && entry.frame >= startFrame)
            {
                return true;
            }
            if (motion.allowNegativeEdge && entry.button == motion.requiredButton && entry.released && entry.frame >= startFrame)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check for any special motion (returns move name if found)
    /// </summary>
    public string CheckAnySpecialMotion(int playerIndex, bool facingRight, Button button)
    {
        foreach (var motion in StandardMotions)
        {
            if (motion.requiredButton == button && CheckMotion(playerIndex, motion, facingRight))
            {
                return motion.moveName;
            }
        }
        return null;
    }

    /// <summary>
    /// Get raw button state
    /// </summary>
    public bool GetButton(int playerIndex, Button button) => currentInput[playerIndex].GetButton(button);
    public bool GetButtonDown(int playerIndex, Button button) => currentInput[playerIndex].GetButtonDown(button);
    public bool GetButtonUp(int playerIndex, Button button) => currentInput[playerIndex].GetButtonUp(button);

    /// <summary>
    /// Get current direction
    /// </summary>
    public Direction GetDirection(int playerIndex) => currentInput[playerIndex].direction;

    /// <summary>
    /// Mirror direction for P2 (facing left)
    /// </summary>
    public static Direction MirrorDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Forward: return Direction.Back;
            case Direction.Back: return Direction.Forward;
            case Direction.UpForward: return Direction.UpBack;
            case Direction.UpBack: return Direction.UpForward;
            case Direction.DownForward: return Direction.DownBack;
            case Direction.DownBack: return Direction.DownForward;
            default: return dir;
        }
    }

    /// <summary>
    /// Clear buffer (on round start, stun, etc.)
    /// </summary>
    public void ClearBuffer(int playerIndex)
    {
        inputBuffers[playerIndex].Clear();
    }
}