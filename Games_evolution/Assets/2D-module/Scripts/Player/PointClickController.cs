using UnityEngine;

public class PointClickController : PlayerController
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("PointClickController: Не найдена камера с тегом MainCamera!");
    }

    void Update()
    {
        HandleInput();
    }

    public override void HandleInput()
    {
        if (cam == null) return;

        // Правая кнопка — осмотр (рейкаст только в момент нажатия)
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            Interactable interactable = (hit.collider != null) ? hit.collider.GetComponent<Interactable>() : null;
            if (interactable != null)
            {
                interactable.Inspect();
            }
        }

        // Левая кнопка при клике на пустое место — сброс предмета
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, есть ли предмет в руке
            if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
            {
                Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                // Если не попали ни в какой объект с Interactable, сбрасываем предмет
                if (hit.collider == null || hit.collider.GetComponent<Interactable>() == null)
                {
                    InventoryManager.Instance.DeselectItem();
                }
            }
        }

        // Escape — сброс предмета
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InventoryManager.Instance?.DeselectItem();
        }
    }

    public override void Move() { }
}