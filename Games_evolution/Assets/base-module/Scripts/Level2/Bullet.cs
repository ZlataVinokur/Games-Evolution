using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Параметры пули")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;
    
    [Header("Тип пули")]
    public bool isEnemyBullet = false;  // <- ЭТО ЕДИНСТВЕННОЕ ИЗМЕНЕНИЕ
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            Vector2 direction = isEnemyBullet ? Vector2.down : Vector2.up;
            rb.linearVelocity = direction * speed;
        }
        
        Destroy(gameObject, lifeTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyBullet)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage();
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
    
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}