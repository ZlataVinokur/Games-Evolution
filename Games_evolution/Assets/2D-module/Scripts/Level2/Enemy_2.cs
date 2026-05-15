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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PixelPlatformerController>()?.TakeDamage(1);
        }
    }
}