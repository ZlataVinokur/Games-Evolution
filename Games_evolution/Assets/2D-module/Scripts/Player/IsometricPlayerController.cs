using UnityEngine;

public class IsometricPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Sprites & Animation")]
    public Sprite idleSprite;
    public Sprite walkSprite1;
    public Sprite walkSprite2;
    public Sprite hurtSprite;
    private SpriteRenderer spriteRenderer;
    private float walkAnimTimer = 0f;
    private int walkCycle = 0;
    private bool isMoving = false;
    private bool isHurt = false;
    private float hurtEndTime = 0f;
    private bool isInvincible = false;
    private float invincibleEndTime = 0f;
    private float hurtFlashTimer = 0f;
    private Vector2 lastMoveDirection = Vector2.right;

    [Header("Shooting")]
    public bool hasGun = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public AudioSource shootSource;
    public int damageBonus = 0;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject[] heartIcons;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        if (idleSprite != null) spriteRenderer.sprite = idleSprite;
    }

    void Update()
    {
        // Чтение ввода – делаем здесь, чтобы moveInput был свежим
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        // Стрельба
        if (hasGun && Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootAtNearestEnemy();
        }

        // Использование предмета
        if (Input.GetKeyDown(KeyCode.E))
            InventoryManager2.Instance?.UseActiveItem();

        // Обновление анимации и поворота
        UpdateAnimation();
        UpdateInvincibilityFlash();
    }

    void FixedUpdate()
    {
        // Движение применяем в FixedUpdate
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    void UpdateAnimation()
    {
        isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            lastMoveDirection = moveInput;
            // Поворот спрайта по горизонтали
            if (moveInput.x != 0)
                spriteRenderer.flipX = moveInput.x < 0;

            // Анимация ходьбы
            if (!isHurt)
            {
                walkAnimTimer += Time.deltaTime;
                if (walkAnimTimer > 0.2f)
                {
                    walkAnimTimer = 0f;
                    walkCycle = (walkCycle + 1) % 2;
                    if (walkCycle == 0) spriteRenderer.sprite = walkSprite1;
                    else spriteRenderer.sprite = walkSprite2;
                }
            }
        }
        else
        {
            walkAnimTimer = 0f;
            if (!isHurt && idleSprite != null)
                spriteRenderer.sprite = idleSprite;
        }

        // Возврат из урона
        if (isHurt && Time.time > hurtEndTime)
        {
            isHurt = false;
            if (!isMoving && idleSprite != null) spriteRenderer.sprite = idleSprite;
        }
    }

    void UpdateInvincibilityFlash()
    {
        if (isInvincible && Time.time > invincibleEndTime)
        {
            isInvincible = false;
            spriteRenderer.color = Color.white;
        }
        if (isInvincible)
        {
            hurtFlashTimer += Time.deltaTime;
            if (hurtFlashTimer > 0.1f)
            {
                hurtFlashTimer = 0f;
                spriteRenderer.color = spriteRenderer.color == Color.white ? Color.red : Color.white;
            }
        }
    }

    void ShootAtNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f);
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDist) { minDist = dist; nearest = col.transform; }
            }
        }
        if (nearest == null) return;
        Vector2 direction = (nearest.position - firePoint.position).normalized;
        ShootInDirection(direction, damageBonus);
    }

    void ShootInDirection(Vector2 direction, int bonusDamage)
    {
        if (projectilePrefab == null || firePoint == null) return;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null) projScript.damage = 10 + bonusDamage;
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj == null) { Destroy(proj); return; }
        rbProj.linearVelocity = direction * projectileSpeed;
        shootSource?.Play();
        Destroy(proj, 2f);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        UpdateHealthUI();

        // Отскок
        Transform enemy = FindClosestEnemy();
        if (enemy != null)
        {
            Vector2 knockback = (transform.position - enemy.position).normalized;
            rb.AddForce(knockback * 5f, ForceMode2D.Impulse);
        }

        isHurt = true;
        hurtEndTime = Time.time + 0.3f;
        if (hurtSprite != null) spriteRenderer.sprite = hurtSprite;

        isInvincible = true;
        invincibleEndTime = Time.time + 1f;
        hurtFlashTimer = 0f;
        FloatingTextManager.Instance?.ShowDamage(transform.position, damage);

        if (currentHealth <= 0) Die();
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 5f);
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDist) { minDist = dist; nearest = col.transform; }
            }
        }
        return nearest;
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (heartIcons == null) return;
        int hearts = Mathf.CeilToInt((float)currentHealth / maxHealth * heartIcons.Length);
        for (int i = 0; i < heartIcons.Length; i++)
            if (heartIcons[i] != null)
                heartIcons[i].SetActive(i < hearts);
    }

    void Die() => GameManager.Instance?.ShowGameOver();
}