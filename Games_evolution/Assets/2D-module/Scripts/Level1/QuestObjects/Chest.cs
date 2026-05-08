using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject keyPrefab;
    private SpriteRenderer sr;
    private bool isOpen = false;

    void Start() => sr = GetComponent<SpriteRenderer>();

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            sr.sprite = openSprite;
            // Спавним ключ как PickUp объект или сразу в инвентарь
            InventoryManager.Instance.AddItem("digital_key");
            DialogueSystem.Instance.ShowDialogue(new[] {
                "В сундуке лежит цифровой ключ!"
            });
        }
    }
}