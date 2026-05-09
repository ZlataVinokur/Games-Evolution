using UnityEngine;

public class Socket : Interactable
{
    [Tooltip("Спрайт после подачи питания (опционально)")]
    public Sprite poweredSprite;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Настройка возможностей: можно применить предмет (провод) и посмотреть
        capabilities = InteractionCapabilities.UseWithItem | InteractionCapabilities.Look;
        useableItemIds.Add("wire");
        dialogueOnLook = new string[] { "Старая розетка. Похоже, ей не хватает провода." };
    }

    protected override bool PerformAction()
    {
        // Проверка на случай повторного использования (уже подключили)
        if (GameManager.Instance.GetFlag("wire_plugged"))
            return false;

        GameManager.Instance.SetFlag("wire_plugged", true);

        // Меняем спрайт, если задан
        if (poweredSprite != null)
            sr.sprite = poweredSprite;

        DialogueSystem.Instance.ShowDialogue(new[]
        {
            "Провод вставлен в розетку! Аркадный автомат оживает."
        });

        // После подключения можно только осматривать
        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Розетка теперь под напряжением." };

        return true;
    }
}