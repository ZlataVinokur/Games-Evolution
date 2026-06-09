using UnityEngine;

public class TamagotchiController : Interactable
{
    public Sprite cryingSprite;
    public Sprite happySprite;
    public GameObject chestObject;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.UseWithItem;
        useableItemIds.Add("mushroom");
        dialogueOnLook = new string[] { "Тамагочи на контроллере выглядит голодным." };
        articleOnFirstInteractId = "article_puzzles";   // статья при осмотре
    }

    protected override bool PerformAction()
    {
        sr.sprite = happySprite;
        GameManager.Instance.SetFlag("tamagotchi_fed", true);

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            "Тамагочи съедает гриб и посылает искру в сундук!"
        }, "player", "happy");

        if (chestObject != null)
            chestObject.GetComponent<Chest>()?.Open();

        UnifiedInfoSystem.Instance?.UnlockArticle("article_puzzles");

        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Тамагочи сыт и доволен." };
        return true;
    }
}