using UnityEngine;

public class BushSocket : Interactable
{
    public Sprite openedSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        interactionType = InteractionType.Use; // клик = раздвинуть
    }

    public override void OnInteract()
    {
        if (!GameManager.Instance.GetFlag("wire_found"))
        {
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Куст густой, но за ним что-то светится. Нужно что-то, чтобы его раздвинуть."
            });
        }
        else
        {
            // Раздвигаем куст
            sr.sprite = openedSprite;
            GameManager.Instance.SetFlag("bush_opened", true);
            DialogueSystem.Instance.ShowDialogue(new[] {
                "Вы раздвинули куст и увидели розетку. Теперь можно подключить провод."
            });
            // Теперь объект может принимать провод
            interactionType = InteractionType.UseWithItem;
            requiredItemId = "wire";
        }
    }
}