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
        DialogueSystem.Instance.ShowDialogue(new[] { "Провод вставлен! Автомат запитан." });
        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Розетка под напряжением." };
        return true;
    }
}