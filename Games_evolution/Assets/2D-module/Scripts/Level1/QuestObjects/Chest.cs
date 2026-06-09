using UnityEngine;

public class Chest : Interactable
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject keyObject;
    private SpriteRenderer sr;
    private bool isOpen = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.Use;
        dialogueOnLook = new string[] { "Закрытый сундук. Нужно как-то его открыть." };
        articleOnFirstInteractId = "article_digital_key";   // откроется при первом осмотре/попытке открыть
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        sr.sprite = openSprite;
        keyObject.SetActive(true);

        UnifiedInfoSystem.Instance.ShowDialogue(new[] {
            "В сундуке лежит цифровой ключ!"
        }, "encyclopedia", "happy");

        // Статья откроется через базовый механизм, но дополнительный вызов не помешает
        UnifiedInfoSystem.Instance?.UnlockArticle("article_digital_key");

        // После открытия сундук больше не интерактивен
        capabilities = InteractionCapabilities.None;
    }
}