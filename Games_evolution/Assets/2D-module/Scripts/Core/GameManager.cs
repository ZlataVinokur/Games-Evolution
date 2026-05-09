using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<string> unlockedLevels = new List<string>();
    public Dictionary<string, bool> unlockedArticles = new Dictionary<string, bool>();
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            // Уведомление будет показано в EncyclopediaManager,
            // который сам вызывает этот метод при необходимости.
        }
    }

    public void SetFlag(string key, bool value) => flags[key] = value;
    public bool GetFlag(string key) => flags.ContainsKey(key) && flags[key];
}