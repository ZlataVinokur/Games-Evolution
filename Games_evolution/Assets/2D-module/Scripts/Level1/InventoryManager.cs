using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private List<ItemData> allPossibleItems;

    private List<ItemData> items = new List<ItemData>();
    private List<InventorySlotUI> slots = new List<InventorySlotUI>();

    public ItemData selectedItem { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(string itemId)
    {
        ItemData data = allPossibleItems.Find(it => it.itemId == itemId);
        if (data != null && !items.Contains(data))
        {
            items.Add(data);
            RebuildUI();
        }
    }

    public void RemoveItem(string itemId)
    {
        ItemData data = items.Find(it => it.itemId == itemId);
        if (data != null)
        {
            items.Remove(data);
            if (selectedItem == data)
                DeselectItem();
            RebuildUI();
        }
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        CursorManager.Instance?.SetCursor(CursorType.Use);
    }

    public void DeselectItem()
    {
        selectedItem = null;
        CursorManager.Instance?.ResetCursor();
    }

    void RebuildUI()
    {
        foreach (var slot in slots)
            Destroy(slot.gameObject);
        slots.Clear();

        foreach (var item in items)
        {
            if (slotParent == null || slotPrefab == null)
            {
                Debug.LogWarning("InventoryManager: slotParent или slotPrefab не назначены");
                return;
            }
            GameObject go = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            if (slotUI != null)
                slotUI.Init(item, this);
            slots.Add(slotUI);
        }
    }
}