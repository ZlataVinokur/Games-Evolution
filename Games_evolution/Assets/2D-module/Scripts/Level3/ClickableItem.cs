using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public System.Action OnPickup;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPickup?.Invoke();
        gameObject.SetActive(false); // ןנוהלוע טסקוחאוע
    }
}