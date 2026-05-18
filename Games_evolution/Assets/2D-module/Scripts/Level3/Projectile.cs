using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        // ѕытаемс€ нанести урон боссу
        BossCombat boss = other.GetComponent<BossCombat>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // ѕытаемс€ нанести урон багу
        HazardBug bug = other.GetComponent<HazardBug>();
        if (bug != null)
        {
            bug.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // ≈сли попали в стену или другой объект (не игрок и не другой снар€д) Ц исчезаем
        if (!other.CompareTag("Player") && !other.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject);
        }
    }
}