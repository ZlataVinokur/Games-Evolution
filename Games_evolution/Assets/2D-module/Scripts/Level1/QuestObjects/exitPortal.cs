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
        DialogueSystem.Instance.ShowDialogue(portalDialogue, speaker: "encyclopedia");
        // Загружаем следующий уровень после закрытия диалога (можно с задержкой)
        StartCoroutine(LoadAfterDelay(2f));
        return true;
    }

    System.Collections.IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }
}