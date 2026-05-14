using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private LivesDisplay livesDisplay;
    private BallController ball;
    
    void Start()
    {
        // Находим на сцене компонент LivesDisplay (текст с жизнями)
        livesDisplay = FindObjectOfType<LivesDisplay>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что в зону попал мяч
        if (other.CompareTag("Ball"))
        {
            ball = other.GetComponent<BallController>();
            
            // Уменьшаем жизни
            if (livesDisplay != null)
            {
                livesDisplay.LoseLife();
            }
            
            // Пересоздаём мяч
            if (ball != null)
            {
                ball.ResetBall();
            }
        }
    }
}