using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    private ItemData item;
    private InventoryManager manager;

    public void Init(ItemData data, InventoryManager invManager)
    {
        item = data;
        manager = invManager;
        icon.sprite = data.icon;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        manager.SelectItem(item);
    }
}