using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController3 : MonoBehaviour
{
    public float speed = 5f;
    public int lives = 3;                       // текущее количество жизней
    public float invincibleDuration = 1.5f;    // длительность неуязвимости после попадания

    private Vector2 targetPosition;
    private Vector2 desiredDirection;
    private Vector2 currentDirection;
    private bool isTeleporting = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isMoving = false;

    // Для неуязвимости
    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    private bool controlsEnabled = true;

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
    void Start()
    {
        targetPosition = transform.position;
        currentDirection = Vector2.right;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Уведомляем UI о начальном количестве жизней
        if (GameController.Instance != null)
            GameController.Instance.UpdateLives(lives);
    }

    void Update()
    {
        if (!controlsEnabled) return;
        HandleInput();
        Move();
        UpdateFacingDirection();
        UpdateAnimator();

        // Таймер неуязвимости
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                // Восстанавливаем видимость (на случай, если мигание остановилось)
                if (spriteRenderer != null)
                    spriteRenderer.enabled = true;
            }
        }
    }

    void HandleInput()
    {
        isMoving = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            desiredDirection = Vector2.up;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            desiredDirection = Vector2.down;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            desiredDirection = Vector2.left;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            desiredDirection = Vector2.right;
            isMoving = true;
        }
    }

    void Move()
    {
        if (isTeleporting) return;

        if (!isMoving)
        {
            targetPosition = transform.position;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (CanMove(desiredDirection))
            {
                currentDirection = desiredDirection;
                targetPosition = (Vector2)transform.position + currentDirection;
            }
            else if (CanMove(currentDirection))
            {
                targetPosition = (Vector2)transform.position + currentDirection;
            }
        }
    }

    void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;

        if (desiredDirection == Vector2.left)
            spriteRenderer.flipX = true;
        else if (desiredDirection == Vector2.right)
            spriteRenderer.flipX = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMovingHorizontal = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        animator.SetBool("IsWalking", isMovingHorizontal);

        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalInput = -1f;
        else if (Input.GetKey(KeyCode.DownArrow)) verticalInput = 1f;
        animator.SetFloat("MoveY", verticalInput);
    }

    bool CanMove(Vector2 direction)
    {
        Vector2 newPos = (Vector2)transform.position + direction;
        Collider2D hit = Physics2D.OverlapCircle(newPos, 0.2f, LayerMask.GetMask("Wall"));
        return hit == null;
    }

    void LoseLife()
    {
        if (isInvincible) return; 

        lives--;
        Debug.Log($"Потеряна жизнь. Осталось: {lives}");
        if (GameController.Instance != null)
            GameController.Instance.UpdateLives(lives);

        if (lives <= 0)
        {
            GameManager manager = FindObjectOfType<GameManager>();
            if (manager != null) manager.ShowGameOver();
            else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
             return; 
        }
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        float blinkInterval = 0.1f;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleDuration && isInvincible)
        {
            visible = !visible;
            if (spriteRenderer != null)
                spriteRenderer.enabled = visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        // По окончании мигания убеждаемся, что спрайт видим
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    // ------------------- ТЕЛЕПОРТАЦИЯ -------------------
    public void TeleportTo(Vector2 newPosition, Vector2 exitDirection)
    {
        StartCoroutine(TeleportRoutine(newPosition, exitDirection));
    }

    private IEnumerator TeleportRoutine(Vector2 newPosition, Vector2 exitDirection)
    {
        if (isTeleporting) yield break;
        isTeleporting = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        transform.position = newPosition;
        targetPosition = newPosition;
        currentDirection = exitDirection;
        desiredDirection = exitDirection;

        yield return new WaitForSeconds(0.3f);

        if (col != null) col.enabled = true;
        isTeleporting = false;
    }

    public Vector2 GetCurrentDirection()
    {
        return currentDirection;
    }

    public void OnTeleport(Vector2 savedDirection)
    {
        StartCoroutine(TeleportRoutine(transform.position, savedDirection));
    }

    private IEnumerator ResetTeleportFlag()
    {
        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;
    }

    // ------------------- СТОЛКНОВЕНИЯ -------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) return;
        Debug.Log("Коснулись: " + other.tag);

        if (other.CompareTag("Pellet"))
        {
            Destroy(other.gameObject);
            if (GameController.Instance != null)
                GameController.Instance.EatDot(10, false);
            else
                Debug.LogError("GameController.Instance не найден!");
        }
        else if (other.CompareTag("SuperPellet"))
        {
            Destroy(other.gameObject);
            if (GameController.Instance != null)
                GameController.Instance.EatDot(50, true);
            else
                Debug.LogError("GameController.Instance не найден!");
        }
       else if (other.CompareTag("Ghost"))
       {
            Debug.Log("=== Попадание в привидение! ===");
            if (GameController.Instance == null)
            {
                Debug.LogError("GameController.Instance не найден!");
                return;
            }
            Debug.Log("isPowerUpMode = " + GameController.Instance.isPowerUpMode);
    
            if (GameController.Instance.isPowerUpMode)
            {
                Ghost ghost = other.GetComponent<Ghost>();
                if (ghost != null)
                    ghost.Eaten();
                Debug.Log("Съели привидение");
            }
            else
            {
                Debug.Log("Вызываем LoseLife()");
                LoseLife();
            }
        }
    }
}