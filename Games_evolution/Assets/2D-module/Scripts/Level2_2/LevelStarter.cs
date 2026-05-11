using UnityEngine;

public class Level2Starter : MonoBehaviour
{
    void Start()
    {
        // Вступительный диалог от Справочника
        DialogueSystem.Instance.ShowDialogue(
            new[] { "Добро пожаловать в эпоху платформеров! ",
                     "Здесь ты научишься двигаться и прыгать, избегать врагов и использовать оружие." },
            speaker: "encyclopedia",
            emotion: "happy"
        );
    }
}