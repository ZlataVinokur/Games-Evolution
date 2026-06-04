using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 3f;
    public bool isChaser = false;
    
    private Vector2 targetPosition;
    private Vector2 direction;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float changeDirectionTimer;
    private float changeDirectionInterval = 1f;
    
    // Для анимации пульсации
    private float bobSpeed = 2f;
    private float bobHeight = 0.05f; // изменение масштаба

    void Start()
    {
        targetPosition = transform.position;
        direction = GetRandomDirection();
        changeDirectionTimer = changeDirectionInterval;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (GameController.Instance != null && !GameController.Instance.IsGameStarted())
            return;
        Move();
        
        if (!isChaser)
        {
            changeDirectionTimer -= Time.deltaTime;
            if (changeDirectionTimer <= 0)
            {
                changeDirectionTimer = changeDirectionInterval;
                ChooseNewDirection();
                targetPosition = (Vector2)transform.position + direction;
            }
        }
        
        // Анимация: пульсация масштаба (не ломает движение)
        float scale = 1f + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
        {
            ChooseNewDirection();
            targetPosition = (Vector2)transform.position + direction;
        }
    }

    void ChooseNewDirection()
    {
        if (isChaser && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            
            Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            
            System.Array.Sort(dirs, (a, b) => {
                float dotA = Vector2.Dot(directionToPlayer, a);
                float dotB = Vector2.Dot(directionToPlayer, b);
                return dotB.CompareTo(dotA);
            });
            
            foreach (Vector2 newDir in dirs)
            {
                if (CanMove(newDir) && newDir != -direction)
                {
                    direction = newDir;
                    return;
                }
            }
        }
        else
        {
            Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            
            for (int i = 0; i < dirs.Length; i++)
            {
                int randomIndex = Random.Range(0, dirs.Length);
                Vector2 temp = dirs[i];
                dirs[i] = dirs[randomIndex];
                dirs[randomIndex] = temp;
            }
            
            foreach (Vector2 newDir in dirs)
            {
                if (CanMove(newDir) && newDir != -direction)
                {
                    direction = newDir;
                    return;
                }
            }
        }
    }

    bool CanMove(Vector2 dir)
    {
        Vector2 newPos = (Vector2)transform.position + dir;
        Collider2D hit = Physics2D.OverlapCircle(newPos, 0.2f, LayerMask.GetMask("Wall"));
        return hit == null;
    }

    Vector2 GetRandomDirection()
    {
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        return dirs[Random.Range(0, dirs.Length)];
    }

    public void SetEdible(bool isEdible)
    {
        if (spriteRenderer != null)
        {
            if (isEdible)
            {
                spriteRenderer.color = Color.blue;
                Debug.Log(gameObject.name + " стал синим!");
            }
            else
            {
                spriteRenderer.color = originalColor;
                Debug.Log(gameObject.name + " вернул цвет!");
            }
        }
        else
        {
            Debug.LogWarning("Нет SpriteRenderer на " + gameObject.name);
        }
    }

    public void Eaten()
    {
        transform.position = Vector2.zero;
        
        if (GameController.Instance != null)
            GameController.Instance.score += 200;
        
        gameObject.SetActive(false);
        Invoke("Respawn", 5f);
    }

    void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = Vector2.zero;
        direction = GetRandomDirection();
        targetPosition = (Vector2)transform.position + direction;
        
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}