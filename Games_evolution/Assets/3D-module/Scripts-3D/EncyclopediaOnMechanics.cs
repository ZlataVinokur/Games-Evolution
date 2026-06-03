using UnityEngine;
using UnityEngine.InputSystem;

public class EncyclopediaOnMechanics : MonoBehaviour
{
    [Header("Статьи для фактов")]
    public Article movementArticle;
    public Article lightArticle;
    public Article lightArticle2;
    public Article cameraArticle;
    public Article platformerArticle;
    public Article endArticle;

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
            case 0: ShowMovementFact(); break;
            case 1: ShowLightFact(); break;
            case 2: ShowLightFact2(); break;
            case 3: ShowCameraFact(); break;
            case 4: ShowPlatformerFact(); break;
            case 5: ShowEndFact(); break;
            default: ShowDefaultFact(); break;
        }
        factIndex++;
    }

    public void ShowMovementFact()
    {
        if (movementArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                movementArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowLightFact()
    {
        if (lightArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                lightArticle.shortAnnotation.ToArray(), "encyclopedia", "happy", force: true);
    }

    public void ShowLightFact2()
    {
        if (lightArticle2 != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                lightArticle2.shortAnnotation.ToArray(), "encyclopedia", "happy", force: true);
    }

    public void ShowCameraFact()
    {
        if (cameraArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                cameraArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowPlatformerFact()
    {
        if (platformerArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                platformerArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowEndFact()
    {
        if (endArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                endArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDefaultFact()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(
            new[] { "Вау! Ты прошла эти испытания и теперь знаешь больше об основах геймдизайна 3D!" },
            "encyclopedia", "happy", force: true);
    }
}