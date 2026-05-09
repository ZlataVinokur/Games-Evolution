using UnityEngine;

public class BushSocket : Interactable
{
    [Tooltip("Объект розетки, который появится после открытия куста")]
    public GameObject socketObject;

    protected override bool PerformAction()
    {
        if (!GameManager.Instance.GetFlag("bush_opened"))
        {
            // Устанавливаем флаг
            GameManager.Instance.SetFlag("bush_opened", true);

            // Показываем диалог
            DialogueSystem.Instance.ShowDialogue(new[]
            {
                "Куст раздвинут. За ним оказалась старая розетка!"
            });

            // Включаем розетку
            if (socketObject != null)
                socketObject.SetActive(true);

            // Полностью отключаем куст (коллайдер, спрайт, всё)
            gameObject.SetActive(false);

            return true;
        }

        // Если куст уже открыт (страховка)
        return false;
    }
}