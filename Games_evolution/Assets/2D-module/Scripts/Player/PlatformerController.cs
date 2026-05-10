using UnityEngine;

public class PlatformerController : PlayerController
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite jumpSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    private bool facingRight = true;
    private bool isGrounded = false;
    private float horizontalInput = 0f;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    // ���������� ������������ ������ �� �������� ������
    public override void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D ��� �������
        if (horizontalInput > 0) facingRight = true;
        else if (horizontalInput < 0) facingRight = false;
    }

    private void Update()
    {
        HandleInput();           // �������� ���������� �����
        UpdateSprite();
    }

    private void FixedUpdate()
    {
        // �������������� ��������
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // �������������� ������ ��� ������� ���������
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // ����������� ������
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void UpdateSprite()
    {
        if (isGrounded || Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            if (horizontalInput > 0) spriteRenderer.sprite = rightSprite;
            else if (horizontalInput < 0) spriteRenderer.sprite = leftSprite;
            else spriteRenderer.sprite = idleSprite;
        }
        else
        {
            spriteRenderer.sprite = jumpSprite;
        }
    }

    public void Knockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public bool FacingRight => facingRight;
}