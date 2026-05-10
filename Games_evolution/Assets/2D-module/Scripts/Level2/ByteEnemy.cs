using UnityEngine;


public class ByteEnemy : Enemy
    {
        [SerializeField] private float direction = 1f;

        private void Update()
        {
            transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Wall"))
            {
                direction *= -1;
                FlipSprite();
            }
        }

        private void FlipSprite()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
