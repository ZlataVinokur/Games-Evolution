using UnityEngine;

public class MarioNPC : Interactable
{
    public Sprite aliveSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.UseWithItem | InteractionCapabilities.Look;
        useableItemIds.Add("coin");
        dialogueOnLook = new string[] { "Марио выглядит обесточенным. Ему не хватает монетки." };
    }

    protected override bool PerformAction()
    {
        sr.sprite = aliveSprite;
        InventoryManager.Instance.AddItem("mushroom");
        GameManager.Instance.SetFlag("mario_helped", true);
        UnifiedInfoSystem.Instance.ShowDialogue(new[]
            { "Марио ожил! 'Спасибо! Возьми этот гриб.'" },
            speaker: "player", emotion: "happy");

        UnifiedInfoSystem.Instance?.UnlockArticle("article_pointandclick");

        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Марио весело подмигивает." };
        return true;
    }
}