using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{ get; private set; }

    public int totalScore;
    public bool[] levelsCompleted;
    private int currentLevelScore = 0;
    private int totalBricksInCurrentLevel = 0;
    private int destroyedBricksCount = 0;

    [SerializeField] private int menuSceneName = 0; // ����� ���� 

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
        currentLevelScore += points;
        totalScore += points;
        SaveGame();
        Debug.Log($"Добавлено {points} очков. Всего: {totalScore}, за уровень: {currentLevelScore}");
    }
    public void ResetCurrentLevelScore()
    {
        totalScore -= currentLevelScore;
        currentLevelScore = 0;
        SaveGame();
        Debug.Log($"Очки за уровень сброшены. Итого: {totalScore}");
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            PlayerPrefs.SetInt("Level_" + i, levelsCompleted[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
        Debug.Log("���� ���������. ����: " + totalScore);
    }

    public void LoadGame()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);

        // ���� ������ ��� �� ���������������, ������� ���
        if (levelsCompleted == null)
        {
            // ���� � ��� 4 ������, ����� �������
            levelsCompleted = new bool[4];
        }

        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            levelsCompleted[i] = PlayerPrefs.GetInt("Level_" + i, 0) == 1;
        }

        Debug.Log("���� ���������. ����: " + totalScore);
    }

    // ��� ������������ � ����� ���������
    public void ResetGame()
    {
        totalScore = 0;
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            levelsCompleted[i] = false;
        }
        SaveGame();
        Debug.Log("������� �������");
    }
    public void RegisterBrick()
    {
        totalBricksInCurrentLevel++;
    }

    public void BrickDestroyed()
    {
        destroyedBricksCount++;

        // ���������, ��� �� ������� ���������
        if (destroyedBricksCount >= totalBricksInCurrentLevel)
        {
            Debug.Log("��� ������� ���������! �������� LevelComplete()");
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



    public List<string> unlockedLevels = new List<string>();
    public Dictionary<string, bool> unlockedArticles = new Dictionary<string, bool>();
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public void CompleteLevel(string levelId)
    {
        if (!unlockedLevels.Contains(levelId))
            unlockedLevels.Add(levelId);
    }

    public void UnlockArticle(string articleId)
    {
        if (!unlockedArticles.ContainsKey(articleId))
        {
            unlockedArticles[articleId] = true;
        }
    }

    public void SetFlag(string key, bool value) => flags[key] = value;
    public bool GetFlag(string key) => flags.ContainsKey(key) && flags[key];


    void Update()
    {
        // ����� � ���� �� ������� Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMenu();
        }
    }
    public void GoToMenu()
    {
        Debug.Log("����� � ���� �� Esc");
        SaveGame();               // ��������� �������� ����� ������
        Time.timeScale = 1f;     // ��������, ��� ����� �� ���������� (���� ���� �����)
        SceneManager.LoadScene(menuSceneName);
    }
}


