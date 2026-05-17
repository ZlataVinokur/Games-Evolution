using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public string requiredItemType;
    public System.Action OnCorrectItemDropped;

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropItem dragged = eventData.pointerDrag.GetComponent<DragAndDropItem>();
        if (dragged != null && dragged.itemType == requiredItemType)
        {
            dragged.gameObject.SetActive(false);
            OnCorrectItemDropped?.Invoke();
        }
    }
}