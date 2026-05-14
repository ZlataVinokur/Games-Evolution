using UnityEngine;

public class Socket : Interactable
{
    public Sprite poweredSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.UseWithItem | InteractionCapabilities.Look;
        useableItemIds.Add("wire");
        dialogueOnLook = new string[] { "Старая розетка. Нужен провод." };
    }

    protected override bool PerformAction()
    {
        if (GameManager.Instance.GetFlag("wire_plugged"))
            return false;

        GameManager.Instance.SetFlag("wire_plugged", true);
        if (poweredSprite != null) sr.sprite = poweredSprite;

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
            { "Провод вставлен! Автомат запитан." },
            speaker: "encyclopedia", emotion: "happy");

        // Открываем статьи об аркадах и инвентаре
        UnifiedInfoSystem.Instance?.UnlockArticle("article_arcade");
        UnifiedInfoSystem.Instance?.UnlockArticle("article_inventory");

        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Розетка под напряжением." };
        return true;
    }
}