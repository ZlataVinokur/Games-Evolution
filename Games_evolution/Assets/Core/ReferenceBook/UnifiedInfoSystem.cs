using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnifiedInfoSystem : MonoBehaviour
{
    public static UnifiedInfoSystem Instance { get; private set; }

    [Header("UI Elements (общая панель)")]
    public GameObject infoPanel;
    public TextMeshProUGUI messageText;
    public Button continueButton;
    public Button startGameButton;
    public Image portraitImage;

    [Header("Диалоговый режим")]
    [SerializeField] private EncyclopediaExpression encyclopediaEmotions;
    [SerializeField] private string defaultEncyclopediaEmotion = "neutral";
    [SerializeField] private CharacterExpression characterExpression;

    [Header("Dynamic UI Binding")]
    [SerializeField] private string canvasName = "InfoCanvas";
    [SerializeField] private string panelName = "InfoPanel";
    [SerializeField] private string messageTextName = "MessageText";
    [SerializeField] private string continueButtonName = "ContinueButton";
    [SerializeField] private string startGameButtonName = "StartGameButton";
    [SerializeField] private string portraitImageName = "PortraitImage";

    [Header("База статей")]
    [SerializeField] private List<Article> allArticles;
    public List<Article> AllArticles => allArticles;

    [Header("Настройки по умолчанию")]
    public string defaultMessage = "Приступай к игре! Следи за подсказками.";
    public float defaultTimedMessageDuration = 3f;

    private bool isShowing = false;
    private Queue<string> messageQueue = new Queue<string>();
    private Action onCompleteCallback;
    private Coroutine timedMessageCoroutine;

    private Queue<string> dialoguePhrases = new Queue<string>();
    private bool isDialogueMode = false;
    private Action onDialogueComplete;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (infoPanel == null)
            RebindUIElements();

        if (continueButton != null) continueButton.onClick.AddListener(OnContinueButton);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGame);

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShowing) return;

        // Пропуск на ПКМ, Enter, Пробел
        bool skipPressed = Input.GetMouseButtonDown(1) ||
                           Input.GetKeyDown(KeyCode.Return) ||
                           Input.GetKeyDown(KeyCode.KeypadEnter) ||
                           Input.GetKeyDown(KeyCode.Space);

        if (skipPressed)
        {
            if (continueButton != null && continueButton.gameObject.activeInHierarchy && continueButton.interactable)
            {
                OnContinueButton();
            }
            else if (startGameButton != null && startGameButton.gameObject.activeInHierarchy && startGameButton.interactable)
            {
                OnStartGame();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (infoPanel == null || messageText == null || continueButton == null)
        {
            RebindUIElements();
        }
    }

    private void RebindUIElements()
    {
        Canvas canvas = null;
        if (!string.IsNullOrEmpty(canvasName))
        {
            GameObject canvasObj = GameObject.Find(canvasName);
            if (canvasObj != null)
                canvas = canvasObj.GetComponent<Canvas>();
        }
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Не найден Canvas для привязки UI UnifiedInfoSystem");
            return;
        }

        Transform panelTransform = canvas.transform.Find(panelName);
        if (panelTransform != null)
            infoPanel = panelTransform.gameObject;
        else
        {
            Debug.LogError($"Не найден объект {panelName} в Canvas {canvas.name}");
            return;
        }

        messageText = infoPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (messageText == null)
        {
            Debug.LogWarning("TextMeshProUGUI не найден");
        }

        Transform continueBtn = infoPanel.transform.Find(continueButtonName);
        if (continueBtn != null)
            continueButton = continueBtn.GetComponent<Button>();
        else
            continueButton = infoPanel.GetComponentInChildren<Button>();

        Transform startBtn = infoPanel.transform.Find(startGameButtonName);
        if (startBtn != null)
            startGameButton = startBtn.GetComponent<Button>();

        Transform portraitTransform = infoPanel.transform.Find(portraitImageName);
        if (portraitTransform != null)
            portraitImage = portraitTransform.GetComponent<Image>();

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueButton);
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGame);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGame);
    }

    // ==================== ПУБЛИЧНЫЕ МЕТОДЫ ====================

    #region Статьи
    public void UnlockArticle(string articleId)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance отсутствует! Статья не может быть разблокирована.");
            return;
        }

        var article = allArticles.Find(a => a.articleId == articleId);
        if (article == null)
        {
            Debug.LogWarning($"Статья с ID {articleId} не найдена в базе.");
            return;
        }

        GameManager.Instance.UnlockArticle(articleId);
        ShowDialogue(new[] { article.shortAnnotation }, "encyclopedia", "happy");
    }

    public string GetArticleText(string articleId)
    {
        if (GameManager.Instance == null)
            return "Ошибка: менеджер игры не найден.";

        if (!GameManager.Instance.unlockedArticles.ContainsKey(articleId))
            return "Статья ещё не открыта.";

        var article = allArticles.Find(a => a.articleId == articleId);
        return article != null ? article.fullText : "Статья не найдена.";
    }

    public bool IsArticleUnlocked(string articleId)
    {
        return GameManager.Instance != null && GameManager.Instance.unlockedArticles.ContainsKey(articleId);
    }
    #endregion

    #region Диалоги
    public void ShowDialogue(string[] lines, string speaker = "encyclopedia", string emotion = "neutral", Action onComplete = null)
    {
        if (isShowing) return;
        if (!EnsureUIReady()) return;

        isShowing = true;
        isDialogueMode = true;
        onDialogueComplete = onComplete;

        infoPanel.SetActive(true);
        dialoguePhrases.Clear();
        foreach (var line in lines) dialoguePhrases.Enqueue(line);

        continueButton.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(false);

        // Всегда показываем портрет
        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(true);
            if (speaker == "encyclopedia" && encyclopediaEmotions != null)
                portraitImage.sprite = encyclopediaEmotions.GetSprite(emotion);
            else if (characterExpression != null)
                portraitImage.sprite = characterExpression.GetSprite(emotion);
        }

        ShowNextDialoguePhrase();
    }

    private void ShowNextDialoguePhrase()
    {
        if (dialoguePhrases.Count > 0)
        {
            messageText.text = dialoguePhrases.Dequeue();
        }
        else
        {
            CloseDialogue();
        }
    }

    private void CloseDialogue()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        isShowing = false;
        isDialogueMode = false;
        if (messageText != null) messageText.text = "";
        // Портрет не отключаем – при следующем диалоге он снова включится
        onDialogueComplete?.Invoke();
        onDialogueComplete = null;
    }
    #endregion

    #region Обучение (с кнопкой "Начать игру")
    public void ShowSequentialMessages(List<string> messages, Action onComplete)
    {
        if (isShowing) return;
        if (!EnsureUIReady()) return;

        isShowing = true;
        isDialogueMode = false;

        infoPanel.SetActive(true);
        messageQueue.Clear();
        foreach (var msg in messages) messageQueue.Enqueue(msg);
        onCompleteCallback = onComplete;

        continueButton.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(false);

        // Всегда показываем портрет
        if (portraitImage != null) portraitImage.gameObject.SetActive(true);

        ShowNextTrainingMessage();
    }

    private void ShowNextTrainingMessage()
    {
        if (messageQueue.Count > 0)
        {
            messageText.text = messageQueue.Dequeue();
        }
        else
        {
            messageText.text = "ГОТОВ НАЧАТЬ ИГРУ?";
            startGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
            // Портрет оставляем – не отключаем (убрано отключение)
        }
    }
    #endregion

    #region Подсказка с паузой
    public void ShowHint(string hintText, Action onUnpause = null)
    {
        if (isShowing) return;
        if (!EnsureUIReady()) return;

        isShowing = true;
        isDialogueMode = false;

        infoPanel.SetActive(true);
        messageText.text = hintText;
        continueButton.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(false);

        // Показываем портрет (можно оставить последний использованный)
        if (portraitImage != null) portraitImage.gameObject.SetActive(true);

        onCompleteCallback = () =>
        {
            if (infoPanel != null) infoPanel.SetActive(false);
            isShowing = false;
            onUnpause?.Invoke();
        };
    }
    #endregion

    #region Таймерное сообщение
    public void ShowTimedMessage(string message, float duration = -1)
    {
        if (!EnsureUIReady()) return;

        if (timedMessageCoroutine != null) StopCoroutine(timedMessageCoroutine);
        if (duration <= 0) duration = defaultTimedMessageDuration;
        timedMessageCoroutine = StartCoroutine(TimedMessageRoutine(message, duration));
    }

    private IEnumerator TimedMessageRoutine(string message, float duration)
    {
        if (infoPanel != null && !infoPanel.activeSelf) infoPanel.SetActive(true);
        if (messageText != null) messageText.text = message;
        // Для таймерных сообщений портрет не показываем (это не диалог)
        // Но если нужен – раскомментируйте:
        // if (portraitImage != null) portraitImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (messageText != null) messageText.text = "";
    }
    #endregion

    #region Утилиты
    public void TellFact(string[] factLines, string emotion = "neutral")
    {
        ShowDialogue(factLines, "encyclopedia", emotion);
    }

    public bool IsShowingAnything() => isShowing || (infoPanel != null && infoPanel.activeSelf);
    #endregion

    // ==================== ОБРАБОТЧИКИ КНОПОК ====================
    private void OnContinueButton()
    {
        if (isDialogueMode)
        {
            ShowNextDialoguePhrase();
            return;
        }

        if (messageQueue.Count == 0 && onCompleteCallback != null)
        {
            var callback = onCompleteCallback;
            onCompleteCallback = null;
            callback.Invoke();
        }
        else
        {
            ShowNextTrainingMessage();
        }
    }

    private void OnStartGame()
    {
        isShowing = false;
        if (startGameButton != null) startGameButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (messageText != null) messageText.text = "";
        if (infoPanel != null) infoPanel.SetActive(false);
        var callback = onCompleteCallback;
        onCompleteCallback = null;
        callback?.Invoke();
    }

    // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================
    private bool EnsureUIReady()
    {
        if (infoPanel != null && messageText != null && continueButton != null)
            return true;

        Debug.LogWarning("UI элементы не привязаны, попытка перепривязки...");
        RebindUIElements();

        if (infoPanel == null || messageText == null || continueButton == null)
        {
            Debug.LogError("Не удалось привязать UI элементы, сообщение не будет показано.");
            return false;
        }

        return true;
    }
}