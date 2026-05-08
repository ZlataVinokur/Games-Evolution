using UnityEngine;

public class MarioNPC : Interactable
{
    public Sprite aliveSprite;
    public GameObject mushroomPrefab;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        interactionType = InteractionType.UseWithItem;
        requiredItemId = "coin";
    }

    public override void UseItem(string usedItemId)
    {
        if (usedItemId == "coin")
        {
            sr.sprite = aliveSprite;
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Марио ожил! 'Спасибо! Возьми этот гриб.'"
            });
            // Спавним гриб в мире или сразу в инвентарь
            InventoryManager.Instance.AddItem("mushroom");
            GameManager.Instance.SetFlag("mario_helped", true);
        }
    }
}