using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;   // "wire", "coin", "mushroom", "digital_key"
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
}