using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    public EncyclopediaOnMechanics encyclopedia; // Ссылка на скрипт энциклопедии

    private void Start()
    {
        Debug.Log($"GameEvents инициализирован на объекте {gameObject.name}");

        // Если ссылка не назначена в инспекторе, попробуем найти автоматически
        if (encyclopedia == null)
        {
            encyclopedia = FindObjectOfType<EncyclopediaOnMechanics>();
        }
    }

    public void OnGameWin()
    {
        GameManager.Instance.LoadQuizForCurrentModule(8);
    }

    // Методы для триггеров
    public void TriggerMovementFact()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowMovementFact();
        }
    }

    public void TriggerLightFact()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowLightFact();
        }
    }

    public void TriggerLightFact2()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowLightFact2();
        }
    }

    public void TriggerCameraFact()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowCameraFact();
        }
    }

    public void TriggerPlatformerFact()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowPlatformerFact();
        }
    }

    public void TriggerEndFact()
    {
        if (encyclopedia != null && !UnifiedInfoSystem.Instance.IsShowingAnything())
        {
            encyclopedia.ShowEndFact();
        }
    }
}