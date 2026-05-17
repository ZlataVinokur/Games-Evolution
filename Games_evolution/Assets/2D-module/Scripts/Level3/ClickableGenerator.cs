using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableGenerator : MonoBehaviour, IPointerClickHandler
{
    public System.Action OnCollect;
    public int energyAmount = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCollect?.Invoke();
        // Можно оставить объект, но сделать неактивным на время
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
}