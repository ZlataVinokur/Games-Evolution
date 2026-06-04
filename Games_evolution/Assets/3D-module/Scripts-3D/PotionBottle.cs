using UnityEngine;

public class PotionBottle : MonoBehaviour
{
    [SerializeField] private AudioClip drinkClip;
    [SerializeField] private PickupableObject heavyObject; // ссылка на лестницу

    public void Drink()
    {
        // Звук питья
        if (drinkClip != null)
            AudioSource.PlayClipAtPoint(drinkClip, transform.position, 1000f);

        // Делаем лестницу лёгкой
        if (heavyObject != null)
            heavyObject.MakeLight();

        // Удаляем флакон
        Destroy(gameObject);
    }
}