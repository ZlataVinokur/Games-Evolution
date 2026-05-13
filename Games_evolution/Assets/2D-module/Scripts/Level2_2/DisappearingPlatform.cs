using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float disappearTime = 0.1f;
    [SerializeField] private float respawnTime = 1.5f;
    [SerializeField] private Vector2 checkSize = new Vector2(2f, 0.5f);
    [SerializeField] private LayerMask playerLayer;

    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;
    private bool isVisible = true;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!isVisible) return;

        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position, checkSize, 0f, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y <= 0f) // игрок падает
            {
                // Дополнительная проверка: центр игрока выше центра платформы (значит, он сверху)
                if (hit.transform.position.y > transform.position.y)
                {
                    StartCoroutine(Disappear());
                }
            }
        }
    }

    IEnumerator Disappear()
    {
        isVisible = false;
        yield return new WaitForSeconds(disappearTime);
        platformCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnTime);
        platformCollider.enabled = true;
        spriteRenderer.enabled = true;
        isVisible = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, checkSize);
    }
}