using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Используйте UnifiedInfoSystem вместо EncyclopediaManager")]
public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowSequentialMessages(List<string> messages, Action onComplete) =>
        UnifiedInfoSystem.Instance?.ShowSequentialMessages(messages, onComplete);

    public void ShowTimedMessage(string message, float duration) =>
        UnifiedInfoSystem.Instance?.ShowTimedMessage(message, duration);

    public void ShowHint(string hintText, Action callback = null) =>
        UnifiedInfoSystem.Instance?.ShowHint(hintText, callback);

    public void UnlockArticle(string articleId) =>
        UnifiedInfoSystem.Instance?.UnlockArticle(articleId);

    public string GetArticleText(string articleId) =>
        UnifiedInfoSystem.Instance?.GetArticleText(articleId);

    public List<Article> AllArticles => UnifiedInfoSystem.Instance?.AllArticles;

    public void TellFact(string[] factLines, string emotion = "neutral") =>
        UnifiedInfoSystem.Instance?.TellFact(factLines, emotion);
}