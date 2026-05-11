using UnityEngine;

public class TeleporterEnemy : EnemyBase
{
    [SerializeField] private float teleportInterval = 3f;
    [SerializeField] private float minY, maxY, minX, maxX;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= teleportInterval)
        {
            Teleport();
            timer = 0;
        }
    }

    void Teleport()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        transform.position = new Vector2(x, y);
    }

    public override void Die()
    {
        PlatformerController.Instance.AddKill();
        Destroy(gameObject);
    }
}