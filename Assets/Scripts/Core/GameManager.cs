using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main game manager - handles round state, UI, win conditions
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int roundsToWin = 2;
    public float roundTime = 99f;
    public int startingHealth = 10000;
    
    [Header("References")]
    public CharacterBase player1;
    public CharacterBase player2;
    public Transform p1Spawn, p2Spawn;
    
    [Header("UI")]
    public Slider p1HealthBar, p2HealthBar;
    public Slider p1DriveBar, p2DriveBar;
    public Slider p1SuperBar, p2SuperBar;
    public Text roundTimerText;
    public Text p1RoundWins, p2RoundWins;
    public GameObject roundStartText, koText, perfectText;
    public Text winnerText;
    
    // State
    public bool isPaused { get; private set; }
    public bool isRoundActive { get; private set; }
    public float currentRoundTime;
    public int p1Wins = 0, p2Wins = 0;
    public int currentRound = 1;
    
    private Coroutine roundCoroutine;
    private bool roundEnded = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        InitializeRound();
    }
    
    private void Update()
    {
        if (isRoundActive && !isPaused)
        {
            currentRoundTime -= Time.deltaTime;
            UpdateTimerUI();
            
            if (currentRoundTime <= 0)
            {
                EndRoundByTime();
            }
        }
        
        // Pause handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void InitializeRound()
    {
        // Reset characters
        if (player1) player1.ResetCharacter(p1Spawn.position, true);
        if (player2) player2.ResetCharacter(p2Spawn.position, false);
        
        currentRoundTime = roundTime;
        roundEnded = false;
        isRoundActive = true;
        isPaused = false;
        
        UpdateUI();
        
        // Start round announcement
        StartCoroutine(RoundStartSequence());
    }
    
    private IEnumerator RoundStartSequence()
    {
        if (roundStartText) roundStartText.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (roundStartText) roundStartText.SetActive(false);
        
        // "FIGHT!"
        // Could add fight text here
    }
    
    public void OnCharacterKO(CharacterBase loser)
    {
        if (roundEnded) return;
        roundEnded = true;
        
        CharacterBase winner = loser == player1 ? player2 : player1;
        
        if (loser == player1) p2Wins++;
        else p1Wins++;
        
        // Check for perfect
        bool perfect = (loser == player1 && player2.currentHealth == player2.maxHealth) ||
                       (loser == player2 && player1.currentHealth == player1.maxHealth);
        
        StartCoroutine(KOSequence(winner, loser, perfect));
    }
    
    private IEnumerator KOSequence(CharacterBase winner, CharacterBase loser, bool perfect)
    {
        isRoundActive = false;
        
        if (koText) koText.SetActive(true);
        if (perfect && perfectText) perfectText.SetActive(true);
        
        winner.comboCount = 0;
        
        yield return new WaitForSeconds(2f);
        
        if (koText) koText.SetActive(false);
        if (perfectText) perfectText.SetActive(false);
        
        CheckMatchWin(winner);
    }
    
    private void EndRoundByTime()
    {
        isRoundActive = false;
        roundEnded = true;
        
        CharacterBase winner = player1.currentHealth > player2.currentHealth ? player1 : player2;
        if (player1.currentHealth == player2.currentHealth) winner = null; // Draw
        
        if (winner == player1) p1Wins++;
        else if (winner == player2) p2Wins++;
        // Draw: no one gets a point
        
        StartCoroutine(TimeUpSequence(winner));
    }
    
    private IEnumerator TimeUpSequence(CharacterBase winner)
    {
        if (winner)
        {
            if (winnerText) winnerText.text = winner.characterName + " WINS!";
        }
        else
        {
            if (winnerText) winnerText.text = "DRAW!";
        }
        
        yield return new WaitForSeconds(2f);
        CheckMatchWin(winner);
    }
    
    private void CheckMatchWin(CharacterBase roundWinner)
    {
        UpdateRoundWinsUI();
        
        if (p1Wins >= roundsToWin || p2Wins >= roundsToWin)
        {
            // Match over
            CharacterBase matchWinner = p1Wins > p2Wins ? player1 : player2;
            StartCoroutine(MatchWinSequence(matchWinner));
        }
        else
        {
            // Next round
            currentRound++;
            InitializeRound();
        }
    }
    
    private IEnumerator MatchWinSequence(CharacterBase winner)
    {
        if (winnerText) 
        {
            winnerText.text = winner.characterName + " WINS THE MATCH!";
            winnerText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(4f);
        
        // Return to character select or restart
        p1Wins = 0; p2Wins = 0; currentRound = 1;
        InitializeRound();
        
        if (winnerText) winnerText.gameObject.SetActive(false);
    }
    
    private void UpdateTimerUI()
    {
        if (roundTimerText)
        {
            int seconds = Mathf.CeilToInt(currentRoundTime);
            roundTimerText.text = seconds.ToString("00");
            
            // Warning color at 10 seconds
            if (seconds <= 10) roundTimerText.color = Color.red;
            else roundTimerText.color = Color.white;
        }
    }
    
    private void UpdateUI()
    {
        if (player1)
        {
            if (p1HealthBar) p1HealthBar.value = (float)player1.currentHealth / player1.maxHealth;
            if (p1DriveBar) p1DriveBar.value = (float)player1.currentDriveGauge / player1.maxDriveGauge;
            if (p1SuperBar) p1SuperBar.value = (float)player1.currentSuperMeter / player1.maxSuperMeter;
        }
        if (player2)
        {
            if (p2HealthBar) p2HealthBar.value = (float)player2.currentHealth / player2.maxHealth;
            if (p2DriveBar) p2DriveBar.value = (float)player2.currentDriveGauge / player2.maxDriveGauge;
            if (p2SuperBar) p2SuperBar.value = (float)player2.currentSuperMeter / player2.maxSuperMeter;
        }
    }
    
    private void UpdateRoundWinsUI()
    {
        if (p1RoundWins) p1RoundWins.text = new string('●', p1Wins) + new string('○', roundsToWin - p1Wins);
        if (p2RoundWins) p2RoundWins.text = new string('●', p2Wins) + new string('○', roundsToWin - p2Wins);
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void RegisterCharacters(CharacterBase p1, CharacterBase p2)
    {
        player1 = p1; player1.playerIndex = 0;
        player2 = p2; player2.playerIndex = 1;
    }
}

// Extension for CharacterBase
public static class CharacterBaseExtensions
{
    public static void ResetCharacter(this CharacterBase character, Vector3 position, bool facingRight)
    {
        character.transform.position = position;
        character.facingRight = facingRight;
        character.currentHealth = character.maxHealth;
        character.currentDriveGauge = character.maxDriveGauge;
        character.currentSuperMeter = 0;
        character.currentState = CharacterState.Idle;
        character.isGrounded = true;
        character.isKnockedDown = false;
        character.hitStunRemaining = 0;
        character.blockStunRemaining = 0;
        character.comboCount = 0;
        character.comboDamageScaling = 1f;
        character.rb.linearVelocity = Vector2.zero;
        
        Vector3 scale = character.graphicsContainer.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        character.graphicsContainer.localScale = scale;
    }
}