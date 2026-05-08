using UnityEngine;

public class TamagotchiController : Interactable
{
    public Sprite cryingSprite;
    public Sprite happySprite;
    private SpriteRenderer sr;
    public GameObject chestObject; // ссылка на сундук

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        interactionType = InteractionType.UseWithItem;
        requiredItemId = "mushroom";
    }

    public override void OnInteract()
    {
        if (!GameManager.Instance.GetFlag("tamagotchi_fed"))
        {
            sr.sprite = cryingSprite;
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Тамагочи плачет и показывает символ голода..."
            });
        }
    }

    public override void UseItem(string usedItemId)
    {
        if (usedItemId == "mushroom")
        {
            sr.sprite = happySprite;
            GameManager.Instance.SetFlag("tamagotchi_fed", true);
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Тамагочи радостно поедает гриб и посылает искру в сундук!"
            });
            // Открываем сундук
            if (chestObject != null)
                chestObject.GetComponent<Chest>().Open();
        }
    }
}