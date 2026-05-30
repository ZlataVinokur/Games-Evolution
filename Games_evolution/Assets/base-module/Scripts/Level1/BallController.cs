using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isLaunched = false;
    private Transform paddle;
    private bool gameStarted = false;
    [SerializeField] private float launchSpeed = 7f;
    [SerializeField] private float maxSpeed = 15f;
    private float originalSpeed;
    private Coroutine speedCoroutine;
    
    // Таймер для защиты от зацикливания
    private float timeSinceLastUsefulHit = 0f;
    private const float maxIdleTime = 3f; // секунд без касания платформы/кирпича
    public void SetGameStarted(bool started)
    {
        gameStarted = started;
       if (!gameStarted)
       {
        // Проверяем, что rb не null
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            isLaunched = false;
            ResetBall(); // ResetBall сам проверит наличие paddle
        }
    }
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
        if (!gameStarted) return;
        if (!isLaunched && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }
        
        if (!isLaunched && paddle != null)
        {
            transform.position = new Vector3(paddle.position.x, paddle.position.y + 0.5f, 0);
        }
        
        if (isLaunched && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        
        // Защита от зацикливания: если мяч долго не касался платформы/кирпича
        if (isLaunched)
        {
            timeSinceLastUsefulHit += Time.deltaTime;
            if (timeSinceLastUsefulHit >= maxIdleTime)
            {
                // Принудительно изменяем траекторию
                Vector2 vel = rb.linearVelocity;
                // Добавляем случайное отклонение по вертикали
                vel.y += Random.Range(1f, 2f);
                // Также можно немного изменить по горизонтали
                vel.x += Random.Range(-0.5f, 0.5f);
                rb.linearVelocity = vel.normalized * vel.magnitude;
                timeSinceLastUsefulHit = 0f;
                Debug.Log("Принудительная коррекция зацикленного полёта мяча");
            }
        }
    }
    
    void LaunchBall()
    {
        isLaunched = true;
        transform.SetParent(null);
        float randomX = Random.Range(-0.5f, 0.5f);
        Vector2 direction = new Vector2(randomX, 1f).normalized;
        rb.linearVelocity = direction * launchSpeed;
        timeSinceLastUsefulHit = 0f;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Сброс таймера при полезных столкновениях
        if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("Brick"))
        {
            timeSinceLastUsefulHit = 0f;
        }
        
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
        else if (collision.gameObject.CompareTag("Brick"))
        {
            collision.gameObject.GetComponent<Brick>()?.Hit();
        }
        // Стены не сбрасывают таймер (они не считаются полезными)
    }
    
    public void ResetBall()
    {
        isLaunched = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        timeSinceLastUsefulHit = 0f;
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