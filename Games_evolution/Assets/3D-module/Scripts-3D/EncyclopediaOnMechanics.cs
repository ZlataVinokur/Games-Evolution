using UnityEngine;
using UnityEngine.InputSystem;

public class EncyclopediaOnMechanics : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] movementFact = new string[]
    {
        "Кстати, механика движения в 3D-пространстве стала возможна благодаря технологии 'матрица трансформации'.",
        "Раньше, в 80-х, персонажи двигались только по 2D-сетке."
    };

    [TextArea(3, 5)]
    public string[] lightFact = new string[]
    {
        "А вот и он, можно добавить света! Но это еще не все, другая часть системы ждет в тени за томами.",
        "А ты знаешь, что освещение - это ключевой аспект для восприятия пространства и создания атмосферы? Теперь знаешь.",
        "Ну красота! А теперь на верх."
    };

    [TextArea(3, 5)]
    public string[] lightFact2 = new string[]
    {
        "Ну красота! А теперь на верх, по аркадным автоматам.",
    };

    [TextArea(3, 5)]
    public string[] cameraFact = new string[]
    {
        "Страшно прыгать на высоте? Ничего, закрой глаза и представь себя со стороны...",
        "Теперь у тебя куда больше контроля над прыжком, а еще можно полюбоваться на себя и анимации.",
        "Камеры от первого и третьего лица появились в игре 'Mario 64' и 'Tomb Raider' (1996).",
        "Это был прорыв в восприятии виртуального мира."
    };

    [TextArea(3, 5)]
    public string[] platformerFact = new string[]
    {
        "Ты хорошо справляешься. Платформеры учат игрока оценивать расстояния и время прыжка.",
        "В 3D добавилась третья ось — теперь нужно прыгать ещё и 'на глубину'."
    };

    [TextArea(3, 5)]
    public string[] endFact = new string[]
    {
        "Вау! Ты прошла эти испытания и теперь знаешь больше об основах геймдизайна 3D!",
        "Проходи в портал и проверь свои знания!"
    };

    private int factIndex = 0;
    private InputSystem3D inputControls;

    void Awake()
    {
        inputControls = new InputSystem3D();
    }

    void OnEnable()
    {
        inputControls.Enable();
        inputControls.Gameplay3D.NextDialogue.performed += OnRightClickOutside;
    }

    void OnDisable()
    {
        inputControls.Gameplay3D.NextDialogue.performed -= OnRightClickOutside;
        inputControls.Disable();
    }

    private void OnRightClickOutside(InputAction.CallbackContext context)
    {
        if (!UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            ShowNextFact();
        }
    }

    void ShowNextFact()
    {
        switch (factIndex)
        {
            case 0:
                ShowMovementFact();
                break;
            case 1:
                ShowLightFact();
                break;
            case 2:
                ShowLightFact2();
                break;
            case 3:
                ShowCameraFact();
                break;
            case 4:
                ShowPlatformerFact();
                break;
            case 5:
                ShowEndFact();
                break;
            default:
                ShowDefaultFact();
                break;
        }
        factIndex++;
    }

    // Публичные методы для вызова из GameEvents
    public void ShowMovementFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(movementFact, "encyclopedia", "explain");
    }

    public void ShowLightFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(lightFact, "encyclopedia", "happy");
    }

    public void ShowLightFact2()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(lightFact2, "encyclopedia", "happy");
    }

    public void ShowCameraFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(cameraFact, "encyclopedia", "explain");
    }

    public void ShowPlatformerFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(platformerFact, "encyclopedia", "explain");
    }

    public void ShowEndFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(endFact, "encyclopedia", "explain");
    }

    public void ShowDefaultFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(new[] { "Вау! Ты прошла эти испытания и теперь знаешь больше об основах геймдизайна 3D!" }, "encyclopedia", "happy");
    }
}