using UnityEngine;

public class TamagotchiController : Interactable
{
    public Sprite cryingSprite;
    public Sprite happySprite;
    public GameObject chestObject; // ссылка на сундук
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.UseWithItem | InteractionCapabilities.Look;
        useableItemIds.Add("mushroom");
        dialogueOnLook = new string[] { "Тамагочи на контроллере выглядит голодным." };
    }

    protected override bool PerformAction()
    {
        sr.sprite = happySprite;
        GameManager.Instance.SetFlag("tamagotchi_fed", true);
        DialogueSystem.Instance.ShowDialogue(new[] { "Тамагочи съедает гриб и посылает искру в сундук!" });
        if (chestObject != null)
            chestObject.GetComponent<Chest>().Open();
        // После кормления – только осмотр
        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Тамагочи сыт и доволен." };
        return true;
    }
}