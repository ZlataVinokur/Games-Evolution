using UnityEngine;

public class DigitalLock : Interactable
{
    public Sprite unlockedSprite;
    public GameObject exitPortal;
    private SpriteRenderer sr;   

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        interactionType = InteractionType.UseWithItem;
        requiredItemId = "digital_key";
    }

    public override void UseItem(string usedItemId)
    {
        if (usedItemId == "digital_key")
        {
            sr.sprite = unlockedSprite;
            GameManager.Instance.SetFlag("lock_opened", true);
            GameManager.Instance.CompleteLevel("Level1");
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Замок открыт! Проход в следующий уровень свободен."
            });
            exitPortal.SetActive(true);
        }
    }
}