using System.Collections;
using UnityEngine;

public class LogicPuzzle : MonoBehaviour
{
    private int itemsPlaced = 0;
    private string[] order = { "wire", "chip", "battery" };
    private int currentIndex = 0;

    void Start()
    {
        // Найти все слоты и подписаться на события
        Slot[] slots = FindObjectsOfType<Slot>();
        foreach (Slot slot in slots)
        {
            slot.OnCorrectItemDropped += () => CheckItem(slot.requiredItemType);
        }
    }

    void CheckItem(string type)
    {
        if (type == order[currentIndex])
        {
            currentIndex++;
            if (currentIndex >= order.Length)
            {
                // Головоломка решена
                RPGLevelManager.Instance.ActivateMeter(1);
                Destroy(gameObject);
            }
        }
        else
        {
            // Неправильный порядок – сброс
            currentIndex = 0;
            UnifiedInfoSystem.Instance.ShowTimedMessage("Не тот порядок! Начни сначала.", 1.5f);
        }
    }
}