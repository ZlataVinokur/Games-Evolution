using UnityEngine;

public class WaterablePlant : MonoBehaviour
{
    public int energyValue = 1;
    private bool isWatered = false;
    public Sprite grownSprite;
    private SpriteRenderer sr;
    public AudioClip waterSound;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isWatered && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (InventoryManager2.Instance != null && InventoryManager2.Instance.HasItem("WateringCan"))
            {
                Water();
            }
            else
            {
                UnifiedInfoSystem.Instance?.ShowTimedMessage("Нужна лейка! Найди её и подбери.", 1f);
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