using UnityEngine;

public class BushSocket : Interactable
{
    [Tooltip("Объект розетки, который появится после открытия куста")]
    public GameObject socketObject;

    protected override bool PerformAction()
    {
        if (!GameManager.Instance.GetFlag("bush_opened"))
        {
            GameManager.Instance.SetFlag("bush_opened", true);

            DialogueSystem.Instance.ShowDialogue(new[]
            {
                "Куст раздвинут. За ним оказалась старая розетка!"
            }, speaker: "encyclopedia", emotion: "surprised");

            if (socketObject != null)
                socketObject.SetActive(true);

            // Открываем статью о логических цепочках
            EncyclopediaManager.Instance?.UnlockArticle("article_puzzles");

            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}