using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager2 : MonoBehaviour
{
    public static InventoryManager2 Instance;
    public Transform inventoryContent;
    public GameObject itemSlotPrefab;

    private List<string> items = new List<string>();
    private List<Sprite> icons = new List<Sprite>();

    void Awake() => Instance = this;

    public void AddItem(string type, Sprite icon)
    {
        items.Add(type);
        icons.Add(icon);
        UpdateUI();
        if (type == "WateringCan")
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Ты подобрал лейку! Теперь поливай грибочки (подойди и нажми E).", 3f);
    }

    public bool HasItem(string type) => items.Contains(type);

    public void RemoveItem(string type)
    {
        int idx = items.IndexOf(type);
        if (idx != -1) { items.RemoveAt(idx); icons.RemoveAt(idx); UpdateUI(); }
    }

    private void UpdateUI()
    {
        foreach (Transform child in inventoryContent) Destroy(child.gameObject);
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, inventoryContent);
            slot.GetComponent<Image>().sprite = icons[i];
            slot.GetComponent<DragAndDropItem>().itemType = items[i];
        }
    }
}