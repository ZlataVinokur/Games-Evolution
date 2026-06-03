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
                encyclopedia.movementArticle.shortAnnotation.ToArray(),
                "encyclopedia",
                "explain",
                force: true
            );
        }
    }

    public void TriggerLightFact()
    {
        if (encyclopedia != null && encyclopedia.lightArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                encyclopedia.lightArticle.shortAnnotation.ToArray(),
                "encyclopedia",
                "happy",
                force: true
            );
        }
    }

    public void TriggerLightFact2()
    {
        if (encyclopedia != null && encyclopedia.lightArticle2 != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                encyclopedia.lightArticle2.shortAnnotation.ToArray(),
                "encyclopedia",
                "happy",
                force: true
            );
        }
    }

    public void TriggerCameraFact()
    {
        if (encyclopedia != null && encyclopedia.cameraArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                encyclopedia.cameraArticle.shortAnnotation.ToArray(),
                "encyclopedia",
                "explain",
                force: true
            );
        }
    }

    public void TriggerPlatformerFact()
    {
        if (encyclopedia != null && encyclopedia.platformerArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                encyclopedia.platformerArticle.shortAnnotation.ToArray(),
                "encyclopedia",
                "explain",
                force: true
            );
        }
    }

    public void TriggerEndFact()
    {
        if (encyclopedia != null && encyclopedia.endArticle != null)
        {
            UnifiedInfoSystem.Instance.ShowDialogue(
                encyclopedia.endArticle.shortAnnotation.ToArray(),
                "encyclopedia",
                "explain",
                force: true
            );
        }
    }
}