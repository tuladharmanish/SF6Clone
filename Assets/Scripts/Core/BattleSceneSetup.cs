using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sets up the battle scene with selected characters
/// </summary>
public class BattleSceneSetup : MonoBehaviour
{
    [Header("Stage")]
    public GameObject stagePrefab;
    public Transform p1SpawnPoint, p2SpawnPoint;
    
    [Header("Systems")]
    public GameObject inputManagerPrefab;
    public GameObject hitboxSystemPrefab;
    public GameObject gameManagerPrefab;
    public GameObject cameraPrefab;
    
    private void Awake()
    {
        // Only run once
        if (FindObjectOfType<GameManager>() != null) return;
        
        SetupScene();
    }
    
    private void SetupScene()
    {
        // 1. Spawn Stage
        if (stagePrefab)
        {
            Instantiate(stagePrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            CreateDefaultStage();
        }
        
        // 2. Spawn Systems
        if (inputManagerPrefab) Instantiate(inputManagerPrefab);
        else CreateDefaultInputManager();
        
        if (hitboxSystemPrefab) Instantiate(hitboxSystemPrefab);
        else new GameObject("HitboxSystem").AddComponent<HitboxSystem>();
        
        GameManager gm = null;
        if (gameManagerPrefab) gm = Instantiate(gameManagerPrefab).GetComponent<GameManager>();
        else gm = new GameObject("GameManager").AddComponent<GameManager>();
        
        // 3. Spawn Camera
        if (cameraPrefab) Instantiate(cameraPrefab);
        else CreateDefaultCamera();
        
        // 4. Spawn Characters
        SpawnCharacters(gm);
    }
    
    private void CreateDefaultStage()
    {
        var stage = new GameObject("Stage");
        
        // Floor
        var floor = new GameObject("Floor");
        floor.transform.parent = stage.transform;
        floor.transform.position = new Vector3(0, -2, 0);
        var floorCol = floor.AddComponent<BoxCollider2D>();
        floorCol.size = new Vector2(20, 1);
        floorCol.offset = new Vector2(0, 0.5f);
        floor.layer = LayerMask.NameToLayer("Ground");
        
        // Visual floor line
        var floorSprite = floor.AddComponent<SpriteRenderer>();
        floorSprite.sprite = CreateProceduralSprite(new Color(0.3f, 0.3f, 0.3f), new Vector2(20, 1));
        floorSprite.drawMode = SpriteDrawMode.Tiled;
        floorSprite.size = new Vector2(20, 1);
        
        // Walls (for wall bounce)
        CreateWall(stage, "LeftWall", new Vector3(-10, 3, 0), new Vector2(1, 6));
        CreateWall(stage, "RightWall", new Vector3(10, 3, 0), new Vector2(1, 6));
        
        // Spawn points
        p1SpawnPoint = new GameObject("P1Spawn").transform;
        p1SpawnPoint.position = new Vector3(-4, -1, 0);
        p1SpawnPoint.parent = stage.transform;
        
        p2SpawnPoint = new GameObject("P2Spawn").transform;
        p2SpawnPoint.position = new Vector3(4, -1, 0);
        p2SpawnPoint.parent = stage.transform;
    }
    
    private void CreateWall(GameObject parent, string name, Vector3 pos, Vector2 size)
    {
        var wall = new GameObject(name);
        wall.transform.parent = parent.transform;
        wall.transform.position = pos;
        var col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        wall.layer = LayerMask.NameToLayer("Wall");
    }
    
    private Sprite CreateProceduralSprite(Color color, Vector2 size)
    {
        var tex = new Texture2D((int)size.x, (int)size.y);
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
    }
    
    private void CreateDefaultInputManager()
    {
        var go = new GameObject("InputManager");
        go.AddComponent<InputManager>();
    }
    
    private void CreateDefaultCamera()
    {
        var cam = new GameObject("MainCamera");
        cam.tag = "MainCamera";
        var camera = cam.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 8f;
        camera.transform.position = new Vector3(0, 2, -10);
        
        // Add camera follow script
        cam.AddComponent<CameraFollow>();
    }
    
    private void SpawnCharacters(GameManager gm)
    {
        var p1Data = CharacterSelectData.p1Character;
        var p2Data = CharacterSelectData.p2Character;
        
        // Default to Ryu vs Chun-Li if no selection
        if (p1Data == null) p1Data = new CharacterData { name = "Ryu", type = typeof(CharacterRyu), color = Color.white };
        if (p2Data == null) p2Data = new CharacterData { name = "Chun-Li", type = typeof(CharacterChunLi), color = Color.blue };
        
        Vector3 p1Pos = p1SpawnPoint ? p1SpawnPoint.position : new Vector3(-4, -1, 0);
        Vector3 p2Pos = p2SpawnPoint ? p2SpawnPoint.position : new Vector3(4, -1, 0);
        
        GameObject p1Obj = CreateCharacter(p1Data, p1Pos, true);
        GameObject p2Obj = CreateCharacter(p2Data, p2Pos, false);
        
        var p1Char = p1Obj.GetComponent<CharacterBase>();
        var p2Char = p2Obj.GetComponent<CharacterBase>();
        
        if (gm) gm.RegisterCharacters(p1Char, p2Char);
        
        // Set camera follow targets
        var camFollow = FindObjectOfType<CameraFollow>();
        if (camFollow) camFollow.SetTargets(p1Obj.transform, p2Obj.transform);
    }
    
    private GameObject CreateCharacter(CharacterData data, Vector3 position, bool facingRight)
    {
        GameObject charObj;
        
        if (data.prefab)
        {
            charObj = Instantiate(data.prefab, position, Quaternion.identity);
        }
        else
        {
            charObj = new GameObject(data.name);
            charObj.transform.position = position;
            
            // Add components
            var rb = charObj.AddComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            var col = charObj.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.5f, 1.8f);
            col.offset = new Vector2(0, 1);
            col.isTrigger = false;
            
            // Graphics container
            var graphics = new GameObject("Graphics");
            graphics.transform.parent = charObj.transform;
            graphics.transform.localPosition = Vector3.zero;
            var sr = graphics.AddComponent<SpriteRenderer>();
            sr.sprite = CreateProceduralSprite(data.color, new Vector2(1, 2));
            sr.sortingOrder = 10;
            
            // Hitbox container
            var hitboxes = new GameObject("Hitboxes");
            hitboxes.transform.parent = charObj.transform;
            
            // Add character script
            var charScript = charObj.AddComponent(data.type) as CharacterBase;
            charScript.playerIndex = facingRight ? 0 : 1;
            charScript.facingRight = facingRight;
        }
        
        charObj.name = "P" + (facingRight ? "1" : "2") + "_" + data.name;
        charObj.layer = LayerMask.NameToLayer("Player");
        charObj.tag = "Player";
        
        return charObj;
    }
}

/// <summary>
/// Camera follows both players, keeping them in view
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target1, target2;
    public float smoothTime = 0.1f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 12f;
    public float zoomSpeed = 2f;
    public Vector2 offset = new Vector2(0, 1);
    
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    
    public void SetTargets(Transform t1, Transform t2)
    {
        target1 = t1;
        target2 = t2;
    }
    
    private void LateUpdate()
    {
        if (target1 == null || target2 == null) return;
        
        // Midpoint
        Vector3 midpoint = (target1.position + target2.position) * 0.5f;
        midpoint += (Vector3)offset;
        midpoint.z = transform.position.z;
        
        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, midpoint, ref velocity, smoothTime);
        
        // Zoom based on distance
        float distance = Vector2.Distance(target1.position, target2.position);
        float targetSize = Mathf.Lerp(minOrthoSize, maxOrthoSize, distance / 15f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        
        // Clamp camera to stage bounds
        ClampCamera();
    }
    
    private void ClampCamera()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -9f + halfWidth, 9f - halfWidth);
        pos.y = Mathf.Clamp(pos.y, -1f + halfHeight, 7f - halfHeight);
        transform.position = pos;
    }
}