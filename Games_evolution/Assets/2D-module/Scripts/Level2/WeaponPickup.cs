using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public float flickerSpeed = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Мигание
        if (sr != null)
        {
            float alpha = 0.7f + Mathf.Sin(Time.time * flickerSpeed) * 0.3f;
            sr.color = new Color(1, 1, 1, alpha);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Удаление произойдёт в контроллере, но можно и здесь
            // Просто уведомление
        }
    }
}