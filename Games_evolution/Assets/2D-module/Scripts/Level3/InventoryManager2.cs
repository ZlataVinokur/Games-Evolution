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

    private void UpdateUI()
    {
        // Очищаем контент
        foreach (Transform child in inventoryContent)
            Destroy(child.gameObject);

        // Создаём слоты для каждого предмета
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, inventoryContent);
            Image img = slot.GetComponent<Image>();
            img.sprite = icons[i];
            DragAndDropItem drag = slot.GetComponent<DragAndDropItem>();
            drag.itemType = items[i];
        }
    }
}