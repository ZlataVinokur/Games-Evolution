using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GreatEncyclopediaViewer : MonoBehaviour
{
    [Header("UI Ссылки")]
    public Transform articleListParent;   // контейнер для кнопок статей
    public GameObject articleButtonPrefab; // префаб кнопки (Button + Text)
    public TextMeshProUGUI fullTextView;   // поле для показа полного текста

    void Start()
    {
        BuildArticleList();
    }

    void BuildArticleList()
    {
        var infoSys = UnifiedInfoSystem.Instance;
        if (infoSys == null || infoSys.AllArticles == null)
        {
            Debug.LogError("UnifiedInfoSystem не найден или нет статей!");
            return;
        }

        foreach (var article in infoSys.AllArticles)
        {
            bool unlocked = infoSys.IsArticleUnlocked(article.articleId);
            GameObject btnObj = Instantiate(articleButtonPrefab, articleListParent);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText == null)
            {
                // Поддержка обычного Text
                var legacyText = btnObj.GetComponentInChildren<Text>();
                if (legacyText != null)
                    legacyText.text = unlocked ? article.title : "??? (закрыто)";
            }
            else
            {
                btnText.text = unlocked ? article.title : "??? (закрыто)";
            }

            Button btn = btnObj.GetComponent<Button>();
            string id = article.articleId; // локальная копия для замыкания
            btn.onClick.AddListener(() =>
            {
                if (unlocked)
                    fullTextView.text = infoSys.GetArticleText(id);
                else
                    fullTextView.text = "Эта статья ещё не открыта. Пройдите уровни, чтобы разблокировать.";
            });
        }
    }
}