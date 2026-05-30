using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : Interactable
{
    [SerializeField] private string nextSceneName = "Level2_Platformer";
    [SerializeField]
    private string[] portalDialogue = new string[]
    {
        "Зачарованный лес остался позади. Впереди — вертикальный тоннель.",
        "Там тебе предстоит научиться двигаться и прыгать. Настоящий платформер!",
        "Помни: каждая механика — шаг в эволюции игр."
    };

    void Start()
    {
        capabilities = InteractionCapabilities.Use;   // реагирует на левую кнопку
        speaker = Speaker.Encyclopedia;               // говорит Справочник
    }

    protected override bool PerformAction()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(portalDialogue, speaker: "encyclopedia");
        // Загружает следующий уровень после закрытия диалога
        StartCoroutine(LoadAfterDelay(2f));
        return true;
    }

    System.Collections.IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CompleteLevel();
    }

    public void CompleteLevel()
    {
        GameManager.Instance.CompleteLevel("Platformer_Module");
        GameManager.Instance.SetFlag("Platformer_Completed", true);
        GameManager.Instance?.ShowWin();
    }
}