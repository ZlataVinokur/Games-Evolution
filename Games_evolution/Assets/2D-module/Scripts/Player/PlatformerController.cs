using UnityEngine;

public class PlatformerController : PlayerController
{
    public static PlatformerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Weapon (height‑based)")]
    [SerializeField] private float weaponYThreshold = 50f;
    private bool weaponGiven = false;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f;
    private float nextFireTime;
    private bool hasWeapon = false;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private int enemiesKilled = 0;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    private bool isInvincible;

    public int EnemiesKilled => enemiesKilled;
    public int Health => currentHealth;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleInput();

        // Выдача оружия по высоте
        if (!weaponGiven && transform.position.y >= weaponYThreshold)
        {
            weaponGiven = true;
            GiveWeapon();
        }

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }
    }

    void FixedUpdate()
    {
        Move();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, jumpForce);
        else
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    public override void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (hasWeapon && Time.time >= nextFireTime)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    public override void Move()
    {
        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.SetDirection(facingRight ? Vector2.right : Vector2.left);
    }

    public void GiveWeapon()
    {
        if (!hasWeapon)
        {
            hasWeapon = true;
            GameManager.Instance.UnlockArticle("platformer_weapons");
            Debug.Log("Оружие получено!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }

        GameManager.Instance.UnlockArticle("platformer_damage");
    }

    void Die()
    {
        SceneLoader.LoadScene("Level2_Platformer");
    }

    public void AddKill()
    {
        enemiesKilled++;
        if (enemiesKilled == 1)
            GameManager.Instance.UnlockArticle("platformer_combat");
    }

    // ФИЗИЧЕСКОЕ столкновение со стенами
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage(1);
        }
    }

    // ТРИГГЕРЫ для врагов и зоны смерти
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
        else if (other.CompareTag("Death"))
        {
            Die();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}