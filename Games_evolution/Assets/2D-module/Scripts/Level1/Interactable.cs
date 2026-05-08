using UnityEngine;

public enum InteractionType { Look, PickUp, Use, UseWithItem }

public class Interactable : MonoBehaviour
{
    public InteractionType interactionType;
    public string itemId;
    public string requiredItemId;
    public string[] dialogueOnLook;
    public string flagToSetOnAction;
    protected bool isActive = true;

    void OnMouseEnter() => CursorManager.Instance?.SetCursor(interactionType switch
    {
        InteractionType.Look => CursorType.Look,
        InteractionType.PickUp => CursorType.Hand,
        InteractionType.Use => CursorType.Hand,
        InteractionType.UseWithItem => CursorType.Use,
        _ => CursorType.Default
    });

    void OnMouseExit() => CursorManager.Instance?.ResetCursor();

    void OnMouseDown()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
        {
            if (CanUseItem(InventoryManager.Instance.selectedItem.itemId))
            {
                InventoryManager.Instance.UseItem(InventoryManager.Instance.selectedItem, gameObject);
                return;
            }
        }
        OnInteract();
    }

    public virtual void OnInteract()
    {
        switch (interactionType)
        {
            case InteractionType.Look:
                if (dialogueOnLook.Length > 0)
                    DialogueSystem.Instance.ShowDialogue(dialogueOnLook);
                break;
            case InteractionType.PickUp:
                InventoryManager.Instance.AddItem(itemId);
                gameObject.SetActive(false);
                break;
            case InteractionType.Use:
                PerformAction();
                break;
            case InteractionType.UseWithItem:
                break;
        }
    }

    public virtual bool CanUseItem(string usedItemId) =>
        interactionType == InteractionType.UseWithItem && usedItemId == requiredItemId;

    public virtual void UseItem(string usedItemId)
    {
        if (usedItemId == requiredItemId)
            PerformAction();
    }

    protected virtual void PerformAction()
    {
        if (!string.IsNullOrEmpty(flagToSetOnAction))
            GameManager.Instance.SetFlag(flagToSetOnAction, true);
    }
}