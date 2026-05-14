using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject infoPanel;
    public TextMeshProUGUI messageText; // ��� Text
    public Button continueButton;
    public Button startGameButton;

    [Header("���������")]
    public string defaultMessage = "��������� � ����! ����� �� �����������.";

    private bool isShowing = false;
    private Queue<string> messageQueue = new Queue<string>();
    private System.Action onCompleteCallback;
    private Coroutine timedMessageCoroutine;
    void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (continueButton == null) Debug.LogError("continueButton not assigned!");
        else continueButton.onClick.AddListener(OnContinueButton);

        if (continueButton != null) continueButton.onClick.AddListener(OnContinueButton);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGame);
    }

    // ���������������� ��������� (��������) � �����������, � ������� Continue
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
            messageText.text = "����� ������ ����?";
            startGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }
    }

    private void OnContinueButton()
    {
        Debug.Log("Continue button clicked");
        ShowNextMessage();

    }

    private void OnStartGame()
    {
        // �� ��������� ������, ������ ������� ������ � ������������� ������� �����
        startGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        messageText.text = defaultMessage;

        // �������� ������� (������ ����, ������������� ����������)
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    // ������������� ��������� � �������� (��� ������)
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
        messageText.text = defaultMessage; // ���������� ������� �����
    }
    private void SetPlayerControlEnabled(bool enabled)
    {
        // ���� ����� ����� ���� ������, ���� ���������� ������������� � LevelManager.
    }

    private void HidePanelAndResume()
    {

        isShowing = false;
        // ���������� �������� ����� LevelManager, ������� ����� ������ �� ������.
    }



    //2d ������

    [SerializeField] private List<Article> allArticles;
    public List<Article> AllArticles => allArticles;

    public void UnlockArticle(string articleId)
    {
        Article article = allArticles.Find(a => a.articleId == articleId);
        if (article == null) return;

        GameManager.Instance.UnlockArticle(articleId);

        // ���������� ������ ����������� � ������� ����������
        DialogueSystem.Instance.ShowDialogue(
            new[] { article.shortAnnotation },
            speaker: "encyclopedia",
            emotion: "happy"
        );
    }

    public string GetArticleText(string articleId)
    {
        if (!GameManager.Instance.unlockedArticles.ContainsKey(articleId))
            return "������ �������������.";
        Article article = allArticles.Find(a => a.articleId == articleId);
        return article != null ? article.fullText : "������ �� �������.";
    }



    // 3d ������
    public void TellFact(string[] factLines, string emotion = "neutral")
    {
        DialogueSystem.Instance.ShowDialogue(factLines, "encyclopedia", emotion);
    }
}