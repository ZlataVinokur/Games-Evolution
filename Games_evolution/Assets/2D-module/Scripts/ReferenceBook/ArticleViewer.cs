using UnityEngine;
using UnityEngine.UI;

public class ArticleViewer : MonoBehaviour
{
    public Transform articleListParent;
    public GameObject articleButtonPrefab;
    public Text fullTextDisplay;

    void Start()
    {
        var articles = EncyclopediaManager.Instance?.AllArticles;
        if (articles == null) return;

        foreach (var art in articles)
        {
            var btnObj = Instantiate(articleButtonPrefab, articleListParent);
            var btnText = btnObj.GetComponentInChildren<Text>();
            bool unlocked = GameManager.Instance.unlockedArticles.ContainsKey(art.articleId);
            btnText.text = unlocked ? art.title : "??? (закрыто)";
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (unlocked)
                    fullTextDisplay.text = art.fullText;
                else
                    fullTextDisplay.text = "Статья ещё не открыта.";
            });
        }
    }
}