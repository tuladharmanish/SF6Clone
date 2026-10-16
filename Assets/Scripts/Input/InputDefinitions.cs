using UnityEngine;

/// <summary>
/// 4-Button layout matching SF6 Modern controls simplified:
/// Light, Medium, Heavy, Special (Drive/Unique)
/// </summary>
public enum Button
{
    Light = 0,      // LP / LK
    Medium = 1,     // MP / MK
    Heavy = 2,      // HP / HK
    Special = 3     // Drive/Unique/Assist
}

/// <summary>
/// Directional input for motion detection
/// </summary>
public enum Direction
{
    Neutral = 0,
    Up = 1,
    UpForward = 2,
    Forward = 3,
    DownForward = 4,
    Down = 5,
    DownBack = 6,
    Back = 7,
    UpBack = 8
}

/// <summary>
/// Input buffer entry for motion detection and combo buffering
/// </summary>
public struct InputEntry
{
    public Button? button;
    public Direction direction;
    public int frame;
    public bool pressed;
    public bool released;

    public InputEntry(Direction dir, int frameNum)
    {
        button = null;
        direction = dir;
        frame = frameNum;
        pressed = false;
        released = false;
    }

    public InputEntry(Button btn, int frameNum, bool isPressed)
    {
        button = btn;
        direction = Direction.Neutral;
        frame = frameNum;
        pressed = isPressed;
        released = !isPressed;
    }
}

/// <summary>
/// Defines a special move motion (quarter circle, dragon punch, etc.)
/// </summary>
[System.Serializable]
public class MotionDefinition
{
    public string moveName;
    public Direction[] sequence;
    public int maxFrames = 15; // Input window in frames
    public Button requiredButton;
    public bool allowNegativeEdge = true; // Button release activation
}

/// <summary>
/// Complete input state for a frame
/// </summary>
public struct FrameInput
{
    public bool[] buttons;      // 4 buttons
    public bool[] buttonsPrev;  // Previous frame
    public Direction direction;
    public Direction directionPrev;

    public bool GetButtonDown(Button btn) => buttons[(int)btn] && !buttonsPrev[(int)btn];
    public bool GetButtonUp(Button btn) => !buttons[(int)btn] && buttonsPrev[(int)btn];
    public bool GetButton(Button btn) => buttons[(int)btn];

    public FrameInput Clone()
    {
        return new FrameInput
        {
            buttons = (bool[])buttons.Clone(),
            buttonsPrev = (bool[])buttonsPrev.Clone(),
            direction = direction,
            directionPrev = directionPrev
        };
    }
}