using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    public float extraJumpForce = 8f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = extraJumpForce;
                rb.linearVelocity = velocity;
                // ����� �������� ���� ��� ������
            }
        }
    }
}