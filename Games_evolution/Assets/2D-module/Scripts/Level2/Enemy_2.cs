using UnityEngine;

public abstract class Enemy_2 : MonoBehaviour
{
    public int health = 1;

    public virtual void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player?.GetComponent<PixelPlatformerController>()?.IncrementKillCount();
            Destroy(gameObject);
        }
    }

    // Пули попадают через триггер (у пули isTrigger = true)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }

    // Столкновение с игроком (не триггер)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Получаем нормаль первого контакта
            ContactPoint2D contact = collision.contacts[0];
            // Если нормаль направлена вниз (Y примерно -1), игрок приземлился сверху
            if (contact.normal.y < -0.5f)
            {
                TakeDamage(); // убиваем врага
                // Отскок игрока
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 7f);
                }
            }
            else // боковое столкновение
            {
                collision.gameObject.GetComponent<PixelPlatformerController>()?.TakeDamage(1);
            }
        }
    }
}