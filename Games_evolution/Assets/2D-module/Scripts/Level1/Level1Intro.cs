using UnityEngine;

public class Level1Intro : MonoBehaviour
{
    public string[] introLines = new string[] {
        "Добро пожаловать в Зачарованный лес. Это мир point-and-click приключений.",
        "Здесь ты будешь исследовать окружение, подбирать предметы и решать головоломки.",
        "Самые известные квесты — Monkey Island и Myst — строились именно на этих механиках.",
        "Давай начнём: осмотрись, и, может, найдёшь что-то полезное..."
    };

    void Start()
    {
        DialogueSystem.Instance.ShowDialogue(introLines, "neutral");
        // Сразу открываем вводную статью о жанре
        EncyclopediaManager.Instance?.UnlockArticle("article_pointandclick");
    }
}