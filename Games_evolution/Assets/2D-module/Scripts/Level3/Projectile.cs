using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 2f;

    void Start() => Destroy(gameObject, lifeTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<HazardBug>();
            if (enemy != null) enemy.TakeDamage(damage);
            else
            {
                var boss = other.GetComponent<NewBoss>();
                if (boss != null) boss.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject);
        }
    }
}