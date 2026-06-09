using UnityEngine;

public class BushSocket : Interactable
{
    public GameObject socketObject;

    void Start()
    {
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.Use;
        dialogueOnLook = new string[] { "ѕодозрительный куст. ћожет, его можно раздвинуть?" };
        articleOnFirstInteractId = "article_puzzles";   // стать€ откроетс€ при первом осмотре или использовании
    }

    protected override bool PerformAction()
    {
        if (GameManager.Instance.GetFlag("bush_opened")) return false;

        GameManager.Instance.SetFlag("bush_opened", true);
        UnifiedInfoSystem.Instance.ShowDialogue(new[]
        {
            " уст раздвинут. «а ним оказалась стара€ розетка!"
        }, "encyclopedia", "curious");

        if (socketObject != null)
            socketObject.SetActive(true);

        gameObject.SetActive(false);
        return true;
    }
}