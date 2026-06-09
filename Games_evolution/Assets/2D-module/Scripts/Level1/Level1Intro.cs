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
        UnifiedInfoSystem.Instance.ShowDialogue(introLines, "encyclopedia", "neutral");
        // Открываем вводную статью о жанре (принудительно, без проверки на первое взаимодействие)
        UnifiedInfoSystem.Instance?.UnlockArticle("article_pointandclick");
    }
}