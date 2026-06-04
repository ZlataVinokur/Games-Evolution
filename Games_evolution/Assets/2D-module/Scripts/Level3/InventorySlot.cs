using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI numberText;
    public string itemType { get; private set; }
    public Sprite itemIcon { get; private set; }

    private int slotIndex;
    private Color defaultColor = Color.white;
    private Color highlightColor = Color.yellow;

    public void Initialize(int index)
    {
        slotIndex = index;
        if (numberText != null) numberText.text = (index + 1).ToString();
        ClearItem();
    }

    public void SetItem(string type, Sprite icon)
    {
        itemType = type;
        itemIcon = icon;
        if (iconImage != null) iconImage.sprite = icon;
        iconImage.enabled = true;
    }

    public void ClearItem()
    {
        itemType = null;
        itemIcon = null;
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (iconImage != null)
            iconImage.color = highlight ? highlightColor : defaultColor;
    }
}