using UnityEngine;

 
public class GlitchEnemy : Enemy
    {
        [SerializeField] private float jumpCooldown = 1.5f;
        [SerializeField] private float jumpHeight = 6f;
        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= jumpCooldown)
            {
                Jump();
                timer = 0;
            }
        }

        private void Jump()
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), 1f) * jumpHeight;
        }
    }
 