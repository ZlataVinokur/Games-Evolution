using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private int health = 1;          // Прочность кирпича
    [SerializeField] private int points = 10;         // Очки за разрушение
    
    private SpriteRenderer spriteRenderer;
    public GameObject powerUpPrefab;

    public float powerUpChance = 0.2f;
    // Цвета для разной прочности
    private Color[] healthColors = {
        Color.red,      // 1 удар
        Color.yellow,   // 2 удара
        Color.green     // 3 удара
    };
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBrick();
            
        }
    }
    public void SetHealth(int newHealth)
    {
        health = newHealth;
        UpdateColor();
    }
    public void Hit()
    {
        health--;
        
        if (health <= 0)
        {
            DestroyBrick();
        }
        else
        {
            UpdateColor();
            // Можно добавить эффект "мигания" при попадании
            StartCoroutine(FlashWhite());
        }
    }
    
    void DestroyBrick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
            GameManager.Instance.BrickDestroyed();
        }
        
        if (powerUpPrefab != null && Random.value < powerUpChance)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            PowerUp pu = powerUp.GetComponent<PowerUp>();
            if (pu != null)
            {
                int rand = Random.Range(0, 3);
                pu.type = (PowerUp.Type)rand;
            }
        }
        Destroy(gameObject);
    }
    
    void UpdateColor()
    {
        if (spriteRenderer != null && health > 0 && health <= healthColors.Length)
        {
            spriteRenderer.color = healthColors[health - 1];
        }
    }
    
    System.Collections.IEnumerator FlashWhite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = originalColor;
    }
    
    // Обработка столкновения с мячом
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Hit();
        }
    }
    
}