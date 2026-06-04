using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public string requiredItemType;
    public System.Action<string, DragAndDropItem> OnCorrectItemDropped;
    public AltarVisual altarVisual;
    private DragAndDropItem currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropItem dragged = eventData.pointerDrag.GetComponent<DragAndDropItem>();
        if (dragged != null && dragged.itemType == requiredItemType)
        {
            currentItem = dragged;
            dragged.gameObject.SetActive(false);
            InventoryManager2.Instance.RemoveItem(dragged.itemType);
            if (altarVisual != null && altarVisual.itemSpriteRenderer != null)
                altarVisual.PlaceItem(dragged.GetComponent<UnityEngine.UI.Image>().sprite);
            OnCorrectItemDropped?.Invoke(requiredItemType, dragged);
        }
        else
        {
            NotificationManager.Instance?.ShowNotification("Этот предмет не подходит для этого алтаря.", 1f);
        }
    }

    public void ReturnItem()
    {
        if (currentItem != null)
        {
            currentItem.gameObject.SetActive(true);
            InventoryManager2.Instance.AddItem(currentItem.itemType, currentItem.GetComponent<UnityEngine.UI.Image>().sprite);
            currentItem = null;
        }
        if (altarVisual != null) altarVisual.ResetAltar();
    }
}