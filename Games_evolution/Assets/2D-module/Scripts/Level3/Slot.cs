using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public string requiredItemType;
    public System.Action<string> OnCorrectItemDropped; // событие с передачей типа предмета

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropItem dragged = eventData.pointerDrag.GetComponent<DragAndDropItem>();
        if (dragged != null && dragged.itemType == requiredItemType)
        {
            // Правильный предмет
            dragged.gameObject.SetActive(false);
            OnCorrectItemDropped?.Invoke(requiredItemType);
        }
        else
        {
            // Неправильный – предмет возвращается (OnEndDrag вернёт его в инвентарь)
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Этот предмет не подходит для этого алтаря.", 1f);
        }
    }
}