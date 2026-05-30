using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager2 : MonoBehaviour
{
    public static InventoryManager2 Instance { get; private set; }

    [Header("UI")]
    public Transform inventoryContent; // ссылка на Content из ScrollView
    public GameObject itemSlotPrefab;  // префаб с Image + DragAndDropItem

    private List<string> items = new List<string>();
    private List<Sprite> icons = new List<Sprite>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(string type, Sprite icon)
    {
        items.Add(type);
        icons.Add(icon);
        UpdateUI();

        if (items.Count == 1) // первый предмет
        {
            UnifiedInfoSystem.Instance?.ShowDialogue(
                new[] { "Предмет в инвентаре! Нажми на него и перетащи на алтарь нужного цвета." },
                "encyclopedia", "neutral", null);
        }
    }

    public bool HasItem(string type)
    {
        return items.Contains(type);
    }

    public void RemoveItem(string type)
    {
        int index = items.IndexOf(type);
        if (index != -1)
        {
            items.RemoveAt(index);
            icons.RemoveAt(index);
            UpdateUI();
        }
    }

    public void AddItemBack(string type, Sprite icon)
    {
        items.Add(type);
        icons.Add(icon);
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (Transform child in inventoryContent)
            Destroy(child.gameObject);
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, inventoryContent);
            slot.GetComponent<Image>().sprite = icons[i];
            slot.GetComponent<DragAndDropItem>().itemType = items[i];
        }
    }
}