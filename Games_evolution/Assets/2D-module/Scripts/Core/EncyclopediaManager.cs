using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    [SerializeField] private List<Article> allArticles;   // заполняется в инспекторе
    [SerializeField] private GameObject notificationPanel; // UI панель уведомлений
    [SerializeField] private TMPro.TextMeshProUGUI notificationText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Показать уведомление при первом открытии статьи.
    /// </summary>
    public void ShowNotification(string articleId)
    {
        Article article = allArticles.Find(a => a.id == articleId);
        if (article != null)
        {
            notificationText.text = article.title + "\n" + article.content.Substring(0, Mathf.Min(80, article.content.Length)) + "...";
            notificationPanel.SetActive(true);
            StartCoroutine(HideNotification(3f));
        }
    }

    System.Collections.IEnumerator HideNotification(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationPanel.SetActive(false);
    }

    /// <summary>
    /// Запуск итогового квиза (вызывается после Level3).
    /// </summary>
    public void StartQuiz()
    {
        // Загружает сцену квиза или активирует UI
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuizScene");
    }
}