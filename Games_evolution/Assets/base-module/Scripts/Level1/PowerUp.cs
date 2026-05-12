using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type { ExpandPaddle, SpeedUpBall, ExtraLife }
    public Type type;

    [SerializeField] private float fallSpeed = 2f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.down * fallSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paddle"))
        {
            ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("DeadZone"))
        {
            Destroy(gameObject);
        }
    }

    void ApplyEffect(GameObject paddle)
    {
        switch (type)
        {
            case Type.ExpandPaddle:
                PaddleController paddleCtrl = paddle.GetComponent<PaddleController>();
                if (paddleCtrl != null) paddleCtrl.ExpandTemporarily(1.8f, 5f);
                break;
            case Type.SpeedUpBall:
                BallController ball = FindObjectOfType<BallController>();
                if (ball != null) ball.IncreaseSpeedTemporarily(1.5f, 5f);
                break;
            case Type.ExtraLife:
                LivesDisplay lives = FindObjectOfType<LivesDisplay>();
                if (lives != null) lives.AddLife();
                break;
        }
    }
}