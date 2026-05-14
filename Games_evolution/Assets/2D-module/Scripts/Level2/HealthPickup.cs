using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;   // сколько сердец восстановить

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlatformerController player = other.GetComponent<PlatformerController>();
            if (player != null && player.Health < 5)   // восстанавливаем только если здоровье не полное
            {
                player.Heal(healAmount);
                EncyclopediaManager.Instance.UnlockArticle("platformer_pickups");
                Destroy(gameObject);
            }
        }
    }
}