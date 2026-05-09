using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    [SerializeField] private List<Article> allArticles;
    public List<Article> AllArticles => allArticles;   // теперь доступен из ArticleViewer

    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMPro.TextMeshProUGUI notificationText; // или обычный Text
    [SerializeField] private float notificationDuration = 4f;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        notificationPanel.SetActive(false);
    }

    /// <summary>
    /// Вызывается, когда статья должна открыться.
    /// </summary>
    public void UnlockArticle(string articleId)
    {
        Article article = allArticles.Find(a => a.articleId == articleId);
        if (article == null) return;

        GameManager.Instance.UnlockArticle(articleId);
        ShowNotification(article.shortAnnotation);
    }

    private void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        CancelInvoke(nameof(HideNotification));
        Invoke(nameof(HideNotification), notificationDuration);
    }

    private void HideNotification() => notificationPanel.SetActive(false);

    public string GetArticleText(string articleId)
    {
        if (!GameManager.Instance.unlockedArticles.ContainsKey(articleId))
            return "Статья заблокирована.";
        Article article = allArticles.Find(a => a.articleId == articleId);
        return article != null ? article.fullText : "Статья не найдена.";
    }
}