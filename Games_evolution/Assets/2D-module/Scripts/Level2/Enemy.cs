using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected int points = 1;
    [SerializeField] protected int health = 1;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int damageToPlayer = 1;

    protected Transform player;
    protected PlatformerController playerController;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null) playerController = player.GetComponent<PlatformerController>();
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Сообщить менеджеру уровня
        Level2Manager manager = FindObjectOfType<Level2Manager>();
        if (manager != null) manager.RegisterKill(points);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Проверка щита
            Shield shield = collision.gameObject.GetComponent<Shield>();
            if (shield != null && shield.IsActive)
            {
                shield.Deactivate(); // щит снимается
                return;
            }
            // Наносим урон игроку
            HealthSystem healthSys = collision.gameObject.GetComponent<HealthSystem>();
            if (healthSys != null) healthSys.TakeDamage(damageToPlayer);
        }
    }
}
