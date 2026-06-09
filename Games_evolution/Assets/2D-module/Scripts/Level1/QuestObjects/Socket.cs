using UnityEngine;

public class Socket : Interactable
{
    public Sprite poweredSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.UseWithItem;
        useableItemIds.Add("wire");
        dialogueOnLook = new string[] { "Старая розетка. Нужен провод." };
        articleOnFirstInteractId = "article_inventory";   // статья об инвентаре откроется при первом осмотре
    }

    protected override bool PerformAction()
    {
        if (GameManager.Instance.GetFlag("wire_plugged")) return false;

        GameManager.Instance.SetFlag("wire_plugged", true);
        if (poweredSprite != null) sr.sprite = poweredSprite;

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            "Провод вставлен! Автомат запитан."
        }, "encyclopedia", "happy");

        // Дополнительно открываем статью об аркадах (если ещё не открыта)
        UnifiedInfoSystem.Instance?.UnlockArticle("article_arcade");

        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Розетка под напряжением." };
        return true;
    }
}