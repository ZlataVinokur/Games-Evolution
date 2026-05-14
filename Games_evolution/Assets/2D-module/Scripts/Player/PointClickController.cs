using UnityEngine;

public class PointClickController : PlayerController_2
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("PointClickController: MainCamera не найдена!");
    }

    void Update()
    {
        HandleInput();
    }

    public override void HandleInput()
    {
        if (cam == null) return;

        // Правая кнопка – осмотр
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            Interactable obj = (hit.collider != null) ? hit.collider.GetComponent<Interactable>() : null;
            if (obj != null)
                obj.Inspect();
        }

        // Левая кнопка – сброс предмета при клике мимо всех Interactable
        if (Input.GetMouseButtonDown(0))
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItem != null)
            {
                Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider == null || hit.collider.GetComponent<Interactable>() == null)
                {
                    InventoryManager.Instance.DeselectItem();
                }
            }
        }

        // Escape – сброс предмета
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InventoryManager.Instance?.DeselectItem();
        }
    }

    public override void Move() { }
}