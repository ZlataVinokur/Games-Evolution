using UnityEngine;


public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private float speed = 4f;
        private Vector2 moveDir;

        public void Init(Vector2 dir)
        {
            moveDir = dir.normalized;
            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            transform.Translate(moveDir * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HealthSystem health = other.GetComponent<HealthSystem>();
                if (health != null) health.TakeDamage(1);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }