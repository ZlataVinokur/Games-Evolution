using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float bounceForce = 20f; // Сила подброса

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Жёстко задаём скорость вверх, сохраняя горизонтальную
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            }
        }
    }
}