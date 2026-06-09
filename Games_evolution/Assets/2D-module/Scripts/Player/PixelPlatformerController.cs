using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PixelPlatformerController : PlayerController_2
{
    [Header("Движение")]
    public float moveSpeed = 5f;
    public float jumpForce = 14f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Телепортация по краям")]
    public bool enableEdgeTeleport = true;
    public float teleportOffset = 0.1f;
    private Camera mainCamera;
    private float leftBound, rightBound;

    [Header("Здоровье")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    private float invincibleTime = 1f;

    [Header("Оружие")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    private bool hasWeapon = false;

    [Header("Враги и прогресс")]
    private int enemiesKilled = 0;
    public int neededKills = 10;
    public float requiredHeight = 150f;
    private bool victoryConditionMet = false;

    [Header("Чекпоинты")]
    public float checkpointInterval = 10f;
    private List<float> checkpoints = new List<float>();
    private int currentCheckpointIndex = 0;
    public float deathFallYOffset = 5f;
    private float lastFallDamageTime = -999f;
    public float fallDamageCooldown = 1f;
    private float fallDamageBlockTimer = 0f;

    [Header("Бонусы")]
    public GameObject healthBonusPrefab;
    public GameObject shieldBonusPrefab;

    [Header("UI")]
    public Image[] heartImages;
    public Sprite heartFull;
    public Sprite heartBroken;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI killsText;

    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip healthBonusSound;
    public AudioClip shieldBonusSound;
    public AudioClip weaponGetSound;
    public AudioClip winSound;
    public AudioClip killSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool firstJumpDone = false;
    private bool weaponUnlockedAndArticleShown = false;
    private bool healthBonusTaken = false;
    private bool shieldBonusTaken = false;
    private bool isStartInvincible = true;
    private float startInvincibleDuration = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.2f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateKillsUI();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (jumpSound != null || damageSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (groundCheck == null)
        {
            GameObject go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = go.transform;
        }

        if (enableEdgeTeleport)
        {
            mainCamera = Camera.main;
            UpdateBounds();
        }

        checkpoints.Clear();
        checkpoints.Add(transform.position.y);
        currentCheckpointIndex = 0;

        StartCoroutine(StartInvincibility());
    }

    IEnumerator StartInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(startInvincibleDuration);
        if (!isStartInvincible) yield break;
        isInvincible = false;
        isStartInvincible = false;
    }

    public override void HandleInput()
    {
        if (IsInputBlocked()) return;
        if (isDead) return;

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        isGrounded = hit.collider != null;

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);

                if (!firstJumpDone)
                {
                    firstJumpDone = true;
                    UnifiedInfoSystem.Instance?.UnlockArticle("platformer_jump");
                }
            }
        }
    }

    void Update()
    {
        if (IsInputBlocked()) return;
        if (isDead) return;

        HandleInput();

        if (enableEdgeTeleport) CheckEdgeTeleport();

        if (heightText != null)
            heightText.text = $"Высота: {Mathf.FloorToInt(transform.position.y)}";

        if (hasWeapon && Input.GetButtonDown("Fire1")) Shoot();

        UpdateCheckpoints();
        CheckFallDamage();

        if (!victoryConditionMet && enemiesKilled >= neededKills && transform.position.y >= requiredHeight)
        {
            victoryConditionMet = true;
            PlaySound(winSound);
            GameManager.Instance?.ShowWin();
        }
    }

    void UpdateCheckpoints()
    {
        float currentY = transform.position.y;
        int newIndex = Mathf.FloorToInt(currentY / checkpointInterval);
        if (newIndex > currentCheckpointIndex)
        {
            for (int i = currentCheckpointIndex + 1; i <= newIndex; i++)
            {
                float cpY = i * checkpointInterval;
                if (!checkpoints.Contains(cpY))
                {
                    checkpoints.Add(cpY);
                    // Можно оставить для отладки
                    // Debug.Log($"[Чекпоинт] Добавлен на высоте {cpY}");
                }
            }
            currentCheckpointIndex = newIndex;
        }
    }

    void CheckFallDamage()
    {
        if (fallDamageBlockTimer > 0f)
        {
            fallDamageBlockTimer -= Time.deltaTime;
            return;
        }
        if (checkpoints.Count == 0) return;
        if (isInvincible || isDead) return;

        // Урон только при падении вниз (вертикальная скорость отрицательна)
        if (rb.linearVelocity.y >= 0f) return;

        float currentCheckpointY = checkpoints[currentCheckpointIndex];
        float threshold = currentCheckpointY - deathFallYOffset;

        // Дополнительная защита: если игрок выше чекпоинта, то падения нет
        if (transform.position.y > currentCheckpointY) return;

        if (transform.position.y < threshold && Time.time >= lastFallDamageTime + fallDamageCooldown)
        {
            Debug.Log($"[Падение] Урон! Высота {transform.position.y:F2} ниже порога {threshold:F2}");
            TakeDamage(1);
            lastFallDamageTime = Time.time;
            TeleportToCheckpoint();
        }
    }

    void TeleportToCheckpoint()
    {
        if (checkpoints.Count == 0) return;
        float targetY = checkpoints[currentCheckpointIndex] + 1f; // чуть выше чекпоинта
        Vector3 pos = transform.position;
        pos.y = targetY;
        transform.position = pos;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        fallDamageBlockTimer = 0.5f;
        StartCoroutine(InvincibilityFrames());

        // Поиск ближайшей платформы под ногами, чтобы не провалиться
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
        if (hit.collider != null)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
        }
    }

    void UpdateBounds()
    {
        if (mainCamera == null) return;
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;
        leftBound = -halfWidth;
        rightBound = halfWidth;
    }

    void CheckEdgeTeleport()
    {
        if (mainCamera == null) return;
        Vector3 pos = transform.position;
        bool teleported = false;
        if (pos.x < leftBound)
        {
            pos.x = rightBound - teleportOffset;
            teleported = true;
        }
        else if (pos.x > rightBound)
        {
            pos.x = leftBound + teleportOffset;
            teleported = true;
        }
        if (teleported) transform.position = pos;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Portal") && victoryConditionMet)
        {
            CompleteLevel();
        }
        else if (other.CompareTag("HealthBonus"))
        {
            Heal(1);
            Destroy(other.gameObject);
            PlaySound(healthBonusSound);
            StartCoroutine(FlashColor(Color.green, 0.3f));
            if (!healthBonusTaken)
            {
                healthBonusTaken = true;
                UnifiedInfoSystem.Instance?.UnlockArticle("platformer_health");
            }
            if (UnifiedInfoSystem.Instance != null)
                UnifiedInfoSystem.Instance.ShowTimedMessage("Здоровье восстановлено! +1 сердце", 2f);
        }
        else if (other.CompareTag("ShieldBonus"))
        {
            StartCoroutine(ApplyShield());
            Destroy(other.gameObject);
            PlaySound(shieldBonusSound);
            StartCoroutine(FlashColor(Color.cyan, 0.3f));
            if (!shieldBonusTaken)
            {
                shieldBonusTaken = true;
                UnifiedInfoSystem.Instance?.UnlockArticle("platformer_shield");
            }
            if (UnifiedInfoSystem.Instance != null)
                UnifiedInfoSystem.Instance.ShowTimedMessage("Щит активирован! Временно неуязвим", 2f);
        }
        else if (other.CompareTag("WeaponPickup"))
        {
            hasWeapon = true;
            Destroy(other.gameObject);
            PlaySound(weaponGetSound);
            StartCoroutine(FlashColor(Color.yellow, 0.3f));
            if (!weaponUnlockedAndArticleShown)
            {
                weaponUnlockedAndArticleShown = true;
                UnifiedInfoSystem.Instance?.UnlockArticle("platformer_weapon");
            }
            VerticalPlatformGenerator gen = FindObjectOfType<VerticalPlatformGenerator>();
            if (gen != null) gen.OnWeaponCollected();
            if (UnifiedInfoSystem.Instance != null)
            {
                string[] fact = { "Теперь у тебя есть оружие! Стреляй левой кнопкой мыши." };
                UnifiedInfoSystem.Instance.ShowDialogue(fact, "encyclopedia", "happy");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y < -0.5f) // сверху
            {
                Enemy_2 enemy = collision.gameObject.GetComponent<Enemy_2>();
                if (enemy != null)
                {
                    enemy.TakeDamage();
                    PlaySound(killSound);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.6f);
                }
            }
            else if (!isInvincible && !isDead)
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;
        currentHealth -= amount;
        UpdateHealthUI();
        PlaySound(damageSound);
        StartCoroutine(FlashColor(Color.red, 0.2f));
        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashColor(Color color, float duration)
    {
        if (spriteRenderer == null) yield break;
        float elapsed = 0;
        Color original = spriteRenderer.color;
        while (elapsed < duration)
        {
            spriteRenderer.color = Color.Lerp(original, color, Mathf.PingPong(elapsed * 4f, 1f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = original;
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
            if (heartImages[i] != null)
                heartImages[i].sprite = (i < currentHealth) ? heartFull : heartBroken;
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
        PlaySound(deathSound);
        GameManager.Instance?.ShowGameOver();
    }

    public void CompleteLevel()
    {
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

        if (enemiesKilled == 1)
        {
            UnifiedInfoSystem.Instance?.UnlockArticle("platformer_first_kill");
        }
        else if (enemiesKilled == neededKills)
        {
            UnifiedInfoSystem.Instance?.UnlockArticle("platformer_portal");
        }
        else if (enemiesKilled % 2 == 0 && UnifiedInfoSystem.Instance != null)
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

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}