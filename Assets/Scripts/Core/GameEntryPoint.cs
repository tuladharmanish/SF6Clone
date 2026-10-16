using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main entry point - handles scene flow
/// </summary>
public class GameEntryPoint : MonoBehaviour
{
    [Header("Scene Names")]
    public string characterSelectScene = "CharacterSelect";
    public string battleScene = "BattleScene";
    public string trainingScene = "TrainingScene";
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        // Start at character select
        if (SceneManager.GetActiveScene().name != characterSelectScene)
        {
            SceneManager.LoadScene(characterSelectScene);
        }
    }
    
    public void GoToCharacterSelect()
    {
        SceneManager.LoadScene(characterSelectScene);
    }
    
    public void GoToBattle()
    {
        SceneManager.LoadScene(battleScene);
    }
    
    public void GoToTraining()
    {
        SceneManager.LoadScene(trainingScene);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}