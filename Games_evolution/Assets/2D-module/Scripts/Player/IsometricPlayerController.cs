using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricPlayerController : PlayerController_2
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Grid and Bounds")]
    public Grid grid;
    public Tilemap wallsTilemap; // для проверки препятствий
    private BoundsInt bounds;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (grid == null) grid = FindObjectOfType<Grid>();
        // Определяем границы тайловой карты (опционально)
    }

    public override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, vertical, 0).normalized;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position;
        direction.Normalize();
        proj.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        // Уничтожить снаряд через 2 секунды
        Destroy(proj, 2f);
    }

    public override void Move()
    {
        Vector3 newPos = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        // Проверка коллизии с тайлами (можно через Tilemap Collider, но Rigidbody2D уже будет работать)
        // Для изометрии: перемещаем в мировых координатах, физика сама обработает коллизии.
        rb.MovePosition(newPos);

        // Анимация: вычисляем направление (основные 8 направлений)
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            string dir = GetDirectionFromAngle(angle);
            anim.SetFloat("MoveX", moveDirection.x);
            anim.SetFloat("MoveY", moveDirection.y);
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    private string GetDirectionFromAngle(float angle)
    {
        // Для изометрии направления: East = 0°, NorthEast = 45°, North = 90° и т.д.
        if (angle >= -22.5f && angle < 22.5f) return "E";
        if (angle >= 22.5f && angle < 67.5f) return "NE";
        if (angle >= 67.5f && angle < 112.5f) return "N";
        if (angle >= 112.5f && angle < 157.5f) return "NW";
        if (angle >= -157.5f && angle < -112.5f) return "SW";
        if (angle >= -112.5f && angle < -67.5f) return "S";
        if (angle >= -67.5f && angle < -22.5f) return "SE";
        return "W";
    }

    void FixedUpdate()
    {
        HandleInput();
        Move();
    }
}