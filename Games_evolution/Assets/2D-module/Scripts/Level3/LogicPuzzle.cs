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
                NotificationManager.Instance?.ShowNotification("Правильный порядок! Синий измеритель активирован.", 2f);
                Destroy(gameObject);
            }
        }
        else
        {
            foreach (Slot slot in slots) slot.ReturnItem();
            currentIndex = 0;
            NotificationManager.Instance?.ShowNotification("Не тот порядок! Нужно: провод → чип → батарея", 2f);
        }
    }
}