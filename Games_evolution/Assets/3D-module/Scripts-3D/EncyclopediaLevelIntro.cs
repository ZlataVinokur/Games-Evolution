using UnityEngine;

public class EncyclopediaLevelIntro : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] introDialogue = new string[]
    {
        "Этот уровень знакомит с 3D-навигацией и платформингом.",
        "Ты можешь свободно двигаться в пространстве.",
        "Нажми ПКМ, чтобы продолжить слушать Справочника."
    };

    void Start()
    {
        // Ждём 0.5 секунды и показываем вступление
        Invoke("ShowIntro", 0.5f);
    }

    void ShowIntro()
    {
        UnifiedInfoSystem.Instance.ShowDialogue(introDialogue, "encyclopedia", "explain");
    }
}