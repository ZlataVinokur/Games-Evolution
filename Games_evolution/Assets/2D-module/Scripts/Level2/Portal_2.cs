using UnityEngine;

public class Portal_2 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PixelPlatformerController player = other.GetComponent<PixelPlatformerController>();
            if (player != null)
            {
                // Дополнительная проверка, что портал активен – переменная portalActive в контроллере
                // Сам CompleteLevel вызовется внутри OnTriggerEnter2D в контроллере
            }
        }
    }
}