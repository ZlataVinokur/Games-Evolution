using UnityEngine;

public class LogicPuzzle : MonoBehaviour
{
    private string[] order = { "wire", "chip", "battery" };
    private int currentIndex = 0;
    private bool solved = false;

    void OnEnable()
    {
        Slot[] slots = FindObjectsOfType<Slot>();
        foreach (Slot slot in slots)
            slot.OnCorrectItemDropped += CheckItem;
    }

    void OnDisable()
    {
        Slot[] slots = FindObjectsOfType<Slot>();
        foreach (Slot slot in slots)
            slot.OnCorrectItemDropped -= CheckItem;
    }

    void CheckItem(string type)
    {
        if (solved) return;
        if (type == order[currentIndex])
        {
            currentIndex++;
            if (currentIndex >= order.Length)
            {
                solved = true;
                RPGLevelManager.Instance.ActivateMeter(1);
                Destroy(gameObject);
            }
        }
        else
        {
            currentIndex = 0;
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Не тот порядок! Начни сначала.", 1.5f);
        }
    }
}