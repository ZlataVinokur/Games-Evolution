using UnityEngine;

 
public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HealthSystem health = other.GetComponent<HealthSystem>();
                if (health != null && health.CurrentHealth < 5) health.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
 