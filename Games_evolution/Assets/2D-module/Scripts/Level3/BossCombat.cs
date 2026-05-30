using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public float moveSpeed = 2f;
    private Transform player;
    private bool isDead = false;
    public GameObject deathPortal;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead || player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) < 1f && Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            var playerController = player.GetComponent<IsometricPlayerController>();
            if (playerController != null)
                playerController.TakeDamage(10);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlayerProjectile") && !isDead)
        {
            //int damage = 10 + (RPGLevelManager.Instance.HasTamagotchiBuff() ? 5 : 0);
            //TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Босс получает {damage} урона! Осталось {currentHealth}", 0.5f);
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
        if (deathPortal != null)
            Instantiate(deathPortal, transform.position, Quaternion.identity);
        //RPGLevelManager.Instance.OnBossDefeated();
    }
}