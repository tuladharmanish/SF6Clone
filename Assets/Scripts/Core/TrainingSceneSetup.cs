using UnityEngine;

/// <summary>
/// Sets up the Training Mode scene
/// </summary>
public class TrainingSceneSetup : MonoBehaviour
{
    private void Awake()
    {
        SetupScene();
    }
    
    private void SetupScene()
    {
        // Reuse battle scene setup but with training mode
        var battleSetup = gameObject.AddComponent<BattleSceneSetup>();
        
        // Add training mode
        gameObject.AddComponent<TrainingMode>();
        
        // Training-specific UI
        CreateTrainingUI();
    }
    
    private void CreateTrainingUI()
    {
        var canvasObj = new GameObject("TrainingUI");
        canvasObj.layer = LayerMask.NameToLayer("UI");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Frame Data Panel
        var panelObj = new GameObject("FrameDataPanel");
        panelObj.transform.parent = canvasObj.transform;
        var panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);
        
        var panelRt = panelObj.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.02f, 0.02f);
        panelRt.anchorMax = new Vector2(0.4f, 0.98f);
        panelRt.offsetMin = panelRt.offsetMax = Vector2.zero;
        
        var textObj = new GameObject("FrameDataText");
        textObj.transform.parent = panelObj.transform;
        var text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        text.supportRichText = true;
        
        var textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10, 10);
        textRt.offsetMax = new Vector2(-10, -10);
        
        // Controls Help
        var helpObj = new GameObject("ControlsHelp");
        helpObj.transform.parent = canvasObj.transform;
        var helpText = helpObj.AddComponent<Text>();
        helpText.text = "TAB: Menu | H: Hitboxes | F: Frame Data | R: Reset | L.Ctrl: Record | L.Shift: Playback | .: Step Frame";
        helpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        helpText.fontSize = 16;
        helpText.color = Color.yellow;
        helpText.alignment = TextAnchor.LowerCenter;
        
        var helpRt = helpObj.GetComponent<RectTransform>();
        helpRt.anchorMin = new Vector2(0, 0);
        helpRt.anchorMax = new Vector2(1, 0.05f);
        helpRt.offsetMin = helpRt.offsetMax = Vector2.zero;
        
        // Link to TrainingMode
        var training = FindObjectOfType<TrainingMode>();
        if (training)
        {
            training.frameDataText = text;
        }
    }
}