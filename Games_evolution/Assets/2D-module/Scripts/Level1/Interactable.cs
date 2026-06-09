using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum InteractionCapabilities
{
    None = 0,
    Look = 1,
    PickUp = 2,
    Use = 4,
    UseWithItem = 8
}

public enum Speaker { Player, Encyclopedia }

public class Interactable : MonoBehaviour
{
    [Header("Возможности объекта")]
    public InteractionCapabilities capabilities = InteractionCapabilities.Look;

    [Tooltip("ID предметов, которые можно применить")]
    public List<string> useableItemIds = new List<string>();

    [Header("Осмотр")]
    public string[] dialogueOnLook;

    [Header("Подбор")]
    public string itemIdToPickUp;

    [Header("Неудачное применение предмета")]
    public string[] failDialogue = new string[] { "Ой, не подходит... Надо поискать куда ещё это можно применить." };

    [Header("Статья справочника (открывается при ПЕРВОМ взаимодействии ЛЮБОГО типа)")]
    public string articleOnFirstInteractId;        // ID статьи, которая откроется один раз

    [Header("Прочее")]
    public string flagToSetOnAction;

    [Header("Диалог")]
    public Speaker speaker = Speaker.Encyclopedia;

    // Внутренний флаг, чтобы статья открывалась только один раз
    private bool firstInteractionDone = false;

    // ---------- КУРСОР ----------
    void OnMouseEnter()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
            CursorManager.Instance?.SetCursor(CursorType.Use);
        else
            CursorManager.Instance?.SetCursor(GetCursorType());
    }

    void OnMouseExit()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
            return;
        CursorManager.Instance?.ResetCursor();
    }

    // ---------- ЛЕВАЯ КНОПКА ----------
    void OnMouseDown()
    {
        // ПЕРВОЕ ВЗАИМОДЕЙСТВИЕ (для любого клика)
        TryFirstInteraction();

        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
        {
            ItemData selected = InventoryManager.Instance.selectedItem;
            if ((capabilities & InteractionCapabilities.UseWithItem) != 0 &&
                useableItemIds.Contains(selected.itemId))
            {
                UseItem(selected);
            }
            else
            {
                UnifiedInfoSystem.Instance.ShowDialogue(failDialogue, "encyclopedia", "curious");
            }
        }
        else
        {
            Interact();
        }
    }

    // ---------- ПУБЛИЧНЫЕ МЕТОДЫ ----------
    public virtual void Interact()
    {
        if ((capabilities & InteractionCapabilities.Use) != 0)
        {
            if (PerformAction())
                return;
        }

        if ((capabilities & InteractionCapabilities.PickUp) != 0)
        {
            InventoryManager.Instance?.AddItem(itemIdToPickUp);
            gameObject.SetActive(false);
            return;
        }

        Inspect();
    }

    public virtual void Inspect()
    {
        // При осмотре тоже открываем статью (если ещё не открыли)
        TryFirstInteraction();

        if ((capabilities & InteractionCapabilities.Look) != 0 && dialogueOnLook.Length > 0)
            UnifiedInfoSystem.Instance.ShowDialogue(dialogueOnLook, speaker == Speaker.Player ? "player" : "encyclopedia", "neutral");
        else
            UnifiedInfoSystem.Instance.ShowDialogue(new[] { "Ничего примечательного." }, "encyclopedia", "neutral");
    }

    protected virtual void UseItem(ItemData item)
    {
        if (useableItemIds.Contains(item.itemId))
        {
            if (PerformAction())
            {
                InventoryManager.Instance?.RemoveItem(item.itemId);
            }
        }
    }

    protected virtual bool PerformAction()
    {
        if (!string.IsNullOrEmpty(flagToSetOnAction))
            GameManager.Instance.SetFlag(flagToSetOnAction, true);
        return true;
    }

    // --- НОВЫЙ МЕТОД ДЛЯ ОТКРЫТИЯ СТАТЬИ ПРИ ПЕРВОМ ВЗАИМОДЕЙСТВИИ ---
    private void TryFirstInteraction()
    {
        if (firstInteractionDone) return;
        if (string.IsNullOrEmpty(articleOnFirstInteractId)) return;

        firstInteractionDone = true;
        UnifiedInfoSystem.Instance?.UnlockArticle(articleOnFirstInteractId);
    }

    public CursorType GetCursorType()
    {
        if ((capabilities & InteractionCapabilities.PickUp) != 0) return CursorType.Hand;
        if ((capabilities & InteractionCapabilities.Use) != 0) return CursorType.Hand;
        if ((capabilities & InteractionCapabilities.Look) != 0) return CursorType.Look;
        return CursorType.Default;
    }
}