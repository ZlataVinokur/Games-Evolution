using UnityEngine;

public class MarioNPC : Interactable
{
    public Sprite aliveSprite;
    private SpriteRenderer sr;
    public GameObject mushroomObject;

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
        mushroomObject.SetActive(true);
        GameManager.Instance.SetFlag("mario_helped", true);
        UnifiedInfoSystem.Instance.ShowDialogue(new[]
            { "Марио ожил! 'Спасибо! Возьми этот гриб.'" },
            speaker: "player", emotion: "happy");

        UnifiedInfoSystem.Instance?.UnlockArticle("article_pointandclick");

        UnifiedInfoSystem.Instance.ShowDialogue(new[] { "В знак благодарности Марисио дал гриб!" }, speaker: "encyclopedia", emotion: "happy");
        return true;
    }
}