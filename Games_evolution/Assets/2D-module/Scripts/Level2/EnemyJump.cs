using UnityEngine;

public class EnemyJump : Enemy_2
{
    public float jumpForce = 5f;
    public float jumpInterval = 1.5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("Jump", 0f, jumpInterval);
    }

    void Jump()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}