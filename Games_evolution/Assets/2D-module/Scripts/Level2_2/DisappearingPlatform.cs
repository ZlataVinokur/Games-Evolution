using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float disappearTime = 0.1f; // Через сколько исчезнуть после касания
    [SerializeField] private float respawnTime = 1.5f;   // Через сколько вернуться обратно

    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Запускаем исчезновение не сразу, а с крошечной задержкой,
            // чтобы игрок успел оттолкнуться
            StartCoroutine(TogglePlatform(false, disappearTime));
        }
    }

    private IEnumerator TogglePlatform(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Если выключаем платформу
        if (!state)
        {
            platformCollider.enabled = false;
            spriteRenderer.enabled = false;
            // Запускаем обратное включение через заданное время
            StartCoroutine(TogglePlatform(true, respawnTime));
        }
        else // Если включаем платформу
        {
            platformCollider.enabled = true;
            spriteRenderer.enabled = true;
        }
    }
}