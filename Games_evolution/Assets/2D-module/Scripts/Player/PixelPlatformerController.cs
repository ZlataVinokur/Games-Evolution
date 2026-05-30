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
    //public int extraJumps = 1;          // количество дополнительных прыжков (двойной прыжок = 1)
    //private int currentExtraJumps;      // текущее количество доступных доп. прыжков
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
    public float weaponUnlockHeight = 20f;
    private bool hasWeapon = false;

    [Header("Враги и прогресс")]
    private int enemiesKilled = 0;
    public int neededKills = 10;
    public float requiredHeight = 150f;      // необходимая высота для победы
    private bool victoryConditionMet = false; // чтобы не вызывать победу несколько раз
    public GameObject portalPrefab;           // опционально, если нужен портал (пока не используется)
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

    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip healthBonusSound;
    public AudioClip shieldBonusSound;
    public AudioClip weaponGetSound;
    public AudioClip winSound;
    public AudioClip killSound;   // опционально

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private bool isDead = false;

    // Флаги для статей
    private bool firstJumpDone = false;
    private bool weaponUnlockedAndArticleShown = false;
    private bool healthBonusTaken = false;
    private bool shieldBonusTaken = false;

    public int CurrentHealth => currentHealth;
    public int EnemiesKilled => enemiesKilled;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateKillsUI();

        //currentExtraJumps = extraJumps;

        //// AudioSource, если не назначен
        //if (audioSource == null)
        //    audioSource = GetComponent<AudioSource>();
        //if (audioSource == null && (jumpSound != null || damageSound != null))
        //    audioSource = gameObject.AddComponent<AudioSource>();
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
    }

    public override void HandleInput()
    {
        if (IsInputBlocked()) return;
        if (isDead) return;

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();

        // Новая проверка земли через Raycast (более надёжно)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        isGrounded = hit.collider != null;
        Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.green);

        //// Сброс дополнительных прыжков при касании земли
        //if (isGrounded)
        //{
        //    currentExtraJumps = extraJumps;
        //}

        // Прыжок (на Space, W, UpArrow)
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
                //StartCoroutine(JumpSquashAndStretch());

                if (!firstJumpDone)
                {
                    firstJumpDone = true;
                    UnifiedInfoSystem.Instance?.UnlockArticle("platformer_jump");
                }
            }
            //else if (currentExtraJumps > 0)
            //{
            //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //    currentExtraJumps--;
            //    PlaySound(jumpSound);
            //    StartCoroutine(JumpSquashAndStretch());
            //}
        }
    }

    //// Эффект сжатия и растяжения при прыжке
    //IEnumerator JumpSquashAndStretch()
    //{
    //    Vector3 originalScale = transform.localScale;
    //    // Сжатие по Y, растяжение по X
    //    transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
    //    yield return new WaitForSeconds(0.1f);
    //    transform.localScale = originalScale;
    //    // Небольшое растяжение в верхней точке
    //    yield return new WaitForSeconds(0.1f);
    //    transform.localScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.1f, originalScale.z);
    //    yield return new WaitForSeconds(0.1f);
    //    transform.localScale = originalScale;
    //}

    void Update()
    {
        HandleInput();
        if (IsInputBlocked()) return;
        if (isDead) return;

        if (enableEdgeTeleport) CheckEdgeTeleport();

        if (heightText != null)
            heightText.text = $"Высота: {Mathf.FloorToInt(transform.position.y)}";

        if (hasWeapon && Input.GetButtonDown("Fire1")) Shoot();

        if (!hasWeapon && transform.position.y >= weaponUnlockHeight)
            UnlockWeapon();

        // Проверка победы: убито достаточно врагов И достигнута нужная высота
        if (!victoryConditionMet && enemiesKilled >= neededKills && transform.position.y >= requiredHeight)
        {
            victoryConditionMet = true;
            PlaySound(winSound);
            GameManager.Instance?.ShowWin();
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

    void UnlockWeapon()
    {
        hasWeapon = true;
        PlaySound(weaponGetSound);
        if (!weaponUnlockedAndArticleShown)
        {
            weaponUnlockedAndArticleShown = true;
            UnifiedInfoSystem.Instance?.UnlockArticle("platformer_weapon");
        }
        if (UnifiedInfoSystem.Instance != null)
        {
            string[] fact = { "В 1985 году в игре Super Mario Bros. появилась возможность стрелять. А в 1987-м Contra сделала стрельбу главной механикой. Теперь и ты вооружён!" };
            UnifiedInfoSystem.Instance.ShowDialogue(fact, "encyclopedia", "happy");
        }
        Debug.Log("Оружие получено!");
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
            if (!shieldBonusTaken)
            {
                shieldBonusTaken = true;
                UnifiedInfoSystem.Instance?.UnlockArticle("platformer_shield");
            }
            if (UnifiedInfoSystem.Instance != null)
                UnifiedInfoSystem.Instance.ShowTimedMessage("Щит активирован! Временно неуязвим", 2f);
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
            else if (!isInvincible)
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;
        currentHealth -= amount;
        UpdateHealthUI();
        PlaySound(damageSound);
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

        if (enemiesKilled == 1)
        {
            UnifiedInfoSystem.Instance?.UnlockArticle("platformer_first_kill");
        }
        else if (enemiesKilled == neededKills)
        {
            UnifiedInfoSystem.Instance?.UnlockArticle("platformer_portal");
            // Дополнительный диалог не нужен, аннотация из статьи покажется автоматически
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