using UnityEngine;

public class DigitalLock : Interactable
{
    public Sprite unlockedSprite;
    public GameObject exitPortal;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.UseWithItem | InteractionCapabilities.Look;
        useableItemIds.Add("digital_key");
        dialogueOnLook = new string[] { "Цифровой замок блокирует проход. Нужен ключ." };
    }

    protected override bool PerformAction()
    {
        sr.sprite = unlockedSprite;
        GameManager.Instance.SetFlag("lock_opened", true);
        GameManager.Instance.CompleteLevel("Level1");
        DialogueSystem.Instance.ShowDialogue(new[] { "Замок открыт! Путь свободен." });
        exitPortal.SetActive(true);
        capabilities = InteractionCapabilities.Look;
        return true;
    }
}