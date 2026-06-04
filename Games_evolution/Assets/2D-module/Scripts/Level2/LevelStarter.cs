using UnityEngine;

public class Level2Starter : MonoBehaviour
{
    void Start()
    {
        // Вступительное сообщение
        UnifiedInfoSystem.Instance.ShowDialogue(
            new[] {
                "Приветствую, Пиксель! Это эпоха платформеров.",
                "Твоя задача — подниматься вверх, избегая стен и врагов.",
                "Великий Справочник будет пояснять новые механики."
            },
            speaker: "encyclopedia",
            emotion: "happy"
        );

        // Открываем вводную статью
        UnifiedInfoSystem.Instance.UnlockArticle("platformer_intro");
    }
}