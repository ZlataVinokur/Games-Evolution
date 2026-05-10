
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Shield shield = other.GetComponent<Shield>();
            if (shield == null) shield = other.gameObject.AddComponent<Shield>();
            shield.Activate();
            Destroy(gameObject);
        }
    }
}