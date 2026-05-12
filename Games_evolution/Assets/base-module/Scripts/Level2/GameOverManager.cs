using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverManager : MonoBehaviour
{
    [Header("Панели")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Кнопки (опционально, если не назначены в инспекторе)")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;

    private void Start()
    {
        // Подписываем кнопки
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);

        // Убедимся, что панели скрыты
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        // Остановить игру? Если нужно, можно Time.timeScale = 0;
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        // Перезагрузить текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        // Загружаем главное меню (индекс 0)
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        // Загружаем следующую сцену (текущий индекс + 1)
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}