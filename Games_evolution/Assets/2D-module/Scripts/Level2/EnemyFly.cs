using UnityEngine;

public class EnemyFly : Enemy_2
{
    public float speed = 2f;
    public float leftBound = -5f;
    public float rightBound = 5f;
    private int direction = 1;

    void Update()
    {
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
        if (transform.position.x >= rightBound) direction = -1;
        if (transform.position.x <= leftBound) direction = 1;
    }
}