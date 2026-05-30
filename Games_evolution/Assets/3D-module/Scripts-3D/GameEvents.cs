using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public EncyclopediaOnMechanics encyclopedia;

    private void Start()
    {
        Debug.Log($"GameEvents инициализирован на объекте {gameObject.name}");
        if (encyclopedia == null)
            encyclopedia = FindObjectOfType<EncyclopediaOnMechanics>();
    }

    public void OnGameWin()
    {
        GameManager.Instance.LoadQuizForCurrentModule(8);
    }

    public void TriggerMovementFact()
    {
        if (encyclopedia != null && encyclopedia.movementArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.movementArticle.shortAnnotation },
                "encyclopedia",
                "explain"
            );
        }
    }

    public void TriggerLightFact()
    {
        if (encyclopedia != null && encyclopedia.lightArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.lightArticle.shortAnnotation },
                "encyclopedia",
                "happy"
            );
        }
    }

    public void TriggerLightFact2()
    {
        if (encyclopedia != null && encyclopedia.lightArticle2 != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.lightArticle2.shortAnnotation },
                "encyclopedia",
                "happy"
            );
        }
    }

    public void TriggerCameraFact()
    {
        if (encyclopedia != null && encyclopedia.cameraArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.cameraArticle.shortAnnotation },
                "encyclopedia",
                "explain"
            );
        }
    }

    public void TriggerPlatformerFact()
    {
        if (encyclopedia != null && encyclopedia.platformerArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.platformerArticle.shortAnnotation },
                "encyclopedia",
                "explain"
            );
        }
    }

    public void TriggerEndFact()
    {
        if (encyclopedia != null && encyclopedia.endArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                new[] { encyclopedia.endArticle.shortAnnotation },
                "encyclopedia",
                "explain"
            );
        }
    }
}