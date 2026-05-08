using UnityEngine;

[CreateAssetMenu(fileName = "NewArticle", menuName = "Reference Book/Article")]
public class Article : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(5, 20)]
    public string content;
    public string triggerId; // Идентификатор игрового события для автооткрытия
}