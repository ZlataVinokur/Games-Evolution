using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject winPanel;

    [Header("Buttons")]
    public Button gameOverRestartButton;
    public Button gameOverMenuButton;
    public Button pauseContinueButton;
    public Button pauseRestartButton;
    public Button pauseMenuButton;
    public Button winNextButton;

    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isWin = false;
    private bool wasCursorLocked;

    public bool[] modulesCompleted;  // 3 модуля

    // старые поля
    public int totalScore;
    public bool[] levelsCompleted;
    private int currentLevelScore = 0;
    private int totalBricksInCurrentLevel = 0;
    private int destroyedBricksCount = 0;
    [SerializeField] private int menuSceneName = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame(); // старый метод LoadGame
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        void InitializeModules()
        {
            if (modulesCompleted == null || modulesCompleted.Length != 3)
                modulesCompleted = new bool[3];
        }
    }

    void Start()
    {
        // Подписываемся на загрузку сцен, чтобы перепривязывать UI
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindAndBindUI();
        HideAllPanels();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOver && !isWin)  // пауза только если не Game Over и не Win
            {
                TogglePause();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Перепривязываем UI при загрузке любой сцены
        FindAndBindUI();
        HideAllPanels();
        isPaused = false;
        isGameOver = false;
        isWin = false;
        Time.timeScale = 1f;
    }

    private void FindAndBindUI()
    {
        // Ищем Canvas (можно задать тег "GameCanvas" или искать первый)
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Canvas не найден в сцене! UI панели не будут работать.");
            return;
        }

        // Если панели не назначены в инспекторе, ищем по имени в Canvas
        if (gameOverPanel == null) gameOverPanel = FindChildByName(canvas.transform, "GameOverPanel");
        if (pausePanel == null) pausePanel = FindChildByName(canvas.transform, "PausePanel");
        if (winPanel == null) winPanel = FindChildByName(canvas.transform, "WinPanel");

        // Ищем кнопки внутри панелей
        if (gameOverPanel != null)
        {
            if (gameOverRestartButton == null) gameOverRestartButton = FindButtonInPanel(gameOverPanel, "RestartButton");
            if (gameOverMenuButton == null) gameOverMenuButton = FindButtonInPanel(gameOverPanel, "MenuButton");
        }
        if (pausePanel != null)
        {
            if (pauseContinueButton == null) pauseContinueButton = FindButtonInPanel(pausePanel, "ContinueButton");
            if (pauseRestartButton == null) pauseRestartButton = FindButtonInPanel(pausePanel, "RestartButton");
            if (pauseMenuButton == null) pauseMenuButton = FindButtonInPanel(pausePanel, "MenuButton");
        }
        if (winPanel != null)
        {
            if (winNextButton == null) winNextButton = FindButtonInPanel(winPanel, "NextButton");
        }

        // Подписываем кнопки (удаляем старые подписки, чтобы не было дублей)
        if (gameOverRestartButton != null) { gameOverRestartButton.onClick.RemoveAllListeners(); gameOverRestartButton.onClick.AddListener(RestartCurrentLevel); }
        if (gameOverMenuButton != null) { gameOverMenuButton.onClick.RemoveAllListeners(); gameOverMenuButton.onClick.AddListener(GoToMainMenu); }
        if (pauseContinueButton != null) { pauseContinueButton.onClick.RemoveAllListeners(); pauseContinueButton.onClick.AddListener(TogglePause); }
        if (pauseRestartButton != null) { pauseRestartButton.onClick.RemoveAllListeners(); pauseRestartButton.onClick.AddListener(RestartCurrentLevel); }
        if (pauseMenuButton != null) { pauseMenuButton.onClick.RemoveAllListeners(); pauseMenuButton.onClick.AddListener(GoToMainMenu); }
        if (winNextButton != null) { winNextButton.onClick.RemoveAllListeners(); winNextButton.onClick.AddListener(LoadNextLevel); }
    }

    private GameObject FindChildByName(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        if (t != null) return t.gameObject;
        // Рекурсивный поиск, если не нашли прямого потомка
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child.gameObject;
        return null;
    }

    private Button FindButtonInPanel(GameObject panel, string buttonName)
    {
        Transform btn = panel.transform.Find(buttonName);
        return btn != null ? btn.GetComponent<Button>() : null;
    }

    private void HideAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    // ------------------- Публичные методы для вызова из уровней -------------------
    public void ShowGameOver()
    {
        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        else Debug.LogError("GameOverPanel не найден!");
        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void ShowWin()
    {
        isWin = true;
        isPaused = false;
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);
        else Debug.LogError("WinPanel не найден!");
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void LoadQuizForCurrentModule(int completedModuleSceneIndex)
    {
        Time.timeScale = 1f;
        // Разблокируем курсор перед загрузкой квиза
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CompleteModule(completedModuleSceneIndex);

        // Сохраняем индекс модуля перед загрузкой сцены квиза
        PlayerPrefs.SetInt("CompletedModuleIndex", completedModuleSceneIndex);
        PlayerPrefs.Save();

        // Загружаем сцену квиза
        SceneManager.LoadScene("Quiz");
    }

    public void TogglePause()
    {
        if (isGameOver || isWin) return;

        if (!isPaused) // перед паузой
        {
            wasCursorLocked = (Cursor.lockState == CursorLockMode.Locked);
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (wasCursorLocked)
        {
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isPaused;
        }

        if (pausePanel != null) pausePanel.SetActive(isPaused);
    }

    public void RestartCurrentLevel()
    {
        isPaused = false;
        isGameOver = false;
        isWin = false;
        Time.timeScale = 1f;
        HideAllPanels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        isPaused = false;
        isGameOver = false;
        isWin = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
    public void LoadNextLevel()
    {
        isPaused = false;
        isGameOver = false;
        isWin = false;
        Time.timeScale = 1f;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            Debug.Log("Игра пройдена! Все уровни завершены.");
    }

    public void CompleteModule(int moduleIndex)
    {
        if (moduleIndex < modulesCompleted.Length && !modulesCompleted[moduleIndex])
        {
            modulesCompleted[moduleIndex] = true;
            SaveGame();

            // Разблокировать статьи модуля
            var infoSys = UnifiedInfoSystem.Instance;
            if (infoSys != null)
            {
                var articles = infoSys.AllArticles.Where(a => a.moduleIndex == moduleIndex);
                foreach (var art in articles)
                    infoSys.UnlockArticle(art.articleId);
            }
        }
    }

    // ==================== СТАРЫЕ МЕТОДЫ (без изменений) ====================
    public void CompleteLevel(int levelIndex, int score)
    {
        if (!levelsCompleted[levelIndex])
        {
            levelsCompleted[levelIndex] = true;
            totalScore += score;
            SaveGame();
        }
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 0) return true;
        return levelsCompleted[levelIndex - 1];
    }

    public void AddScore(int points)
    {
        currentLevelScore += points;
        totalScore += points;
        SaveGame();
        Debug.Log($"Добавлено {points} очков. Всего: {totalScore}, за уровень: {currentLevelScore}");
    }

    public void ResetCurrentLevelScore()
    {
        totalScore -= currentLevelScore;
        currentLevelScore = 0;
        SaveGame();
        Debug.Log($"Очки за уровень сброшены. Итого: {totalScore}");
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        for (int i = 0; i < levelsCompleted.Length; i++)
            PlayerPrefs.SetInt("Level_" + i, levelsCompleted[i] ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Игра сохранена. Очки: " + totalScore);

        for (int i = 0; i < modulesCompleted.Length; i++)
            PlayerPrefs.SetInt("Module_" + i, modulesCompleted[i] ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        if (levelsCompleted == null) levelsCompleted = new bool[4];
        for (int i = 0; i < levelsCompleted.Length; i++)
            levelsCompleted[i] = PlayerPrefs.GetInt("Level_" + i, 0) == 1;
        Debug.Log("Игра загружена. Очки: " + totalScore);

        if (modulesCompleted == null) modulesCompleted = new bool[3];
        for (int i = 0; i < modulesCompleted.Length; i++)
            modulesCompleted[i] = PlayerPrefs.GetInt("Module_" + i, 0) == 1;
    }

    public void ResetGame()
    {
        totalScore = 0;
        for (int i = 0; i < levelsCompleted.Length; i++) levelsCompleted[i] = false;
        SaveGame();
        Debug.Log("Прогресс сброшен");
    }

    public void RegisterBrick() { totalBricksInCurrentLevel++; }
    public void BrickDestroyed()
    {
        destroyedBricksCount++;
        if (destroyedBricksCount >= totalBricksInCurrentLevel) LevelComplete();
    }
    public void ResetLevelBrickCounter()
    {
        totalBricksInCurrentLevel = 0;
        destroyedBricksCount = 0;
    }
    private void LevelComplete()
    {
        if (levelsCompleted != null && levelsCompleted.Length > 0)
        {
            levelsCompleted[0] = true;
            SaveGame();
        }
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.ShowWin();
        else Debug.LogError("GameOverManager not found on scene!");
    }

    public List<string> unlockedLevels = new List<string>();
    public Dictionary<string, bool> unlockedArticles = new Dictionary<string, bool>();
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public void CompleteLevel(string levelId)
    {
        if (!unlockedLevels.Contains(levelId)) unlockedLevels.Add(levelId);
    }
    public void UnlockArticle(string articleId)
    {
        if (!unlockedArticles.ContainsKey(articleId)) unlockedArticles[articleId] = true;
    }
    public void SetFlag(string key, bool value) => flags[key] = value;
    public bool GetFlag(string key) => flags.ContainsKey(key) && flags[key];

    // Этот метод вызывается из меню (оставить для совместимости)
    public void GoToMenu() => GoToMainMenu();


    // 2 module RPG prefs saving

    private bool[] rpgMeters = new bool[3]; // сохранять в PlayerPrefs или просто в памяти, т.к. сцена перезапускается внутри сессии

    public void SetRPGMeter(int index, bool value)
    {
        rpgMeters[index] = value;
    }
    public bool[] GetRPGMeters() => rpgMeters;
}