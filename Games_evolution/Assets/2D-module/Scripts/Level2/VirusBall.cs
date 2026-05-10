using UnityEngine;


public class VirusBall : Enemy
{
    [SerializeField] private float fallSpeed = 3f;

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // Игнорируем столкновения с платформами (проходит насквозь)
        if (collision.gameObject.CompareTag("Platform"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
