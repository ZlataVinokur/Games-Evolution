using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemType;
    public Sprite itemIcon;
    private bool playerInRange = false;

    private static bool logicArticleShown = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!logicArticleShown && (itemType == "кассета" || itemType == "хард-драйв" || itemType == "диск"))
            {
                logicArticleShown = true;
                UnifiedInfoSystem.Instance?.UnlockArticle("rpg_logic");
            }

            InventoryManager2.Instance.AddItem(itemType, itemIcon);
            Destroy(gameObject);

            if (itemType == "пистолет")
            {
                var player = FindObjectOfType<IsometricPlayerController>();
                if (player != null) player.hasGun = true;
                NotificationManager.Instance?.ShowNotification("Пистолет экипирован! ЛКМ – стрельба.", 2f);
            }
            else
            {
                NotificationManager.Instance?.ShowNotification($"Предмет '{itemType}' добавлен в инвентарь", 1.5f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            NotificationManager.Instance?.ShowNotification($"Нажмите E, чтобы подобрать {itemType}", 1.5f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}