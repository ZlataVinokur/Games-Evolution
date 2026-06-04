using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float horizontalSpeed = 1f;    // скорость по горизонтали
    [SerializeField] private float stepDownDistance = 0.5f; // на сколько опускаться при достижении края
    [SerializeField] private float leftBoundary = -6f;      // левая граница
    [SerializeField] private float rightBoundary = 6f;      // правая граница
    
    private Vector2 moveDirection = Vector2.right;
    
    void Update()
    {
        // Движение в текущем направлении
        Vector2 newPosition = (Vector2)transform.position + moveDirection * horizontalSpeed * Time.deltaTime;
        
        // Проверка достижения границ
        if (newPosition.x >= rightBoundary)
        {
            moveDirection = Vector2.left;
            newPosition.x = rightBoundary;
            // Опускаем всю группу? Лучше опускать каждого врага индивидуально
            transform.position = new Vector2(newPosition.x, transform.position.y - stepDownDistance);
        }
        else if (newPosition.x <= leftBoundary)
        {
            moveDirection = Vector2.right;
            newPosition.x = leftBoundary;
            transform.position = new Vector2(newPosition.x, transform.position.y - stepDownDistance);
        }
        else
        {
            transform.position = newPosition;
        }
        
        // Если враг опустился слишком низко — вызываем поражение (опционально)
        if (transform.position.y <= -4f)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null) player.GameOver();
        }
    }
}