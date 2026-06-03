using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Book/Article")]
public class Article : ScriptableObject
{

    [Header("Привязка к уровню")]
    public int moduleIndex = 0; // индекс модуля
    [Header("Идентификация")]
    public string articleId;
    public string title;

    [Header("Для энциклопедии (полная статья)")]
    [TextArea(5, 20)]
    public string fullText;

    public List<string> shortAnnotation = new List<string>();

    // Свойство для обратной совместимости (если где-то используется как строка)
    public string ShortAnnotationText
    {
        get
        {
            if (shortAnnotation != null && shortAnnotation.Count > 0)
                return shortAnnotation[0];
            return "";
        }
    }

    [Header("Для обучения (последовательные сообщения)")]
    public List<string> tutorialMessages;

    [Header("Для таймерных подсказок (во время игры)")]
    public List<TimedHint> timedHints;
}

[System.Serializable]
public class TimedHint
{
    public float delayAfterStart;
    [TextArea(1, 3)]
    public string message;
    public float duration = 5f;
}