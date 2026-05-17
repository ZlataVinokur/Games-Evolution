using UnityEngine;

public class IsometricPlayerController : PlayerController_2
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject[] heartIcons; // ссылки на иконки сердец (UI Image)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Нет Rigidbody2D на игроке!");
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public override void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, vertical, 0).normalized;
    }

    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj == null) { Debug.LogError("У снаряда нет Rigidbody2D!"); Destroy(proj); return; }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
        rbProj.linearVelocity = direction * projectileSpeed;

        Destroy(proj, 2f);
    }

    public override void Move()
    {
        if (rb == null) return;
        Vector3 newPos = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPos);

        if (anim != null)
        {
            if (moveDirection != Vector3.zero)
            {
                anim.SetFloat("MoveX", moveDirection.x);
                anim.SetFloat("MoveY", moveDirection.y);
                anim.SetBool("IsMoving", true);
            }
            else
                anim.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    // --- Health methods ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        int hearts = Mathf.CeilToInt((float)currentHealth / maxHealth * heartIcons.Length);
        for (int i = 0; i < heartIcons.Length; i++)
            heartIcons[i].SetActive(i < hearts);
    }

    void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ShowGameOver();
        else
            Debug.Log("Игрок умер");
    }
}