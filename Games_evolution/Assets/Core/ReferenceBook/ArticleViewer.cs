using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArticleViewer : MonoBehaviour
{
    [Header("Модули (3 статичные кнопки)")]
    public Button moduleButton0;
    public Button moduleButton1;
    public Button moduleButton2;

    [Header("Контейнер для списка статей")]
    public Transform articlesContainer;    // Content для ScrollView статей
    public TMP_Text articleTitleText;      // заголовок статьи
    public TMP_Text articleContentText;    // полный текст статьи

    [Header("Префаб кнопки статьи")]
    public GameObject articleButtonPrefab;

    [Header("Навигация")]
    public Button backButton;

    // Данные
    private List<Article> allArticles;
    private Dictionary<int, List<Article>> articlesByModule;
    private int currentModule = -1;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager не найден!");
            return;
        }

        LoadArticles();

        // Привязываем кнопки модулей
        moduleButton0.onClick.AddListener(() => OnModuleSelected(0));
        moduleButton1.onClick.AddListener(() => OnModuleSelected(1));
        moduleButton2.onClick.AddListener(() => OnModuleSelected(2));

        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);

        // По умолчанию показываем первый модуль (0)
        OnModuleSelected(0);
    }

    private void LoadArticles()
    {
        var infoSys = UnifiedInfoSystem.Instance;
        if (infoSys == null || infoSys.AllArticles == null)
        {
            Debug.LogError("UnifiedInfoSystem или статьи не загружены!");
            allArticles = new List<Article>();
            articlesByModule = new Dictionary<int, List<Article>>();
            return;
        }

        allArticles = infoSys.AllArticles;
        articlesByModule = allArticles
            .Where(a => a != null)
            .GroupBy(a => a.moduleIndex)   // используем moduleIndex (0,1,2)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private void OnModuleSelected(int moduleIndex)
    {
        currentModule = moduleIndex;
        PopulateArticles(moduleIndex);
    }

    private void PopulateArticles(int moduleIndex)
    {
        // Очищаем старый список статей
        foreach (Transform child in articlesContainer)
            Destroy(child.gameObject);

        // Сбрасываем текст
        if (articleTitleText != null) articleTitleText.text = "";
        articleContentText.text = "Выберите статью";

        if (!articlesByModule.ContainsKey(moduleIndex))
        {
            articleContentText.text = "Для этого модуля нет статей.";
            return;
        }

        List<Article> articles = articlesByModule[moduleIndex];
        bool moduleUnlocked = IsModuleUnlocked(moduleIndex);

        foreach (var article in articles)
        {
            GameObject btnObj = Instantiate(articleButtonPrefab, articlesContainer);
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            Image lockIcon = btnObj.transform.Find("LockIcon")?.GetComponent<Image>();

            bool unlocked = moduleUnlocked;

            if (unlocked)
            {
                btnText.text = article.title;
                if (lockIcon != null) lockIcon.gameObject.SetActive(false);
            }
            else
            {
                btnText.text = "??? (заблокировано)";
                if (lockIcon != null) lockIcon.gameObject.SetActive(true);
            }

            Button btn = btnObj.GetComponent<Button>();
            btn.interactable = unlocked;

            if (unlocked)
            {
                btn.onClick.AddListener(() => ShowArticle(article));
            }
            // Если заблокировано – кнопка неактивна, ничего не делаем
        }
    }

    private void ShowArticle(Article article)
    {
        if (articleTitleText != null)
            articleTitleText.text = article.title;
        articleContentText.text = article.fullText;
    }

    private bool IsModuleUnlocked(int moduleIndex)
    {
        // Предполагаем, что в GameManager есть массив modulesCompleted[moduleIndex]
        // Если его нет – добавим позже.
        if (GameManager.Instance == null) return false;

        // TODO: заменить на реальную проверку. Пока возвращаем true для отладки
        // Реализуем через отдельный массив в GameManager
        if (GameManager.Instance.modulesCompleted == null) return false;
        return moduleIndex < GameManager.Instance.modulesCompleted.Length &&
               GameManager.Instance.modulesCompleted[moduleIndex];
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}