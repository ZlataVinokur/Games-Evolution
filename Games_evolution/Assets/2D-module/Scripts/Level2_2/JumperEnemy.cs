using UnityEngine;

public class JumperEnemy : EnemyBase
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D rb;
    private float directionTimer;
    private float dirChangeInterval = 2f;
    private int dirX = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        directionTimer = dirChangeInterval;
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0)
        {
            dirX = Random.Range(0, 2) == 0 ? -1 : 1;
            directionTimer = dirChangeInterval;
        }
    }

    void FixedUpdate()
    {
        if (IsGrounded())
            rb.linearVelocity = new Vector2(dirX * moveSpeed, jumpForce);
        else
            rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        // ѕроста€ проверка снизу, можно добавить groundCheck
        return Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
    }

    public override void Die()
    {
        PlatformerController.Instance.AddKill();
        Destroy(gameObject);
    }
}