using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject keyPrefab;   // не обязательно, т.к. ключ добавляется сразу в инвентарь
    private SpriteRenderer sr;
    private bool isOpen = false;

    void Start() => sr = GetComponent<SpriteRenderer>();

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            sr.sprite = openSprite;
            InventoryManager.Instance.AddItem("digital_key");
            DialogueSystem.Instance.ShowDialogue(new[] {
                "В сундуке лежит цифровой ключ!"
            }, speaker: "encyclopedia", emotion: "surprised");

            // Открываем статью о цифровых ключах
            EncyclopediaManager.Instance?.UnlockArticle("article_digital_key");
        }
    }
}