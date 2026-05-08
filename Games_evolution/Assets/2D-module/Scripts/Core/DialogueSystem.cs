using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button nextButton;

    private Queue<string> phrases = new Queue<string>();
    private System.Action onDialogueEnd;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextPhrase);
    }

    public void ShowDialogue(string[] lines, System.Action onEnd = null)
    {
        phrases.Clear();
        foreach (var line in lines) phrases.Enqueue(line);
        onDialogueEnd = onEnd;
        dialoguePanel.SetActive(true);
        NextPhrase();
    }

    public void NextPhrase()
    {
        if (phrases.Count > 0)
        {
            dialogueText.text = phrases.Dequeue();
        }
        else
        {
            dialoguePanel.SetActive(false);
            onDialogueEnd?.Invoke();
        }
    }
}