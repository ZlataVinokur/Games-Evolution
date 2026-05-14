using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public string[] answers = new string[3];
        public int correctAnswerIndex; // 0, 1 или 2
    }

    [Header("UI элементы")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public Button[] answerButtons; // 3 кнопки
    public GameObject finalPanel;
    public Button mainMenuButton;
    public Button nextModuleButton;

    [Header("Вопросы")]
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    private int currentQuestion = 0;
    private int score = 0;
    [Header("Визуальная обратная связь")]
    public Image flashImage;          // панель для вспышки (разместите поверх всего)
    public float flashDuration = 0.3f;

    private bool isWaiting = false;   // блокировка повторных кликов
    public GameObject introPanel;          // новая панель с поздравлением
    public Button startQuizButton;
    void Start()
    {
        quizPanel.SetActive(false);
        finalPanel.SetActive(false);
        introPanel.SetActive(false);

        // Привязываем кнопки финальной панели
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (nextModuleButton != null)
            nextModuleButton.onClick.AddListener(GoToNextModule);
        if (startQuizButton != null)
            startQuizButton.onClick.AddListener(StartQuiz);
        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = new Color(0, 0, 0, 0);
        }
    }
    public void ShowIntroPanel()
    {
        introPanel.SetActive(true);
        quizPanel.SetActive(false);
        finalPanel.SetActive(false);
    }
    public void StartQuiz()
    {
        introPanel.SetActive(false);
        currentQuestion = 0;
        score = 0;
        quizPanel.SetActive(true);
        finalPanel.SetActive(false);
        ShowQuestion();
    }
    void Awake()
    {
        questions = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                questionText = "КАКАЯ МЕХАНИКА ПОЯВИЛАСЬ В ARKANOID?",
                answers = new string[] { "СТРЕЛЬБА", "ДВИЖЕНИЕ И ОТСКОК", "СБОР ПРЕДМЕТОВ" },
                correctAnswerIndex = 1
            },
            new QuizQuestion
            {
                questionText = "ЧТО ТАКОЕ ПРОГРЕССИЯ СЛОЖНОСТИ В SPACE INVADERS?",
                answers = new string[] { "УВЕЛИЧЕНИЕ ЖИЗНЕЙ", "УСКОРЕНИЕ ВРАГОВ", "ПОЯВЛЕНИЕ БОССА" },
                correctAnswerIndex = 1
            },
            new QuizQuestion
            {
                questionText = "ЧТО ДАЮТ СУПЕР-ТОЧКИ В PAC-MAN?",
                answers = new string[] { "ДОП. ЖИЗНЬ", "УЯЗВИМОСТЬ ПРИВИДЕНИЙ", "ЗАМЕДЛЕНИЕ" },
                correctAnswerIndex = 1
            },
            new QuizQuestion
            {
                questionText = "ОСНОВНАЯ МЕХАНИКА TETRIS?",
                answers = new string[] { "СБОР МОНЕТ", "УПРАВЛЕНИЕ ФИГУРАМИ И ЗАПОЛНЕНИЕ РЯДОВ", "СТРЕЛЬБА" },
                correctAnswerIndex = 1
            },
            new QuizQuestion
            {
                questionText = "В КАКОЙ ИГРЕ ВПЕРВЫЕ ПОЯВИЛИСЬ ВОЛНЫ ВРАГОВ?",
                answers = new string[] { "ARKANOID", "SPACE INVADERS", "PAC-MAN" },
               correctAnswerIndex = 1
            }
        };
    }
    private void ShowQuestion()
    {
        if (currentQuestion < questions.Count)
        {
            QuizQuestion q = questions[currentQuestion];
            questionText.text = q.questionText;
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < q.answers.Length)
                {
                    answerButtons[i].GetComponentInChildren<TMP_Text>().text = q.answers[i];
                    int answerIndex = i;
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
                    answerButtons[i].interactable = true;
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            EndQuiz();
        }
    }
    private void OnAnswerSelected(int selected)
    {
        if (isWaiting) return; // уже обрабатывается анимация
        isWaiting = true;

        QuizQuestion q = questions[currentQuestion];
        bool isCorrect = (selected == q.correctAnswerIndex);
        if (isCorrect) score++;

        StartCoroutine(ShowAnswerFeedback(isCorrect, () =>
        {
            currentQuestion++;
            isWaiting = false;
            ShowQuestion();
        }));
    }
    private IEnumerator ShowAnswerFeedback(bool correct, System.Action onComplete)
    {
        // Блокируем кнопки
        foreach (var btn in answerButtons)
            if (btn != null) btn.interactable = false;

        Color feedbackColor = correct ? Color.green : Color.red;
        feedbackColor.a = 0.6f;

        // Вспышка
        if (flashImage != null)
        {
            flashImage.color = feedbackColor;
            yield return new WaitForSeconds(flashDuration);
            // Плавное исчезновение
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.6f, 0f, elapsed / flashDuration);
                flashImage.color = new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, alpha);
                yield return null;
            }
            flashImage.color = new Color(0, 0, 0, 0);
        }
        else
        {
            yield return new WaitForSeconds(flashDuration);
        }
        foreach (var btn in answerButtons)
            if (btn != null) btn.interactable = true;

        onComplete?.Invoke();
    }
    private void EndQuiz()
    {
        quizPanel.SetActive(false);
        finalPanel.SetActive(true);
        TMP_Text resultText = finalPanel.GetComponentInChildren<TMP_Text>();
        if (resultText != null)
            resultText.text = $"Вы ответили правильно на {score} из {questions.Count} вопросов!";
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("0_Menu");
    }

    private void GoToNextModule()
    {
        // Загрузка следующего модуля (2D). Укажите точное имя сцены.
        SceneManager.LoadScene("2D_Module");
    }
}