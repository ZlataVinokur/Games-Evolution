using UnityEngine;

public class HazardBug : MonoBehaviour
{
    public static System.Action OnAnyBugDeath;

    [Header("Stats")]
    public int health = 5;
    public int damage = 15;
    public float moveSpeed = 1.5f;
    public float wanderRadius = 2.5f;
    public float wanderInterval = 1.5f;

    public AudioSource hurtSound;
    public AudioSource deathSound;

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
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);
    }

    void PickNewTarget() => targetPosition = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<IsometricPlayerController>()?.TakeDamage(damage);
            if (!hasTriggeredDialogue)
            {
                hasTriggeredDialogue = true;
                UnifiedInfoSystem.Instance?.ShowDialogue(
                    new[] { "Ой! Эти баги больно кусаются. В старых RPG такие враги часто встречались в траве. Лучше обходи их стороной или стреляй!" },
                    "encyclopedia", "laugh");
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        hurtSound?.Play();
        if (sprite != null) sprite.color = Color.red;
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Багу нанесён урон! Осталось HP: {health}", 0.5f);
        if (health <= 0) Die();
        else Invoke(nameof(ResetColor), 0.2f);
    }

    void ResetColor() { if (sprite != null) sprite.color = Color.white; }

    void Die()
    {
        Debug.Log($"Баг умирает, вызываем OnAnyBugDeath");
        deathSound?.Play();
        OnAnyBugDeath?.Invoke();
        Destroy(gameObject);
    }
}