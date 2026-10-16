using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Sets up the Character Select scene
/// </summary>
public class CharacterSelectSceneSetup : MonoBehaviour
{
    private void Awake()
    {
        SetupScene();
    }
    
    private void SetupScene()
    {
        // Camera
        var cam = new GameObject("MainCamera");
        cam.tag = "MainCamera";
        var camera = cam.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0, 0, -10);
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        
        // Canvas
        var canvasObj = new GameObject("Canvas");
        canvasObj.layer = LayerMask.NameToLayer("UI");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Background
        CreateBackground(canvasObj.transform);
        
        // Title
        CreateTitle(canvasObj.transform);
        
        // Character Grid
        CreateCharacterGrid(canvasObj.transform);
        
        // Player Indicators
        CreatePlayerIndicators(canvasObj.transform);
        
        // Start Button
        CreateStartButton(canvasObj.transform);
        
        // Add manager
        canvasObj.AddComponent<CharacterSelectManager>();
    }
    
    private void CreateBackground(Transform parent)
    {
        var bg = new GameObject("Background");
        bg.transform.parent = parent;
        var img = bg.AddComponent<Image>();
        img.color = new Color(0.05f, 0.05f, 0.1f);
        var rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
    
    private void CreateTitle(Transform parent)
    {
        var titleObj = new GameObject("Title");
        titleObj.transform.parent = parent;
        var text = titleObj.AddComponent<Text>();
        text.text = "SELECT YOUR FIGHTER";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 48;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        
        var rt = titleObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.8f);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
    
    private void CreateCharacterGrid(Transform parent)
    {
        var gridObj = new GameObject("CharacterGrid");
        gridObj.transform.parent = parent;
        var grid = gridObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(150, 200);
        grid.spacing = new Vector2(20, 20);
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        
        var rt = gridObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.25f);
        rt.anchorMax = new Vector2(0.9f, 0.75f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
    
    private void CreatePlayerIndicators(Transform parent)
    {
        // P1
        var p1Obj = new GameObject("P1Indicator");
        p1Obj.transform.parent = parent;
        var p1Text = p1Obj.AddComponent<Text>();
        p1Text.text = "P1: Select Character";
        p1Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        p1Text.fontSize = 24;
        p1Text.color = Color.cyan;
        p1Text.alignment = TextAnchor.MiddleLeft;
        
        var p1Rt = p1Obj.GetComponent<RectTransform>();
        p1Rt.anchorMin = new Vector2(0.05f, 0.1f);
        p1Rt.anchorMax = new Vector2(0.5f, 0.2f);
        p1Rt.offsetMin = p1Rt.offsetMax = Vector2.zero;
        
        // P2
        var p2Obj = new GameObject("P2Indicator");
        p2Obj.transform.parent = parent;
        var p2Text = p2Obj.AddComponent<Text>();
        p2Text.text = "P2: Select Character";
        p2Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        p2Text.fontSize = 24;
        p2Text.color = Color.magenta;
        p2Text.alignment = TextAnchor.MiddleRight;
        
        var p2Rt = p2Obj.GetComponent<RectTransform>();
        p2Rt.anchorMin = new Vector2(0.5f, 0.1f);
        p2Rt.anchorMax = new Vector2(0.95f, 0.2f);
        p2Rt.offsetMin = p2Rt.offsetMax = Vector2.zero;
        
        // Store references for manager
        // This would be linked via inspector in practice
    }
    
    private void CreateStartButton(Transform parent)
    {
        var btnObj = new GameObject("StartButton");
        btnObj.transform.parent = parent;
        var btn = btnObj.AddComponent<Button>();
        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 0.2f);
        
        var textObj = new GameObject("Text");
        textObj.transform.parent = btnObj.transform;
        var text = textObj.AddComponent<Text>();
        text.text = "START FIGHT";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        var rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.4f, 0.02f);
        rt.anchorMax = new Vector2(0.6f, 0.08f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        
        var textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = textRt.offsetMax = Vector2.zero;
    }
}