using UnityEngine;

public class FlyerEnemy : EnemyBase
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // ƒвижение влево и синусоида
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x,
            startPos.y + Mathf.Sin(Time.time * frequency) * amplitude, 0);

        // ”ничтожить, если ушЄл за левый край
        if (transform.position.x < -10f)
            Destroy(gameObject);
    }

    public override void Die()
    {
        PlatformerController.Instance.AddKill();
        Destroy(gameObject);
    }
}