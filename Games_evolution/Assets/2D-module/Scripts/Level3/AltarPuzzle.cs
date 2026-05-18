using UnityEngine;

public class AltarPuzzle : MonoBehaviour
{
    public int altarIndex; // 0,1,2
    private static int currentOrder = 0;
    private bool isActivated = false;

    public static bool CheckOrder(int index)
    {
        return index == currentOrder;
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        currentOrder++;
        GetComponent<SpriteRenderer>().color = Color.green;
        // Убираем слот или делаем неактивным
        if (currentOrder >= 3)
            RPGLevelManager.Instance.ActivateMeter(1);
    }
}