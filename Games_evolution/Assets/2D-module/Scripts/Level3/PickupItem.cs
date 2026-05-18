using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemType;
    public Sprite itemIcon;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InventoryManager2.Instance.AddItem(itemType, itemIcon);
            Destroy(gameObject);
            UnifiedInfoSystem.Instance?.ShowTimedMessage($"Вы подобрали: {itemType} (нажмите E)", 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UnifiedInfoSystem.Instance?.ShowTimedMessage($"Нажмите E, чтобы подобрать {itemType}", 1f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}