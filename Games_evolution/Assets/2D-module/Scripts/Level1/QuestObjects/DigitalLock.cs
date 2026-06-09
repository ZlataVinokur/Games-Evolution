using UnityEngine;

public class DigitalLock : Interactable
{
    public Sprite unlockedSprite;
    public GameObject exitPortal;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.UseWithItem;
        useableItemIds.Add("digital_key");
        dialogueOnLook = new string[] { "Цифровой замок блокирует проход. Нужен ключ." };
        articleOnFirstInteractId = "article_digital_key";   // статья при осмотре
    }

    protected override bool PerformAction()
    {
        sr.sprite = unlockedSprite;
        GameManager.Instance.SetFlag("lock_opened", true);
        GameManager.Instance.CompleteLevel("Level1");

        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            "Замок открыт! Путь свободен."
        }, "encyclopedia", "happy");

        exitPortal.SetActive(true);
        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Замок сломан, проход открыт." };
        gameObject.SetActive(false);
        return true;
    }
}