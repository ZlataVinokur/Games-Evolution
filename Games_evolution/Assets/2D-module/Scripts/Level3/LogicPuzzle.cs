using UnityEngine;

public class LogicPuzzle : MonoBehaviour
{
    private string[] order = { "wire", "chip", "battery" };
    private int currentIndex = 0;
    private bool solved = false;
    private Slot[] slots;

    void OnEnable()
    {
        slots = FindObjectsByType<Slot>(FindObjectsSortMode.None);
        foreach (Slot slot in slots)
            slot.OnCorrectItemDropped += CheckItem;
    }

    void OnDisable()
    {
        foreach (Slot slot in slots)
            slot.OnCorrectItemDropped -= CheckItem;
    }

    void CheckItem(string type, DragAndDropItem item)
    {
        if (solved) return;
        if (type == order[currentIndex])
        {
            currentIndex++;
            if (currentIndex >= order.Length)
            {
                solved = true;
                RPGLevelManager.Instance.ActivateMeter(1);
                // Можно удалить все алтари или оставить как есть
                Destroy(gameObject);
            }
        }
        else
        {
            // Неправильный порядок: возвращаем ВСЕ предметы в инвентарь и сбрасываем алтари
            foreach (Slot slot in slots)
            {
                slot.ReturnItem();
            }
            currentIndex = 0;
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Не тот порядок! Нужно: сначала провод, потом чип, потом батарея.", 3f);
        }
    }
}