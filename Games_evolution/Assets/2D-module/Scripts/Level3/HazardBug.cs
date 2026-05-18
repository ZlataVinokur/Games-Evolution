using UnityEngine;

public class HazardBug : MonoBehaviour
{
    [Header("Stats")]
    public int health = 5;
    public int damage = 15;
    public float moveSpeed = 1.5f;
    public float wanderRadius = 2.5f;
    public float wanderInterval = 1.5f;

    private Vector2 targetPosition;
    private float nextWanderTime;
    private bool hasTriggeredDialogue = false;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        PickNewTarget();
        nextWanderTime = Time.time + wanderInterval;
    }

    void Update()
    {
        if (Time.time >= nextWanderTime)
        {
            PickNewTarget();
            nextWanderTime = Time.time + wanderInterval;
        }
        // Движение к цели
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);
    }

    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomOffset;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            IsometricPlayerController player = col.gameObject.GetComponent<IsometricPlayerController>();
            if (player != null) player.TakeDamage(damage);
            if (!hasTriggeredDialogue)
            {
                hasTriggeredDialogue = true;
                UnifiedInfoSystem.Instance?.ShowDialogue(
                    new string[] { "Ой! Эти баги больно кусаются. В старых RPG такие враги часто встречались в траве. Лучше обходи их стороной или стреляй!" },
                    "encyclopedia", "laugh"
                );
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            health--;
            Destroy(other.gameObject); // снаряд исчезает
            if (sprite != null) sprite.color = Color.red;
            UnifiedInfoSystem.Instance?.ShowTimedMessage($"Багу нанесён урон! Осталось HP: {health}", 0.5f);
            if (health <= 0)
            {
                Die();
            }
            else
            {
                Invoke(nameof(ResetColor), 0.2f);
            }
        }
    }

    void ResetColor()
    {
        if (sprite != null) sprite.color = Color.white;
    }

    void Die()
    {
        // Можно добавить эффект взрыва, частицы
        Destroy(gameObject);
        UnifiedInfoSystem.Instance?.ShowTimedMessage("Баг уничтожен!", 1f);
    }
}