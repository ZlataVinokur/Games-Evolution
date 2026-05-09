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

public class Interactable : MonoBehaviour
{
    [Header("Возможности объекта")]
    public InteractionCapabilities capabilities = InteractionCapabilities.Look;

    [Tooltip("ID предметов, которые можно применить к объекту")]
    public List<string> useableItemIds = new List<string>();

    [Header("Осмотр")]
    public string[] dialogueOnLook;

    [Header("Подбор")]
    public string itemIdToPickUp;

    [Header("Неудачное применение предмета")]
    public string[] failDialogue = new string[] { "Ой, не подходит... Надо поискать куда ещё это можно применить." };

    [Header("Прочее")]
    public string flagToSetOnAction;

    // ---------- ОТОБРАЖЕНИЕ КУРСОРА ----------
    void OnMouseEnter()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
        {
            // Если в руке предмет — всегда показываем Use
            CursorManager.Instance?.SetCursor(CursorType.Use);
        }
        else
        {
            // Иначе стандартный курсор в зависимости от возможностей
            CursorManager.Instance?.SetCursor(GetCursorType());
        }
    }

    void OnMouseExit()
    {
        // Если в руке предмет, не сбрасываем курсор (оставим Use)
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
            return;
        CursorManager.Instance?.ResetCursor();
    }

    // ---------- ЛЕВАЯ КНОПКА (действие / подбор / применение предмета) ----------
    void OnMouseDown()
    {
        // Если игрок держит предмет в руке — пытаемся использовать его на этом объекте
        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
        {
            ItemData selectedItem = InventoryManager.Instance.selectedItem;
            if ((capabilities & InteractionCapabilities.UseWithItem) != 0 &&
                useableItemIds.Contains(selectedItem.itemId))
            {
                // Предмет подходит
                UseItem(selectedItem);
            }
            else
            {
                // Предмет не подходит — показываем сообщение, предмет остаётся в руке
                DialogueSystem.Instance.ShowDialogue(failDialogue);
            }
        }
        else
        {
            // Нет предмета в руке — выполняем основное действие
            Interact();
        }
    }

    // ---------- ПУБЛИЧНЫЕ МЕТОДЫ (используются контроллером для правой кнопки и т.д.) ----------
    /// <summary> Основное действие (активация, подбор или осмотр). </summary>
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

        // Если ничего не вышло — осмотр
        Inspect();
    }

    /// <summary> Осмотр (показывает диалог). Вызывается по правой кнопке. </summary>
    public virtual void Inspect()
    {
        if ((capabilities & InteractionCapabilities.Look) != 0 && dialogueOnLook.Length > 0)
        {
            DialogueSystem.Instance.ShowDialogue(dialogueOnLook);
        }
        else
        {
            DialogueSystem.Instance.ShowDialogue(new[] { "Ничего примечательного." });
        }
    }

    // ---------- ВСПОМОГАТЕЛЬНЫЕ ПЕРЕОПРЕДЕЛЯЕМЫЕ МЕТОДЫ ----------
    protected virtual void UseItem(ItemData item)
    {
        if (useableItemIds.Contains(item.itemId))
        {
            PerformAction();
            InventoryManager.Instance?.RemoveItem(item.itemId); // предмет удаляется после использования
        }
    }

    protected virtual bool PerformAction()
    {
        if (!string.IsNullOrEmpty(flagToSetOnAction))
            GameManager.Instance.SetFlag(flagToSetOnAction, true);
        return true;
    }

    public CursorType GetCursorType()
    {
        if ((capabilities & InteractionCapabilities.PickUp) != 0)
            return CursorType.Hand;
        if ((capabilities & InteractionCapabilities.Use) != 0)
            return CursorType.Hand;
        if ((capabilities & InteractionCapabilities.Look) != 0)
            return CursorType.Look;
        return CursorType.Default;
    }
}