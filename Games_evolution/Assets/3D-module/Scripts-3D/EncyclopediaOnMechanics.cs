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
        "А вот и он, можно добавить света! Но это еще не все, другая часть системы ждет за томами.",
        "А ты знаешь, что освещение - это ключевой аспект для восприятия пространства и создания атмосферы? Теперь знаешь.",
        "Ну красота! А теперь на верх."
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
        "Ты хорошо справляешься. Платформеры учат игрока оценивать расстояния и время прыжка.",
        "Вау! Ты прошла эти испытания и теперь знаешь больше об основах геймдизайна 3D!"
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
        // Используем IsShowingAnything() вместо несуществующего IsDialogueActive()
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
                UnifiedInfoSystem.Instance.ShowDialogue(movementFact, "encyclopedia", "explain");
                break;
            case 1:
                UnifiedInfoSystem.Instance.ShowDialogue(lightFact, "encyclopedia", "happy");
                break;
            case 2:
                UnifiedInfoSystem.Instance.ShowDialogue(cameraFact, "encyclopedia", "explain");
                break;
            case 3:
                UnifiedInfoSystem.Instance.ShowDialogue(platformerFact, "encyclopedia", "explain");
                break;
            case 4:
                UnifiedInfoSystem.Instance.ShowDialogue(endFact, "encyclopedia", "explain");
                break;
            default:
                UnifiedInfoSystem.Instance.ShowDialogue(new[] { "Вау! Ты прошла эти испытания и теперь знаешь больше об основах геймдизайна 3D!" }, "encyclopedia", "happy");
                break;
        }
        factIndex++;
    }
}