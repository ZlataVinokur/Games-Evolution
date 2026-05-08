using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Прогресс уровней
    public List<string> unlockedLevels = new List<string>();
    public Dictionary<string, bool> unlockedArticles = new Dictionary<string, bool>();
    // Игровые флаги (используются для квестовой логики)
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Начальный уровень всегда разблокирован
            if (!unlockedLevels.Contains("Level1"))
                unlockedLevels.Add("Level1");
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            EncyclopediaManager.Instance?.ShowNotification(articleId);
        }
    }

    public void SetFlag(string key, bool value) => flags[key] = value;
    public bool GetFlag(string key) => flags.ContainsKey(key) && flags[key];
}