using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

public class PixelPlatformerController : PlayerController_2
{
    [Header("Движение")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Здоровье")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    private float invincibleTime = 1f;

    [Header("Оружие")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float weaponUnlockHeight = 20f;
    private bool hasWeapon = false;

    [Header("Враги и прогресс")]
    private int enemiesKilled = 0;
    private int neededKills = 5;
    private bool portalActive = false;
    public GameObject portalPrefab;
    private Vector3 portalPosition = new Vector3(12f, 30f, 0f);

    [Header("Бонусы")]
    public GameObject healthBonusPrefab;
    public GameObject shieldBonusPrefab;

    [Header("UI")]
    public Image[] heartImages;
    public Sprite heartFull;
    public Sprite heartBroken;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI killsText;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private bool isDead = false;

    // Свойства для доступа из UI
    public int CurrentHealth => currentHealth;
    public int EnemiesKilled => enemiesKilled;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Убедимся, что коллайдер не триггер
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateKillsUI();
    }

    void Start()
    {
        // Проверка groundCheck
        if (groundCheck == null)
        {
            GameObject go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = go.transform;
        }
    }

    public override void HandleInput()
    {
        if (IsInputBlocked()) return;
        if (isDead) return;

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Расширенная диагностика
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        isGrounded = groundCollider != null;


        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log($"Jump pressed, isGrounded={isGrounded}");
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    public override void Move() { } // не используется

    void Update()
    {
        HandleInput();
        if (IsInputBlocked()) return;
        if (isDead) return;

        // Обновление UI высоты
        if (heightText != null)
            heightText.text = $"Высота: {Mathf.FloorToInt(transform.position.y)}";

        // Стрельба
        if (hasWeapon && Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Получение оружия по высоте
        if (!hasWeapon && transform.position.y >= weaponUnlockHeight)
        {
            UnlockWeapon();
        }

        // Активация портала
        if (!portalActive && enemiesKilled >= neededKills)
        {
            GameManager manager = FindObjectOfType<GameManager>();
            if (manager != null) manager.ShowWin();
        }
    }

    void Shoot()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject nearest = enemies.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).First();
        Vector2 direction = (nearest.transform.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet_2>().Initialize(direction);
    }

    void UnlockWeapon()
    {
        hasWeapon = true;
        if (UnifiedInfoSystem.Instance != null)
        {
            string[] fact = { "В 1985 году в игре Super Mario Bros. появилась возможность стрелять. А в 1987-м Contra сделала стрельбу главной механикой. Теперь и ты вооружён!" };
            UnifiedInfoSystem.Instance.ShowDialogue(fact, "encyclopedia", "happy");
        }
        Debug.Log("Оружие получено!");
    }

    void ActivatePortal()
    {
        portalActive = true;
        Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        if (UnifiedInfoSystem.Instance != null)
        {
            string[] msg = { "Портал в правом верхнем углу! Прыгни в него, чтобы завершить уровень, или продолжай подниматься выше – выбор за тобой." };
            UnifiedInfoSystem.Instance.ShowDialogue(msg, "encyclopedia", "neutral");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Portal") && portalActive)
        {
            CompleteLevel();
        }
        else if (other.CompareTag("HealthBonus"))
        {
            Heal(1);
            Destroy(other.gameObject);
            if (UnifiedInfoSystem.Instance != null)
                UnifiedInfoSystem.Instance.ShowTimedMessage("Здоровье восстановлено! +1 сердце", 2f);
        }
        else if (other.CompareTag("ShieldBonus"))
        {
            StartCoroutine(ApplyShield());
            Destroy(other.gameObject);
            if (UnifiedInfoSystem.Instance != null)
                UnifiedInfoSystem.Instance.ShowTimedMessage("Щит активирован! Временно неуязвим", 2f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)  
    {
        if (isInvincible) return;
        currentHealth -= amount;
        UpdateHealthUI();
        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0) Die();
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(invincibleTime / 6f);
        }
        sr.enabled = true;
        isInvincible = false;
    }

    IEnumerator ApplyShield()
    {
        isInvincible = true;
        yield return new WaitForSeconds(5f);
        if (!isInvincible) yield break;
        isInvincible = false;
    }

    void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
                heartImages[i].sprite = (i < currentHealth) ? heartFull : heartBroken;
        }
    }

    void UpdateKillsUI()
    {
        if (killsText != null)
            killsText.text = $"Врагов: {enemiesKilled}/{neededKills}";
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Debug.Log("Game Over");
        GameManager.Instance?.ShowGameOver();
    }

    void CompleteLevel()
    {
        GameManager.Instance?.CompleteLevel("platformer");
        GameManager.Instance?.ShowWin();
    }

    public bool IsInputBlocked()
    {
        if (GameManager.Instance == null) return isDead;
        var gm = GameManager.Instance;
        bool uiActive = (gm.gameOverPanel != null && gm.gameOverPanel.activeSelf) ||
                        (gm.pausePanel != null && gm.pausePanel.activeSelf) ||
                        (gm.winPanel != null && gm.winPanel.activeSelf);
        return isDead || uiActive;
    }

    public void IncrementKillCount()
    {
        enemiesKilled++;
        UpdateKillsUI();

        if (enemiesKilled == 1 && UnifiedInfoSystem.Instance != null)
        {
            string[] firstKillMsg = { "Первый враг повержен! В ранних платформерах врагов либо обходили, либо они были статичными. Позже появились патрулирующие и летающие враги." };
            UnifiedInfoSystem.Instance.ShowDialogue(firstKillMsg, "encyclopedia", "neutral");
        }
        else if (enemiesKilled == neededKills)
        {
            if (UnifiedInfoSystem.Instance != null)
            {
                string[] allKilledMsg = { "Уничтожено 5 врагов! Теперь открыт портал к выходу." };
                UnifiedInfoSystem.Instance.ShowDialogue(allKilledMsg, "encyclopedia", "happy");
            }
        }
        else if (UnifiedInfoSystem.Instance != null && enemiesKilled % 2 == 0)
        {
            UnifiedInfoSystem.Instance.ShowTimedMessage($"Уничтожено врагов: {enemiesKilled}/{neededKills}", 1.5f);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}