using UnityEngine;

 
public class Wall : MonoBehaviour
    {
        [SerializeField] private float knockbackForce = 8f;
        [SerializeField] private int damage = 1;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Направление от стены
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                PlatformerController controller = collision.gameObject.GetComponent<PlatformerController>();
                if (controller != null) controller.Knockback(dir, knockbackForce);

                HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
                if (health != null) health.TakeDamage(damage);
            }
        }
    }
 