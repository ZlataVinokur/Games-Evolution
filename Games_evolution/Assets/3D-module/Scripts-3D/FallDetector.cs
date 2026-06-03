using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("Настройки падения")]
    [SerializeField] private float fallThreshold = 5f; // На сколько метров нужно упасть ниже максимума
    [SerializeField] private float groundY = 0f; // Y-координата самой нижней точки (пола)
    [SerializeField] private float minYForFall = 0f; // Минимальная Y, ниже которой падение не считается (нижние уровни)

    private float maxReachedY;
    private bool isDead = false;

    void Start()
    {
        // Запоминаем стартовую высоту
        maxReachedY = transform.position.y;
    }

    void Update()
    {
        if (isDead) return;

        float currentY = transform.position.y;

        // Обновляем максимальную высоту, если поднялись выше
        if (currentY > maxReachedY)
        {
            maxReachedY = currentY;
        }

        // Проверяем: упал ли игрок НИЖЕ максимума на fallThreshold
        // И находится ли он ВЫШЕ минимального уровня (чтобы на нижних этажах не умирать)
        if (maxReachedY > minYForFall && currentY < maxReachedY - fallThreshold)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log($"Игрок упал! Текущая Y: {transform.position.y}, Максимальная Y: {maxReachedY}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOver();
        }
    }

    // Сброс при перезапуске (если нужно)
    public void ResetDetector()
    {
        isDead = false;
        maxReachedY = transform.position.y;
    }
}