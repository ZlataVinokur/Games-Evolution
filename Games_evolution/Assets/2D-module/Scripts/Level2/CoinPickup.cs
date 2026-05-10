using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Добавляем очки, например в GameManager или Level2Manager
            Destroy(gameObject);
        }
    }
}