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

    // Делаем публичное свойство только для чтения
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

    public bool UseItem(ItemData item, GameObject target)
    {
        if (selectedItem == item)
        {
            Interactable targetInter = target.GetComponent<Interactable>();
            if (targetInter != null && targetInter.CanUseItem(selectedItem.itemId))
            {
                targetInter.UseItem(selectedItem.itemId);
                items.Remove(selectedItem);
                DeselectItem();
                RebuildUI();
                return true;
            }
        }
        return false;
    }

    void RebuildUI()
    {
        foreach (var slot in slots) Destroy(slot.gameObject);
        slots.Clear();
        foreach (var item in items)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.Init(item, this);
            slots.Add(slotUI);
        }
        CursorManager.Instance?.ResetCursor();
    }
}