using UnityEngine;

[CreateAssetMenu(menuName = "Reference Book/Article")]
public class Article : ScriptableObject
{
    public string articleId;
    public string title;
    [TextArea(5, 20)]
    public string fullText;      // полный текст для справочника
    public string shortAnnotation; // 1-2 предложения для всплывающего уведомления
}