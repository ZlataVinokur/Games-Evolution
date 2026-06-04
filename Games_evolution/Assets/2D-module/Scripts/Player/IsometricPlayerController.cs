using UnityEngine;

public class IsometricPlayerController : PlayerController_2
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public AudioSource footstepSource;
    public float footstepInterval = 0.5f;
    private float nextFootstepTime;
    private Vector3 moveDirection;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public AudioSource shootSource;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject[] heartIcons;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public override void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, v, 0).normalized;
    }

    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootAtNearestEnemy();
        }
    }

    public int damageBonus = 0;

    public void HealFull()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void ShootAtNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f);
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDist) { minDist = dist; nearest = col.transform; }
            }
        }
        if (nearest == null) return;
        Vector2 direction = (nearest.position - firePoint.position).normalized;
        ShootInDirection(direction, damageBonus);
    }

    void ShootInDirection(Vector2 direction, int bonusDamage)
    {
        if (projectilePrefab == null || firePoint == null) return; // ÑÍÀ×ÀËÀ ÏÐÎÂÅÐÊÀ

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null) projScript.damage = 10 + bonusDamage;

        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj == null) { Destroy(proj); return; }
        rbProj.linearVelocity = direction * projectileSpeed;
        shootSource?.Play();
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
                if (footstepSource != null && Time.time >= nextFootstepTime)
                {
                    nextFootstepTime = Time.time + footstepInterval;
                    footstepSource.Play();
                }
            }
            else anim.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate() => Move();

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
        FloatingTextManager.Instance?.ShowDamage(transform.position, damage);
    }

    void UpdateHealthUI()
    {
        int hearts = Mathf.CeilToInt((float)currentHealth / maxHealth * heartIcons.Length);
        for (int i = 0; i < heartIcons.Length; i++)
            heartIcons[i].SetActive(i < hearts);
    }

    void Die() => GameManager.Instance?.ShowGameOver();
}