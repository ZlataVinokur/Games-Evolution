using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float leftBoundary = -7f;
    [SerializeField] private float rightBoundary = 7f;

    [Header("Стрельба")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int bulletCount = 1;
    private float nextFireTime;

    [Header("Жизни")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;
    private bool isInvincible = false;

    [Header("Улучшения")]
    private float originalFireRate;
    private int originalBulletCount;
    private float powerUpEndTime;
    private bool hasPowerUp = false;
    private Vector3 originalScale;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private LivesDisplay livesDisplay;
    private Animator anim;
    private bool controlsEnabled = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator component not found on player");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentLives = maxLives;
        originalFireRate = fireRate;
        originalBulletCount = bulletCount;
        livesDisplay = FindObjectOfType<LivesDisplay>();
        if (livesDisplay == null)
            Debug.LogWarning("LivesDisplay not found in scene");
        originalScale = transform.localScale;
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }

    void Update()
    {
        if (!controlsEnabled) return;
        HandleMovement();
        HandleShooting();
        FlipSprite();

        if (hasPowerUp && Time.time >= powerUpEndTime)
        {
            DisablePowerUp();
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 newPosition = rb.position + new Vector2(moveInput * speed * Time.deltaTime, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        rb.MovePosition(newPosition);
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown("space") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (anim != null)
            anim.SetTrigger("shoot");
        if (bulletPrefab != null && firePoint != null)
        {
            if (bulletCount == 1)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.isEnemyBullet = false;
                }
            }
            else
            {
                float offset = 0.3f;
                for (int i = 0; i < bulletCount; i++)
                {
                    float xOffset = (i - (bulletCount - 1) / 2f) * offset;
                    Vector3 spawnPos = firePoint.position + new Vector3(xOffset, 0, 0);

                    GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.isEnemyBullet = false;
                    }
                }
            }
        }
    }

    void FlipSprite()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            float newX = Mathf.Abs(originalScale.x) * Mathf.Sign(moveInput);
            transform.localScale = new Vector3(newX, originalScale.y, originalScale.z);
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        if (livesDisplay != null)
            livesDisplay.LoseLife();
        else
            currentLives--;

        if (livesDisplay == null || livesDisplay.GetCurrentLives() <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        float timer = 0;
        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Player died!");
        GameOver();
    }

    public void GameOver()
    {
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void DisablePowerUp()
    {
        hasPowerUp = false;
        fireRate = originalFireRate;
        bulletCount = originalBulletCount;

        StopCoroutine(PowerUpVisualEffect());
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    IEnumerator PowerUpVisualEffect()
    {
        while (hasPowerUp)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.cyan;
                yield return new WaitForSeconds(0.2f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}