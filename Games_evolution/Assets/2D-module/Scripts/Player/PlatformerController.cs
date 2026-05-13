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

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackUpForce = 4f;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    private bool isInvincible;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Article")]
    private bool stompArticleOpened;
    private bool jumpArticleOpened;

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

        // Выдача оружия по достижении высоты
        if (!weaponGiven && transform.position.y >= weaponYThreshold)
        {
            weaponGiven = true;
            GiveWeapon();
        }

        // Таймер неуязвимости
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }

        // Обновление аниматора
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        Move();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, jumpForce);
        else
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded && !jumpArticleOpened)
        {
            jumpArticleOpened = true;
            EncyclopediaManager.Instance.UnlockArticle("platformer_physics");
        }
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
            EncyclopediaManager.Instance.UnlockArticle("platformer_weapons");
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

        EncyclopediaManager.Instance.UnlockArticle("platformer_damage");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        // UI обновится автоматически, если HealthUI в Update читает свойство Health
    }

    void Die()
    {
        SceneLoader.LoadScene("Level2_Platformer");
    }

    public void AddKill()
    {
        enemiesKilled++;
        if (enemiesKilled == 1)
            EncyclopediaManager.Instance.UnlockArticle("platformer_combat");
    }

    // Отбрасывание при контакте со стеной или врагом
    void ApplyKnockback(float sourceX)
    {
        float directionX = transform.position.x < sourceX ? -1f : 1f;
        rb.linearVelocity = new Vector2(directionX * knockbackForce, knockbackUpForce);
    }

    // Физические столкновения (стены)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage(1);
            ApplyKnockback(collision.transform.position.x);
        }
    }

    // Триггеры (враги, зона смерти)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            // Условие убийства: падаем (скорость < -0.1f) и находимся выше верхней части врага
            if (rb != null && rb.linearVelocity.y < -0.1f && transform.position.y > other.transform.position.y + 0.5f)
            {
                // Убиваем врага
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.Die();
                    // немного подпрыгнуть после убийства (как в Марио)
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f); // подпрыгнуть на 10
                    if (!stompArticleOpened)
                    {
                        EncyclopediaManager.Instance.UnlockArticle("platformer_enemy_stomp");
                        stompArticleOpened = true;
                    }
                }
            }
            else
            {
                // Врезались во врага сбоку или снизу — получаем урон
                TakeDamage(1);
                ApplyKnockback(other.transform.position.x);
            }
        }
        else if (other.CompareTag("Death"))
        {
            Die();
        }
    }

    // --- АНИМАЦИЯ ---
    private bool wasGrounded;

    void UpdateAnimator()
    {
        Animator anim = GetComponent<Animator>();
        if (anim == null) return;

        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
        anim.SetFloat("horizontalSpeed", Mathf.Abs(horizontalInput));

        if (isGrounded && !wasGrounded)
        {
            // Только что коснулись платформы — принудительно проигрываем анимацию отскока с начала
            anim.Play("Jump", 0, 0f);
        }
        wasGrounded = isGrounded;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}