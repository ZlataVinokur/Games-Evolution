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

    // Факты
    public void TriggerMovementFact()
    {
        encyclopedia?.ShowMovementFact();
    }

    public void TriggerFreeCameraFact()
    {
        encyclopedia?.ShowFreeCameraFact();
    }

    public void TriggerInteractionFact()
    {
        encyclopedia?.ShowInteractionFact();
    }

    public void TriggerLightFact()
    {
        encyclopedia?.ShowLightFact();
    }

    public void TriggerLightFact2()
    {
        encyclopedia?.ShowLightFact2();
    }

    public void TriggerTPCameraFact()
    {
        encyclopedia?.ShowTPCameraFact();
    }

    public void TriggerPlatformerFact()
    {
        encyclopedia?.ShowPlatformerFact();
    }

    public void TriggerAnimationFact()
    {
        encyclopedia?.ShowAnimationFact();
    }

    public void TriggerPhysicsFact()
    {
        encyclopedia?.ShowPhysicsFact();
    }

    public void TriggerEndFact()
    {
        encyclopedia?.ShowEndFact();
    }

    // Диалоги
    public void TriggerDialog1()
    {
        encyclopedia?.ShowDialog1();
    }

    public void TriggerDialog2()
    {
        encyclopedia?.ShowDialog2();
    }

    public void TriggerDialog3()
    {
        encyclopedia?.ShowDialog3();
    }

    public void TriggerDialogMouse()
    {
        encyclopedia?.ShowDialogMouse();
    }

    public void TriggerDialogRetort()
    {
        encyclopedia?.ShowDialogRetort();
    }

    public void TriggerDialogScales()
    {
        encyclopedia?.ShowDialogScales();
    }
}