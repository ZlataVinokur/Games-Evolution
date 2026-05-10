using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    [SerializeField] private List<Article> allArticles;
    public List<Article> AllArticles => allArticles;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockArticle(string articleId)
    {
        Article article = allArticles.Find(a => a.articleId == articleId);
        if (article == null) return;

        GameManager.Instance.UnlockArticle(articleId);

        // Показываем диалог Справочника с краткой аннотацией
        DialogueSystem.Instance.ShowDialogue(
            new[] { article.shortAnnotation },
            speaker: "encyclopedia",
            emotion: "happy"   
        );
    }

    public string GetArticleText(string articleId)
    {
        if (!GameManager.Instance.unlockedArticles.ContainsKey(articleId))
            return "Статья заблокирована.";
        Article article = allArticles.Find(a => a.articleId == articleId);
        return article != null ? article.fullText : "Статья не найдена.";
    }
}