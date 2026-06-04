using UnityEngine;

public class NewBoss : MonoBehaviour
{
    public int maxHealth = 80;
    private int currentHealth;
    public float moveSpeed = 2.5f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private Transform player;
    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        UnifiedInfoSystem.Instance?.ShowTimedMessage("Финальный страж появился! У него 80 HP.", 2f);
    }

    void Update()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) < 1.2f && Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            player.GetComponent<IsometricPlayerController>()?.TakeDamage(15);
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Страж атакует! -15 здоровья.", 0.5f);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FloatingTextManager.Instance?.ShowDamage(transform.position, damage);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Ты победил стража! Возвращайся к порталу." }, "encyclopedia", "happy");
        RPGLevelManager.Instance?.OnBossDefeated();
        Destroy(gameObject);
    }
}