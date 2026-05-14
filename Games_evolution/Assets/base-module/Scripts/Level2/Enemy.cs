using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Основные параметры")]
    [SerializeField] protected int health = 1;
    [SerializeField] protected int scoreValue = 100;
    [SerializeField] protected float shootCooldown = 2f;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;
    
    [Header("Визуальные эффекты")]
    [SerializeField] protected GameObject deathEffectPrefab;
    [SerializeField] protected GameObject hitEffectPrefab;
    
    protected float shootTimer;
    protected bool canShoot = true;
    
    protected virtual void Start()
    {
        shootTimer = shootCooldown;
    }
    
    protected virtual void Update()
    {
        // Обработка стрельбы
        if (canShoot)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
        }
    }
    
    protected virtual void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isEnemyBullet = true;
            }
        }
    }
    
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        
        // Эффект попадания
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);
        else
            Debug.LogWarning("ScoreManager not found! Score not added.");
        
        // Эффект смерти
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Уведомляем WaveSpawner о смерти врага
        WaveSpawner waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveSpawner != null)
        {
            waveSpawner.OnEnemyDestroyed();
        }
        
        Destroy(gameObject);
   
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Если враг достиг нижней части экрана - игрок проигрывает
        if (other.CompareTag("BottomBoundary"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.GameOver();
            }
        }
    }
}
