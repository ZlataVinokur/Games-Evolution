using UnityEngine;

public class SmartPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private bool isBouncy = false;          // если true – батут
    [SerializeField] private float bounceForce = 20f;       // сила отскока (для батута)
    [SerializeField] private bool isSlippery = false;        // если true – скользкая
    [SerializeField] private PhysicsMaterial2D slipperyMaterial; // материал с трением 0

    private Collider2D solidCollider;   // физический коллайдер (НЕ триггер)
    private Collider2D triggerCollider; // триггер-коллайдер (для обнаружения входа)

    void Awake()
    {
        // На платформе должно быть два коллайдера:
        // 1. Триггер (например, BoxCollider2D с IsTrigger = true) – для обнаружения игрока.
        // 2. Твёрдый коллайдер (IsTrigger = false) – для остановки игрока, когда нужно.
        // Код ищет их по типу.
        foreach (var col in GetComponents<Collider2D>())
        {
            if (col.isTrigger) triggerCollider = col;
            else solidCollider = col;
        }
        // Изначально твёрдый коллайдер выключен
        if (solidCollider != null) solidCollider.enabled = false;

        // Если платформа скользкая, назначить материал на твёрдый коллайдер
        if (isSlippery && solidCollider != null && slipperyMaterial != null)
            solidCollider.sharedMaterial = slipperyMaterial;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Если это батут – просто подбросить и НЕ включать твёрдый коллайдер
        if (isBouncy)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            return;
        }

        // Проверка: игрок должен падать сверху (вертикальная скорость < 0)
        if (rb.linearVelocity.y <= 0)
        {
            // Определяем, находится ли игрок над платформой
            // Сравниваем Y верхней точки коллайдера игрока и Y верхней грани платформы
            float playerTopY = other.bounds.max.y;
            float platformTopY = triggerCollider.bounds.max.y;

            if (playerTopY > platformTopY)  // игрок выше верхней грани платформы
            {
                // Включаем физический коллайдер, чтобы остановить игрока
                if (solidCollider != null) solidCollider.enabled = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Когда игрок покинул зону триггера – отключаем твёрдый коллайдер
        if (solidCollider != null) solidCollider.enabled = false;
    }
}