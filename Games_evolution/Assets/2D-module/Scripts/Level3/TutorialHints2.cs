using UnityEngine;

public class TutorialHints2 : MonoBehaviour
{
    private bool hasShownMovementHint = false;
    private bool hasShownShootHint = false;
    private bool hasShownPickupHint = false;

    void Start()
    {
        Invoke(nameof(ShowMovementHint), 1f);
    }

    void ShowMovementHint()
    {
        if (!hasShownMovementHint)
        {
            hasShownMovementHint = true;
            UnifiedInfoSystem.Instance?.ShowDialogue(
                new[] { "Управляй Пикселем с помощью WASD или стрелок. На F - стреляй в ближайшего врага.", "Наводи курсор мыши – стрельба автоматическая по ближайшему врагу при нажатии F." },
                "encyclopedia", "neutral", null);
        }
    }

    public void TryShowShootHint()
    {
        if (!hasShownShootHint)
        {
            hasShownShootHint = true;
            UnifiedInfoSystem.Instance?.ShowDialogue(
                new[] { "Отлично! Ты выстрелил. Нажми F, чтобы атаковать врагов." },
                "encyclopedia", "happy", null);
        }
    }

    public void TryShowPickupHint()
    {
        if (!hasShownPickupHint)
        {
            hasShownPickupHint = true;
            UnifiedInfoSystem.Instance?.ShowDialogue(
                new[] { "Подойди к предмету и нажми E, чтобы подобрать его. Предметы появятся в инвентаре слева." },
                "encyclopedia", "neutral", null);
        }
    }
}