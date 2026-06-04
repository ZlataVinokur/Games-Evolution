using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float bounceForce = 20f;
    [SerializeField] private Vector2 checkSize = new Vector2(2f, 1f); // размер области (подгони под спрайт)
    [SerializeField] private LayerMask playerLayer;                    // слой игрока

    private Vector2 checkPosition => (Vector2)transform.position;
    private Rigidbody2D playerRb;
    private bool playerWasInArea;

    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapBox(checkPosition, checkSize, 0f, playerLayer);
        bool playerInArea = hit != null && hit.CompareTag("Player");

        if (playerInArea && !playerWasInArea) // игрок только что зашёл
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }

        playerWasInArea = playerInArea;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, checkSize);
    }
}