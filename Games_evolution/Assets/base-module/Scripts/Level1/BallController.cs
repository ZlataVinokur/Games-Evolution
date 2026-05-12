using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isLaunched = false;
    private Transform paddle;
    
    [SerializeField] private float launchSpeed = 5f;
    private bool gameStarted = false;
    private float originalSpeed;
    private Coroutine speedCoroutine;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddle = GameObject.FindGameObjectWithTag("Paddle").transform;
        transform.SetParent(paddle);
        transform.localPosition = new Vector3(0, 0.5f, 0);
        originalSpeed = launchSpeed;
    }
    
    void Update()
    {
        if (!gameStarted) return;
        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }
        if (!isLaunched && paddle != null)
        {
            transform.position = new Vector3(paddle.position.x, paddle.position.y + 0.5f, 0);
        }
    }
   public void SetGameStarted(bool started)
   {
        gameStarted = started;
    }
    void LaunchBall()
    {
        isLaunched = true;
        transform.SetParent(null);
        float randomX = Random.Range(-0.5f, 0.5f);
        Vector2 direction = new Vector2(randomX, 1f).normalized;
        rb.linearVelocity = direction * launchSpeed;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPoint = transform.position.x - collision.transform.position.x;
            float paddleWidth = collision.collider.bounds.size.x;
            float normalizedHit = hitPoint / (paddleWidth / 2f);
            float angle = normalizedHit * 60f;
            Vector2 newDirection = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
            float currentSpeed = rb.linearVelocity.magnitude;
            rb.linearVelocity = newDirection * currentSpeed;
        }
    }
    
    public void ResetBall()
    {
        isLaunched = false;
        rb.linearVelocity = Vector2.zero;
        GameObject paddleObject = GameObject.FindGameObjectWithTag("Paddle");
        if (paddleObject != null)
        {
            paddle = paddleObject.transform;
            transform.SetParent(paddle);
            transform.localPosition = new Vector3(0, 0.6f, 0);
        }
    }
    
    // Метод для бонуса "Ускорение мяча"
    public void IncreaseSpeedTemporarily(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed > 0)
            originalSpeed = currentSpeed;
        else
            originalSpeed = launchSpeed;
        
        rb.linearVelocity = rb.linearVelocity.normalized * (originalSpeed * multiplier);
        speedCoroutine = StartCoroutine(ResetSpeedAfterDelay(duration));
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.linearVelocity = rb.linearVelocity.normalized * originalSpeed;
    }
}