using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class ModuleQuiz
    {
        public int moduleSceneIndex;
        public List<QuizQuestion> questions;
    }

    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public string[] answers = new string[3];
        public int correctAnswerIndex;
    }

    public static QuizManager Instance { get; private set; }

    [Header("UI")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public GameObject finalPanel;
    public Button mainMenuButton;
    public Button nextModuleButton;
    public GameObject introPanel;
    public Button startQuizButton;
    public Image flashImage;
    public float flashDuration = 0.3f;
    [Header("Панель провала")]
    public GameObject failPanel;
    public Button failEncyclopediaButton;
    public TMP_Text failMessageText;

    [Header("Настройка модулей")]
    public List<ModuleQuiz> modules = new List<ModuleQuiz>();

    private int currentModuleIndex = -1;
    private int currentQuestion = 0;
    private int score = 0;
    private bool isWaiting = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            // Инициализация UI (по умолчанию всё выключено)
            quizPanel.SetActive(false);
            finalPanel.SetActive(false);
            introPanel.SetActive(false);

            // Кнопки и события инициализируем всегда (они могут понадобиться)
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

            // Проверяем, не возвращаемся ли мы из справочника
            if (PlayerPrefs.HasKey("ReturnToQuiz"))
            {
                // Возвращаемся - удаляем флаг, но оставляем CompletedModuleIndex
                PlayerPrefs.DeleteKey("ReturnToQuiz");
            }

            // Загружаем сохраненный индекс модуля
            if (PlayerPrefs.HasKey("CompletedModuleIndex"))
            {
                int moduleIndex = PlayerPrefs.GetInt("CompletedModuleIndex");
                LoadQuizForModule(moduleIndex);
            }
            else
            {
                // Нет индекса – переходим в меню
                GoToMainMenu();
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (failEncyclopediaButton != null)
            failEncyclopediaButton.onClick.AddListener(GoToEncyclopedia);
    }

    void OnEnable()
    {
        Time.timeScale = 1f;
    }

    public void LoadQuizForModule(int completedModuleSceneIndex)
    {
        // **Проверка: только индекс 10 активирует квиз**
        if (SceneManager.GetActiveScene().buildIndex != 10)
        {
            Debug.Log($"Квиз не предназначен для этой сцены. Загружаем меню.");
            // Скрываем все панели квиза
            if (quizPanel != null) quizPanel.SetActive(false);
            if (finalPanel != null) finalPanel.SetActive(false);
            if (introPanel != null) introPanel.SetActive(false);
            return;
        }

        // Ищем модуль с нужным индексом (например, модуль RPG)
        for (int i = 0; i < modules.Count; i++)
        {
            if (modules[i].moduleSceneIndex == completedModuleSceneIndex)
            {
                currentModuleIndex = i;
                ShowIntroPanel();
                return;
            }
        }
        Debug.LogError($"Квиз для модуля {completedModuleSceneIndex} не найден");
        GoToMainMenu();
    }

    public void ShowIntroPanel()
    {
        // Убедимся, что мы в разрешённом режиме (индекс 10)
        if (currentModuleIndex == -1)
        {
            GoToMainMenu();
            return;
        }
        introPanel.SetActive(true);
        quizPanel.SetActive(false);
        finalPanel.SetActive(false);
    }

    public void StartQuiz()
    {
        if (currentModuleIndex == -1 || SceneManager.GetActiveScene().buildIndex != 10)
        {
            return;
        }
        introPanel.SetActive(false);
        currentQuestion = 0;
        score = 0;
        quizPanel.SetActive(true);
        finalPanel.SetActive(false);
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        var questions = modules[currentModuleIndex].questions;

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
                    answerButtons[i].gameObject.SetActive(true);
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
        if (isWaiting) return;
        isWaiting = true;

        var questions = modules[currentModuleIndex].questions;
        bool isCorrect = (selected == questions[currentQuestion].correctAnswerIndex);
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
        foreach (var btn in answerButtons)
            if (btn != null) btn.interactable = false;

        Color feedbackColor = correct ? Color.green : Color.red;
        feedbackColor.a = 0.6f;

        if (flashImage != null)
        {
            flashImage.color = feedbackColor;
            yield return new WaitForSeconds(flashDuration);
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

        var questions = modules[currentModuleIndex].questions;
        float percent = (float)score / questions.Count * 100f;

        if (GameManager.Instance != null)
        {
            int moduleIndex = GetModuleNumberFromSceneIndex(modules[currentModuleIndex].moduleSceneIndex);
            GameManager.Instance.CompleteModule(moduleIndex);
        }

        if (percent >= 50f)
        {            

            finalPanel.SetActive(true);
            TMP_Text resultText = finalPanel.GetComponentInChildren<TMP_Text>();
            if (resultText != null)
                resultText.text = $"Вы ответили правильно на {score} из {questions.Count} вопросов!";

            if (nextModuleButton != null)
            {
                bool isLastModule = (currentModuleIndex == modules.Count - 1);
                if (isLastModule)
                {
                    // Последний модуль - кнопка ведёт в меню
                    nextModuleButton.onClick.RemoveAllListeners();
                    nextModuleButton.onClick.AddListener(GoToMainMenu);
                    nextModuleButton.GetComponentInChildren<TMP_Text>().text = "В меню";
                }
                else
                {
                    // Не последний - следующий модуль
                    nextModuleButton.onClick.RemoveAllListeners();
                    nextModuleButton.onClick.AddListener(GoToNextModule);
                    nextModuleButton.GetComponentInChildren<TMP_Text>().text = "Продолжить";
                }
                nextModuleButton.gameObject.SetActive(true);
            }

            // Удаляем временные ключи после успешного прохождения
            PlayerPrefs.DeleteKey("CompletedModuleIndex");
            PlayerPrefs.DeleteKey("RetryModuleSceneIndex");
            PlayerPrefs.Save();
        }
        else
        {
            // Провал - показываем панель со ссылкой на справочник
            failPanel.SetActive(true);
            if (failMessageText != null)
                failMessageText.text = $"Квиз не пройден ({score}/{questions.Count}). Изучите материалы в справочнике и попробуйте снова.";

            // Сохраняем какой модуль пытались пройти
            PlayerPrefs.SetInt("RetryModuleSceneIndex", modules[currentModuleIndex].moduleSceneIndex);
            PlayerPrefs.Save();
        }
    }

    private int GetModuleNumberFromSceneIndex(int sceneIndex)
    {
        if (sceneIndex == 1) return 0;
        if (sceneIndex == 7) return 1;
        if (sceneIndex == 8) return 2;
        return -1;
    }

    private void GoToEncyclopedia()
    {
        // Сохраняем что нужно вернуться в квиз
        PlayerPrefs.SetInt("ReturnToQuiz", 1);
        PlayerPrefs.Save();

        SceneLoader.LoadScene("ReferenceBook");
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("0_Menu");
    }

    private void GoToNextModule()
    {
        if (currentModuleIndex + 1 < modules.Count)
        {
            SceneManager.LoadScene(modules[currentModuleIndex + 1].moduleSceneIndex);
        }
    }
}