using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Добавить эту строку
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image portraitImage;

    private Queue<string> phrases = new Queue<string>();

    // Новый Input Actions
    private InputSystem3D inputControls;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextPhrase);

        // Инициализация Input System
        inputControls = new InputSystem3D();
    }

    void OnEnable()
    {
        // Включаем управление
        inputControls.Enable();
        // Подписываемся на событие нажатия ПКМ
        inputControls.Gameplay3D.NextDialogue.performed += OnRightClick;
    }

    void OnDisable()
    {
        // Отписываемся
        inputControls.Gameplay3D.NextDialogue.performed -= OnRightClick;
        inputControls.Disable();
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        // Если диалог открыт - следующая фраза
        if (dialoguePanel.activeSelf)
        {
            NextPhrase();
        }
    }

    public void ShowDialogue(string[] lines, string speaker = "player", string emotion = "neutral")
    {
        dialoguePanel.SetActive(true);

        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(true);
            // Твой код для портретов
        }

        phrases.Clear();
        foreach (var line in lines) phrases.Enqueue(line);
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
            if (portraitImage != null)
                portraitImage.gameObject.SetActive(false);
        }
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}