using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 startPos;
    private Canvas parentCanvas;
    public string itemType;

    // Для призрака
    private GameObject dragGhost;
    private Canvas ghostCanvas;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Создаём призрак
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(parentCanvas.transform, false);
        dragGhost.transform.SetAsLastSibling(); // поверх всего

        // Копируем изображение
        Image originalImage = GetComponent<Image>();
        Image ghostImage = dragGhost.AddComponent<Image>();
        ghostImage.sprite = originalImage.sprite;
        ghostImage.raycastTarget = false; // чтобы не мешать лучам

        // Копируем размер и pivot
        RectTransform ghostRect = dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = rectTransform.sizeDelta;
        ghostRect.pivot = rectTransform.pivot;

        // Позиционируем под курсор
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        ghostRect.anchoredPosition = localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Оригинал не двигаем, двигаем только призрака
        if (dragGhost != null)
        {
            RectTransform ghostRect = dragGhost.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);
            ghostRect.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Уничтожаем призрака
        if (dragGhost != null) Destroy(dragGhost);

        // Далее – обработка дропа (как было раньше)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Slot slot = hit.collider.GetComponent<Slot>();
            if (slot != null)
            {
                slot.OnDrop(eventData);
                return;
            }
        }

        // Не попали – возвращаем предмет на исходную позицию
        rectTransform.anchoredPosition = startPos;
    }
}