using UnityEngine;
using UnityEngine.UI;
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

            Image img = dragged.GetComponent<Image>();
            if (altarVisual != null && img != null)
                altarVisual.PlaceItem(img.sprite);

            OnCorrectItemDropped?.Invoke(requiredItemType, dragged);
        }
        else
        {
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Этот предмет не подходит для этого алтаря.", 1f);
        }
    }

    public void ReturnItem()
    {
        if (currentItem != null)
        {
            currentItem.gameObject.SetActive(true);
            Image img = currentItem.GetComponent<Image>();
            if (img != null)
                InventoryManager2.Instance.AddItem(currentItem.itemType, img.sprite); // изменено
            currentItem = null;
        }
        if (altarVisual != null)
            altarVisual.ResetAltar();
    }
}