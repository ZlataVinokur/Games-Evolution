using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isLaunched = false;
    private Transform paddle;
    
    [SerializeField] private float launchSpeed = 7f;
    [SerializeField] private float maxSpeed = 15f;
    private float originalSpeed;
    private Coroutine speedCoroutine;
    
    private int wallHitCount = 0;
    private const int maxWallHitsWithoutInteraction = 4;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        GameObject paddleObject = GameObject.FindGameObjectWithTag("Paddle");
        if (paddleObject != null)
        {
            paddle = paddleObject.transform;
            transform.SetParent(paddle);
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        else
        {
            Debug.LogError("Paddle not found! Make sure it has tag 'Paddle'");
        }
        
        originalSpeed = launchSpeed;
    }
    
    void Update()
    {
        // Запуск по пробелу
        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }
        
        // Если не запущен, следуем за платформой
        if (!isLaunched && paddle != null)
        {
            transform.position = new Vector3(paddle.position.x, paddle.position.y + 0.5f, 0);
        }
        
        // Ограничение максимальной скорости
        if (isLaunched && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
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
            wallHitCount = 0;
            float hitPoint = transform.position.x - collision.transform.position.x;
            float paddleWidth = collision.collider.bounds.size.x;
            float normalizedHit = hitPoint / (paddleWidth / 2f);
            float angle = normalizedHit * 60f;
            Vector2 newDirection = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;
            float currentSpeed = rb.linearVelocity.magnitude;
            rb.linearVelocity = newDirection * currentSpeed;
        }
        else if (collision.gameObject.CompareTag("Brick"))
        {
            wallHitCount = 0;
            collision.gameObject.GetComponent<Brick>()?.Hit();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            wallHitCount++;
            if (wallHitCount >= maxWallHitsWithoutInteraction)
            {
                Vector2 vel = rb.linearVelocity;
                if (Mathf.Abs(vel.y) < 0.5f)
                    vel.y = Random.Range(0.5f, 1f);
                else
                    vel.y += Random.Range(-0.3f, 0.3f);
                rb.linearVelocity = vel.normalized * vel.magnitude;
                wallHitCount = 0;
                Debug.Log("Коррекция зацикленного полёта мяча");
            }
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
    
    public void IncreaseSpeedTemporarily(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed > 0)
            originalSpeed = currentSpeed;
        else
            originalSpeed = launchSpeed;
        
        Vector2 newVel = rb.linearVelocity.normalized * (originalSpeed * multiplier);
        if (newVel.magnitude > maxSpeed)
            newVel = newVel.normalized * maxSpeed;
        rb.linearVelocity = newVel;
        
        speedCoroutine = StartCoroutine(ResetSpeedAfterDelay(duration));
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb.linearVelocity.magnitude > 0)
            rb.linearVelocity = rb.linearVelocity.normalized * originalSpeed;
        else
            rb.linearVelocity = Vector2.up * originalSpeed;
    }
}