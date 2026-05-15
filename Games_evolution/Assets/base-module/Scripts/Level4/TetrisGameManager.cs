using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TetrisGameManager : MonoBehaviour
{
    public TetrisGrid grid;
    public TetrisSpawner spawner;
    public TMP_Text scoreText;
    public TMP_Text linesText;
    public GameObject gameOverPanel;
    public Animator pixelAnimator;
    public Image flashImage;
    public bool IsGameOver { get; private set; }
    public GameManager GameManager; 
    private int currentScore = 0;
    private int targetRows = 3;
    private int rowsClearedTotal = 0;

    private float thinkCooldown = 0f;
    private float thinkInterval = 12f;
    private bool controlsEnabled = true;
    private bool gameStarted = false;
    public QuizManager quizManager;
    private void Start()
    {
        IsGameOver = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (grid != null)
        {
            grid.OnRowCleared += OnRowClearedHandler;
            grid.OnGameOver += GameOver;
        }

        UpdateUI();
        thinkCooldown = thinkInterval;
        if (pixelAnimator != null) pixelAnimator.Rebind();

        // Подготовка игры (без запуска геймплея)
        PrepareGame();
    }

    private void PrepareGame()
    {
        currentScore = 0;
        rowsClearedTotal = 0;
        IsGameOver = false;
        if (grid != null) grid.ResetGrid();
        UpdateUI();
        // Не спавним фигуру и не устанавливаем gameStarted = true – ждём вызова StartGame()
    }

    // Вызывается из Level4Manager после обучения
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;
        controlsEnabled = true;   // разблокируем управление (фактически его разблокирует Level4Manager)
        if (spawner != null) spawner.EnableSpawning();
        thinkCooldown = thinkInterval;
    }

    private void Update()
    {
        if (!controlsEnabled) return;
        if (!gameStarted) return;

        if (!IsGameOver && pixelAnimator != null)
        {
            thinkCooldown -= Time.deltaTime;
            if (thinkCooldown <= 0f)
            {
                pixelAnimator.SetTrigger("Think");
                thinkCooldown = thinkInterval + Random.Range(-2f, 3f);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
        TetrisPiece piece = FindObjectOfType<TetrisPiece>();
        if (piece != null) piece.SetControlsEnabled(enabled);
    }

    private void OnRowClearedHandler(int points)
    {
        StartCoroutine(ShakeCamera(0.1f, 0.1f));
        StartCoroutine(FlashNow());
        
        if (IsGameOver) return;

        currentScore += points;
        rowsClearedTotal++;
        UpdateUI();

        if (pixelAnimator != null)
            pixelAnimator.SetTrigger("Happy");

        if (rowsClearedTotal % 5 == 1)
        {
            TetrisPiece piece = FindObjectOfType<TetrisPiece>();
            if (piece != null) piece.IncreaseFallSpeed(0.8f);
        }

        if (rowsClearedTotal >= targetRows)
            WinLevel();
    }
     private void WinLevel()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log($"Уровень пройден! Очищено рядов: {rowsClearedTotal}");
        if (GameManager.Instance != null)
            GameManager.Instance.CompleteLevel(3, currentScore);

        if (quizManager != null)
            quizManager.ShowIntroPanel();
        else
            Debug.LogWarning("QuizManager not found!");
    }
    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (GameManager != null)
            GameManager.ShowGameOver();
        else if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("0_Menu");
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "ОЧКИ: " + currentScore;
        if (linesText != null) linesText.text = "РЯД: " + rowsClearedTotal + " / " + targetRows;
    }

    private void OnDestroy()
    {
        if (grid != null)
        {
            grid.OnRowCleared -= OnRowClearedHandler;
            grid.OnGameOver -= GameOver;
        }
    }

    private IEnumerator FlashNow()
    {
        flashImage.color = new Color(1f, 1f, 1f, 0.15f);
        yield return new WaitForSeconds(0.07f);
        flashImage.color = new Color(1f, 1f, 1f, 0f);
    }

    private IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;
        Vector3 originalPos = cam.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.transform.position = originalPos + (Vector3)Random.insideUnitCircle * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = originalPos;
    }
}