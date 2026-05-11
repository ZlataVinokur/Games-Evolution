using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        // Повернуть пулю в направлении
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBase>()?.Die();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}