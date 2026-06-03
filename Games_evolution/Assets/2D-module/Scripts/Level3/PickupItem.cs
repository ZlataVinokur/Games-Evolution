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
            var toast = GetComponent<WorldToast>();
            if (toast != null) toast.Show($"Предмет теперь в инвентаре", 1.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // подсказка над предметом
            var toast = GetComponent<WorldToast>();
            if (toast != null) toast.Show($"Нажмите E, чтобы подобрать {itemType}", 1.5f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}