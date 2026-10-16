using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character select screen manager
/// </summary>
public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }
    
    [Header("Character Prefabs")]
    public GameObject ryuPrefab;
    public GameObject chunLiPrefab;
    public GameObject zangiefPrefab;
    public GameObject guilePrefab;
    public GameObject lukePrefab;
    public GameObject jpPrefab;
    
    [Header("UI")]
    public Transform p1PortraitContainer, p2PortraitContainer;
    public GameObject portraitPrefab;
    public Text p1SelectedText, p2SelectedText;
    public Button startButton;
    
    [Header("Stage")]
    public GameObject stagePrefab;
    
    private CharacterData[] allCharacters;
    private CharacterData p1Selection, p2Selection;
    private int p1Cursor = 0, p2Cursor = 0;
    private bool p1Locked = false, p2Locked = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        InitializeCharacterData();
    }
    
    private void Start()
    {
        SetupPortraits();
        startButton.interactable = false;
        startButton.onClick.AddListener(StartGame);
    }
    
    private void InitializeCharacterData()
    {
        allCharacters = new CharacterData[]
        {
            new CharacterData { name = "Ryu", prefab = ryuPrefab, type = typeof(CharacterRyu), 
                description = "Balanced Shoto\nFireball / DP / Tatsu", color = Color.white },
            new CharacterData { name = "Chun-Li", prefab = chunLiPrefab, type = typeof(CharacterChunLi),
                description = "Rushdown / Pokes\nLightning Legs / SBK", color = Color.blue },
            new CharacterData { name = "Zangief", prefab = zangiefPrefab, type = typeof(CharacterZangief),
                description = "Grappler\nSPD / Lariat / Armor", color = Color.red },
            new CharacterData { name = "Guile", prefab = guilePrefab, type = typeof(CharacterGuile),
                description = "Charge Zoner\nSonic Boom / Flash Kick", color = Color.green },
            new CharacterData { name = "Luke", prefab = lukePrefab, type = typeof(CharacterLuke),
                description = "Modern Rushdown\nSand Blaster / Flash Knuckle", color = new Color(1f, 0.5f, 0f) },
            new CharacterData { name = "JP", prefab = jpPrefab, type = typeof(CharacterJP),
                description = "Tricky Zoner\nPortals / Teleports / Traps", color = new Color(0.5f, 0f, 0.5f) }
        };
    }
    
    private void SetupPortraits()
    {
        // Clear existing
        foreach (Transform child in p1PortraitContainer) Destroy(child.gameObject);
        foreach (Transform child in p2PortraitContainer) Destroy(child.gameObject);
        
        // Create portraits
        for (int i = 0; i < allCharacters.Length; i++)
        {
            CreatePortrait(p1PortraitContainer, allCharacters[i], i, true);
            CreatePortrait(p2PortraitContainer, allCharacters[i], i, false);
        }
        
        UpdateCursor(0, true);
        UpdateCursor(0, false);
    }
    
    private void CreatePortrait(Transform container, CharacterData data, int index, bool isP1)
    {
        GameObject portrait = Instantiate(portraitPrefab, container);
        // Portrait would have image, name text, etc.
        // For procedural, we'll just use colored squares
        var img = portrait.GetComponent<Image>();
        if (img) img.color = data.color;
        
        var btn = portrait.GetComponent<Button>();
        if (btn)
        {
            int idx = index;
            btn.onClick.AddListener(() => OnPortraitClick(idx, isP1));
        }
    }
    
    private void Update()
    {
        if (p1Locked && p2Locked) return;
        
        // P1 Input (WASD + J)
        if (!p1Locked)
        {
            if (Input.GetKeyDown(KeyCode.D)) MoveCursor(1, true);
            if (Input.GetKeyDown(KeyCode.A)) MoveCursor(-1, true);
            if (Input.GetKeyDown(KeyCode.J)) LockSelection(true);
        }
        
        // P2 Input (Arrows + Keypad4)
        if (!p2Locked)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveCursor(1, false);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveCursor(-1, false);
            if (Input.GetKeyDown(KeyCode.Keypad4)) LockSelection(false);
        }
    }
    
    private void MoveCursor(int dir, bool isP1)
    {
        if (isP1)
        {
            p1Cursor = (p1Cursor + dir + allCharacters.Length) % allCharacters.Length;
            UpdateCursor(p1Cursor, true);
        }
        else
        {
            p2Cursor = (p2Cursor + dir + allCharacters.Length) % allCharacters.Length;
            UpdateCursor(p2Cursor, false);
        }
    }
    
    private void UpdateCursor(int index, bool isP1)
    {
        Transform container = isP1 ? p1PortraitContainer : p2PortraitContainer;
        Text text = isP1 ? p1SelectedText : p2SelectedText;
        
        for (int i = 0; i < container.childCount; i++)
        {
            var child = container.GetChild(i);
            var outline = child.GetComponent<Outline>();
            if (outline) outline.enabled = (i == index);
        }
        
        if (text) text.text = "P" + (isP1 ? "1" : "2") + ": " + allCharacters[index].name;
    }
    
    private void OnPortraitClick(int index, bool isP1)
    {
        if (isP1) { p1Cursor = index; UpdateCursor(index, true); }
        else { p2Cursor = index; UpdateCursor(index, false); }
    }
    
    private void LockSelection(bool isP1)
    {
        if (isP1)
        {
            p1Selection = allCharacters[p1Cursor];
            p1Locked = true;
            if (p1SelectedText) p1SelectedText.text = "P1: " + p1Selection.name + " ✓";
        }
        else
        {
            p2Selection = allCharacters[p2Cursor];
            p2Locked = true;
            if (p2SelectedText) p2SelectedText.text = "P2: " + p2Selection.name + " ✓";
        }
        
        startButton.interactable = p1Locked && p2Locked;
    }
    
    private void StartGame()
    {
        // Load battle scene with selected characters
        CharacterSelectData.p1Character = p1Selection;
        CharacterSelectData.p2Character = p2Selection;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
}

[System.Serializable]
public class CharacterData
{
    public string name;
    public GameObject prefab;
    public System.Type type;
    public string description;
    public Color color;
}

public static class CharacterSelectData
{
    public static CharacterData p1Character;
    public static CharacterData p2Character;
}