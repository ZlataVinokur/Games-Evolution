using UnityEngine;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Самый простой способ показать информацию
    public void TellFact(string[] factLines, string emotion = "neutral")
    {
        DialogueSystem.Instance.ShowDialogue(factLines, "encyclopedia", emotion);
    }
}