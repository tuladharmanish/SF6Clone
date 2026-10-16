using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual input display for debugging/training
/// </summary>
public class InputDisplay : MonoBehaviour
{
    public int playerIndex = 0;
    public Vector2 position = new Vector2(100, 100);
    public float size = 40f;
    public float spacing = 5f;
    
    private Image[] buttonImages = new Image[4];
    private Image[] directionImages = new Image[9];
    private Color pressedColor = Color.green;
    private Color releasedColor = new Color(0.3f, 0.3f, 0.3f);
    private Color directionColor = Color.cyan;
    
    private void Start()
    {
        CreateDisplay();
    }
    
    private void CreateDisplay()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        
        // Button display (L, M, H, S)
        string[] buttonLabels = { "L", "M", "H", "S" };
        for (int i = 0; i < 4; i++)
        {
            var btnObj = new GameObject($"Button_{buttonLabels[i]}");
            btnObj.transform.SetParent(transform, false);
            
            var bg = btnObj.AddComponent<Image>();
            bg.color = releasedColor;
            
            var txtObj = new GameObject("Label");
            txtObj.transform.SetParent(btnObj.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = buttonLabels[i];
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontStyle = FontStyle.Bold;
            
            var rt = btnObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(size, size);
            rt.anchoredPosition = position + new Vector2(i * (size + spacing), 0);
            
            var txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
            
            buttonImages[i] = bg;
        }
        
        // Direction display (numpad layout)
        Direction[] dirs = {
            Direction.UpBack, Direction.Up, Direction.UpForward,
            Direction.Back, Direction.Neutral, Direction.Forward,
            Direction.DownBack, Direction.Down, Direction.DownForward
        };
        
        for (int i = 0; i < 9; i++)
        {
            var dirObj = new GameObject($"Dir_{dirs[i]}");
            dirObj.transform.SetParent(transform, false);
            
            var bg = dirObj.AddComponent<Image>();
            bg.color = releasedColor;
            
            var rt = dirObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(size * 0.7f, size * 0.7f);
            
            // Numpad layout
            int row = i / 3;
            int col = i % 3;
            rt.anchoredPosition = position + new Vector2(col * (size * 0.7f + spacing), -(size + spacing) - row * (size * 0.7f + spacing));
            
            directionImages[i] = bg;
        }
    }
    
    private void Update()
    {
        var input = InputManager.Instance;
        if (input == null) return;
        
        // Update buttons
        for (int i = 0; i < 4; i++)
        {
            Button btn = (Button)i;
            bool pressed = input.GetButton(playerIndex, btn);
            buttonImages[i].color = pressed ? pressedColor : releasedColor;
        }
        
        // Update directions
        Direction currentDir = input.GetDirection(playerIndex);
        for (int i = 0; i < 9; i++)
        {
            Direction dir = (Direction)i;
            bool active = (dir == currentDir);
            directionImages[i].color = active ? directionColor : releasedColor;
        }
    }
}