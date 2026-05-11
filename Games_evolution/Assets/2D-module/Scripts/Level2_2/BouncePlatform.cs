using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float extraJumpForce = 15f;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, extraJumpForce);
        }
    }
}