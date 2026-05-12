using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance;

    [Header("UI Elements")]
    public GameObject infoPanel;
    public TextMeshProUGUI messageText; // или Text
    public Button continueButton;
    public Button startGameButton;

    [Header("Настройки")]
    public string defaultMessage = "Приступай к игре! Следи за подсказками.";

    private bool isShowing = false;
    private Queue<string> messageQueue = new Queue<string>();
    private System.Action onCompleteCallback;
    private Coroutine timedMessageCoroutine;

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
        }

        if (continueButton != null) continueButton.onClick.AddListener(OnContinueButton);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGame);
    }

    // Последовательные сообщения (обучение) – блокирующие, с кнопкой Continue
    public void ShowSequentialMessages(List<string> messages, System.Action onComplete)
    {
        if (isShowing) return;
        messageQueue.Clear();
        foreach (var msg in messages) messageQueue.Enqueue(msg);
        onCompleteCallback = onComplete;
        ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            messageText.text = messageQueue.Dequeue();
            continueButton.gameObject.SetActive(true);
            startGameButton.gameObject.SetActive(false);
        }
        else
        {
            messageText.text = "ГОТОВ НАЧАТЬ ИГРУ?";
            startGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }
    }

    private void OnContinueButton()
    {
     
        ShowNextMessage();
        
    }

    private void OnStartGame()
    {
        // Не закрываем панель, просто убираем кнопки и устанавливаем базовый текст
        startGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        messageText.text = defaultMessage;

        // Вызываем коллбэк (запуск игры, разблокировка управления)
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    // Неблокирующее сообщение с таймером (без кнопки)
    public void ShowTimedMessage(string message, float displayDuration)
    {
        if (timedMessageCoroutine != null) StopCoroutine(timedMessageCoroutine);
        timedMessageCoroutine = StartCoroutine(TimedMessageCoroutine(message, displayDuration));
    }
    public void ShowHint(string hintText, System.Action callback = null)
    {
        if (isShowing) return;
        messageText.text = hintText;
        infoPanel.SetActive(true);
        isShowing = true;
        SetPlayerControlEnabled(false);
        continueButton.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(false);
        onCompleteCallback = callback;
    }
    private IEnumerator TimedMessageCoroutine(string message, float displayDuration)
    {
        if (!infoPanel.activeSelf) infoPanel.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(displayDuration);
        messageText.text = defaultMessage; // возвращаем базовый текст
    }
    private void SetPlayerControlEnabled(bool enabled)
    {
    // Этот метод может быть пустым, если вы управляете контроллерами в LevelManager.
    // Оставьте заглушку, чтобы компилятор не ругался.
    }

    private void HidePanelAndResume()
    {

       isShowing = false;
    // Управление включаем через LevelManager, поэтому здесь ничего не делаем.
    }
}