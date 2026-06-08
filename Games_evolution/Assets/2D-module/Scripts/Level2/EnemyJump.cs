using UnityEngine;

public class EnemyJump : Enemy_2
{
    public float jumpForce = 5f;
    public float jumpCooldown = 1.5f;   // задержка между прыжками
    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextJumpTime;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        nextJumpTime = Time.time;
    }

    void Update()
    {
        // Проверка земли (луч вниз)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        isGrounded = hit.collider != null;

        if (isGrounded && Time.time >= nextJumpTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            nextJumpTime = Time.time + jumpCooldown;
        }
    }
}