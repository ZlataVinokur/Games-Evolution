using UnityEngine;

public class PointClickController : PlayerController
{
    // В Level1 персонаж не двигается, управление сводится к скроллу камеры и кликам
    public override void HandleInput()
    {
        // Камера двигается отдельным скриптом, здесь обрабатываем клики по интерактивным объектам
        if (Input.GetMouseButtonDown(0))
        {
            // Рейкаст для 2D
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }
        // Отмена выбора предмета по ПКМ
        if (Input.GetMouseButtonDown(1))
        {
            InventoryManager.Instance?.DeselectItem();
        }
    }

    public override void Move() { } // персонаж статичен
}