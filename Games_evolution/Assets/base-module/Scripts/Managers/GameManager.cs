using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int totalScore;
    public bool[] levelsCompleted;

    private int totalBricksInCurrentLevel = 0;
    private int destroyedBricksCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CompleteLevel(int levelIndex, int score)
    {
        if (!levelsCompleted[levelIndex])
        {
            levelsCompleted[levelIndex] = true;
            totalScore += score;
            SaveGame();
        }
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 0) return true;
        return levelsCompleted[levelIndex - 1];
    }
    
    public void AddScore(int points)
    {
        totalScore += points;
        SaveGame();
    }
    
    public void SaveGame()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            PlayerPrefs.SetInt("Level_" + i, levelsCompleted[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Игра сохранена. Очки: " + totalScore);
    }
    
    public void LoadGame()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        
        // Если массив еще не инициализирован, создаем его
        if (levelsCompleted == null)
        {
            // Пока у нас 4 уровня, потом изменим
            levelsCompleted = new bool[4];
        }
        
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            levelsCompleted[i] = PlayerPrefs.GetInt("Level_" + i, 0) == 1;
        }
        
        Debug.Log("Игра загружена. Очки: " + totalScore);
    }
    
    // Для тестирования — сброс прогресса
    public void ResetGame()
    {
        totalScore = 0;
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            levelsCompleted[i] = false;
        }
        SaveGame();
        Debug.Log("Прогрес сброшен");
    }
    public void RegisterBrick()
    {
        totalBricksInCurrentLevel++;
    }
    
    public void BrickDestroyed()
    {
        destroyedBricksCount++;
        
        // Проверяем, все ли кирпичи разрушены
        if (destroyedBricksCount >= totalBricksInCurrentLevel)
        {
            Debug.Log("ВСЕ КИРПИЧИ РАЗРУШЕНЫ! Вызываем LevelComplete()");
            LevelComplete();
        }
    }
    
    public void ResetLevelBrickCounter()
    {
        totalBricksInCurrentLevel = 0;
        destroyedBricksCount = 0;
    }
    
    private void LevelComplete()
    {

        if (levelsCompleted != null && levelsCompleted.Length > 0)
        {
            levelsCompleted[0] = true;
            SaveGame();
        }

        GameOverManager gm = FindObjectOfType<GameOverManager>();
        if (gm != null)
            gm.ShowLevelComplete();
        else
            Debug.LogError("GameOverManager not found on scene!");
    }
}
    

