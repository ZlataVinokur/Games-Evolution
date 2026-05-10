using UnityEngine;


public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        private Vector2 moveDirection;

        public void Init(Vector2 dir)
        {
            moveDirection = dir.normalized;
            // Поворачиваем снаряд в направлении
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Destroy(gameObject, 3f); // самоуничтожение
        }

        private void Update()
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Уничтожаем врага, если попали
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null) enemy.TakeDamage(1);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Platform") || other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
