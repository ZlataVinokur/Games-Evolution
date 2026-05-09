using UnityEngine;

public class CharacterExpression : MonoBehaviour
{
    public static CharacterExpression Instance { get; private set; }

    [System.Serializable]
    public struct EmotionSprite
    {
        public string emotionName;
        public Sprite sprite;
    }

    public EmotionSprite[] emotions; // neutral, surprised, happy, sad, curious, etc.

    private void Awake() => Instance = this;

    public Sprite GetSprite(string emotion)
    {
        foreach (var e in emotions)
            if (e.emotionName == emotion) return e.sprite;
        return emotions.Length > 0 ? emotions[0].sprite : null;
    }
}