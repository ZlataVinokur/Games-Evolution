using UnityEngine;
using UnityEngine.InputSystem;

public class EncyclopediaOnMechanics : MonoBehaviour
{
    [Header("Статьи для фактов")]
    public Article movementArticle;
    public Article freeCameraArticle;
    public Article interactionArticle;
    public Article lightArticle;
    public Article lightArticle2;
    public Article tpCameraArticle;
    public Article platformerArticle;
    public Article animationArticle;
    public Article physicsArticle;
    public Article endArticle;

    [Header("Статьи для диалогов")]
    public Article dialog1;
    public Article dialog2;
    public Article dialog3;
    public Article dialogMouse;
    public Article dialogRetort;
    public Article dialogScales;

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
            case 1: ShowFreeCameraFact(); break;
            case 2: ShowInteractionFact(); break;
            case 3: ShowLightFact(); break;
            case 4: ShowLightFact2(); break;
            case 5: ShowTPCameraFact(); break;
            case 6: ShowPlatformerFact(); break;
            case 7: ShowAnimationFact(); break;
            case 8: ShowPhysicsFact(); break;
            case 9: ShowEndFact(); break;
        }
        factIndex++;
    }

    public void ShowMovementFact()
    {
        if (movementArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                movementArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowFreeCameraFact()
    {
        if (freeCameraArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                freeCameraArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowInteractionFact()
    {
        if (interactionArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                interactionArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
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

    public void ShowTPCameraFact()
    {
        if (tpCameraArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                tpCameraArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowPlatformerFact()
    {
        if (platformerArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                platformerArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowAnimationFact()
    {
        if (animationArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                animationArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowPhysicsFact()
    {
        if (physicsArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                physicsArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowEndFact()
    {
        if (endArticle != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                endArticle.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    // Диалоги
    public void ShowDialog1()
    {
        if (dialog1 != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialog1.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDialog2()
    {
        if (dialog2 != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialog2.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDialog3()
    {
        if (dialog3 != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialog3.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDialogMouse()
    {
        if (dialogMouse != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialogMouse.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDialogRetort()
    {
        if (dialogRetort != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialogRetort.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }

    public void ShowDialogScales()
    {
        if (dialogScales != null)
            UnifiedInfoSystem.Instance.ShowDialogue(
                dialogScales.shortAnnotation.ToArray(), "encyclopedia", "explain", force: true);
    }
}