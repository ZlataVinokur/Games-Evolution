using UnityEngine;

public class WaterablePlant : MonoBehaviour
{
    public int energyValue = 1;
    public Sprite grownSprite;
    public AudioClip waterSound;

    private bool isWatered = false;
    private SpriteRenderer sr;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isWatered && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            // Проверяем, что в активном слоте именно лейка
            string activeItem = InventoryManager2.Instance?.GetActiveItemType();
            if (activeItem == "лейка")
            {
                Water();
            }
            else
            {
                NotificationManager.Instance?.ShowNotification("Нужно выбрать лейку в инвентаре (цифры 1-5) и нажать E", 1.5f);
            }
        }
    }

    void Water()
    {
        isWatered = true;
        if (grownSprite != null && sr != null) sr.sprite = grownSprite;
        if (waterSound != null) AudioSource.PlayClipAtPoint(waterSound, transform.position);

        var manager = FindFirstObjectByType<EnergyCollectionManager>();
        if (manager != null) manager.AddEnergy(energyValue);

        GetComponent<Collider2D>().enabled = false;
    }
}