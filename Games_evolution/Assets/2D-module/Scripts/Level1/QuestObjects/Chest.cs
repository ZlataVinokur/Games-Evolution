using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject keyObject;
    private SpriteRenderer sr;
    private bool isOpen = false;

    void Start() => sr = GetComponent<SpriteRenderer>();

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            sr.sprite = openSprite;
            keyObject.SetActive(true);
            UnifiedInfoSystem.Instance.ShowDialogue(new[] {
                "В сундуке лежит цифровой ключ!"
            }, speaker: "encyclopedia", emotion: "happy");

            // Открываем статью о цифровых ключах
            UnifiedInfoSystem.Instance?.UnlockArticle("article_digital_key");
        }
    }
}