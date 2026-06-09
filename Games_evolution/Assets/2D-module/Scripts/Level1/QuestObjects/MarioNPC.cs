using UnityEngine;

public class MarioNPC : Interactable
{
    public Sprite aliveSprite;
    private SpriteRenderer sr;
    public GameObject mushroomObject;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.UseWithItem;
        useableItemIds.Add("coin");
        dialogueOnLook = new string[] { "Марио выглядит обесточенным. Ему не хватает монетки." };
        articleOnFirstInteractId = "article_pointandclick";   // статья при первом осмотре
    }

    protected override bool PerformAction()
    {
        sr.sprite = aliveSprite;
        mushroomObject.SetActive(true);
        GameManager.Instance.SetFlag("mario_helped", true);

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            "Марио ожил!"
        }, "player", "happy");

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            "В знак благодарности Марио дал гриб!"
        }, "encyclopedia", "happy");

        // Статья уже открыта при осмотре, но дополнительный вызов не страшен
        UnifiedInfoSystem.Instance?.UnlockArticle("article_pointandclick");

        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Марио счастлив. Гриб получен." };
        return true;
    }
}