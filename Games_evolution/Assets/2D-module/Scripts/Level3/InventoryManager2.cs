using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager2 : MonoBehaviour
{
    public static InventoryManager2 Instance;

    [Header("UI")]
    public Transform inventoryContent;
    public GameObject itemSlotPrefab;

    [Header("Settings")]
    public int maxSlots = 5;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private int activeSlotIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateSlots();
        SelectSlot(0);
    }

    void Update()
    {
        for (int i = 0; i < maxSlots && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    void CreateSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, inventoryContent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot == null) slot = slotObj.AddComponent<InventorySlot>();
            slot.Initialize(i);
            slots.Add(slot);
        }
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= maxSlots) return;
        activeSlotIndex = index;
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetHighlight(i == activeSlotIndex);
        NotificationManager.Instance?.ShowNotification($"Выбран слот {index + 1}: {(slots[index].itemType ?? "пусто")}", 0.8f);
    }

    public void AddItem(string type, Sprite icon)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemType == null)
            {
                slots[i].SetItem(type, icon);
                NotificationManager.Instance?.ShowNotification($"Предмет '{type}' добавлен в слот {i + 1}", 1.5f);
                return;
            }
        }
        NotificationManager.Instance?.ShowNotification("Инвентарь полон!", 1f);
    }

    public bool HasItem(string type)
    {
        foreach (var slot in slots)
            if (slot.itemType == type) return true;
        return false;
    }

    public void RemoveItem(string type)
    {
        foreach (var slot in slots)
        {
            if (slot.itemType == type)
            {
                slot.ClearItem();
                break;
            }
        }
    }

    public string GetActiveItemType()
    {
        if (activeSlotIndex < 0 || activeSlotIndex >= slots.Count) return null;
        return slots[activeSlotIndex].itemType;
    }
    public void UseActiveItem()
    {
        if (activeSlotIndex < 0 || activeSlotIndex >= slots.Count) return;
        var slot = slots[activeSlotIndex];
        if (slot.itemType == null) return;

        // Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Проверяем коллайдер в этой точке
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
        if (hit != null)
        {
            EnergyChest chest = hit.GetComponent<EnergyChest>();
            if (chest != null && chest.TryUseItem(slot.itemType))
            {
                slot.ClearItem();
                return;
            }
        }
        NotificationManager.Instance?.ShowNotification($"Не на что применить {slot.itemType}", 1f);
    }
}