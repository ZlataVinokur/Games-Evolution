using UnityEngine;

public class EncyclopediaExpression : MonoBehaviour
{
    public static EncyclopediaExpression Instance { get; private set; }

    [System.Serializable]
    public struct EmotionSprite
    {
        public string emotionName;
        public Sprite sprite;
    }

    public EmotionSprite[] emotions; // например, neutral, explain, happy, surprised

    private void Awake()
    {
        Instance = this;
        // Не уничтожаем при переходе между сценами
        DontDestroyOnLoad(gameObject);
    }

    public Sprite GetSprite(string emotionName)
    {
        foreach (var e in emotions)
            if (e.emotionName == emotionName)
                return e.sprite;
        // Если нет подходящей эмоции, возвращаем первую или null
        return emotions.Length > 0 ? emotions[0].sprite : null;
    }
}