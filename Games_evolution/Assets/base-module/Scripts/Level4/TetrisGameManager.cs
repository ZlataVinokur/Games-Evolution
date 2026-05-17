using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TetrisGameManager : MonoBehaviour
{
    public TetrisGrid grid;
    public TetrisSpawner spawner;
    public TMP_Text scoreText;
    public TMP_Text linesText;
    public Animator pixelAnimator;
    public Image flashImage;

    public bool IsGameOver { get; private set; }

    private int currentScore = 0;
    private int targetRows = 3;
    private int rowsClearedTotal = 0;
    private float thinkCooldown = 0f;
    private float thinkInterval = 12f;
    private bool controlsEnabled = true;
    private bool gameStarted = false;

    // Индекс этого уровня в сценах (0,1,2,3...). По умолчанию 3 для Level4.
    [SerializeField] private int levelIndex = 3;

    private void Start()
    {
        IsGameOver = false;
        if (grid != null)
        {
            grid.OnRowCleared += OnRowClearedHandler;
            grid.OnGameOver += GameOver;
        }

        UpdateUI();
        thinkCooldown = thinkInterval;
        if (pixelAnimator != null) pixelAnimator.Rebind();

        PrepareGame();
    }

    private void PrepareGame()
    {
        currentScore = 0;
        rowsClearedTotal = 0;
        IsGameOver = false;
        if (grid != null) grid.ResetGrid();
        UpdateUI();
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;
        controlsEnabled = true;
        if (spawner != null) spawner.EnableSpawning();
        thinkCooldown = thinkInterval;
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
        TetrisPiece piece = FindObjectOfType<TetrisPiece>();
        if (piece != null) piece.SetControlsEnabled(enabled);
    }

    private void Update()
    {
        if (!controlsEnabled || !gameStarted || IsGameOver) return;

        // Анимация "думает"
        if (pixelAnimator != null)
        {
            thinkCooldown -= Time.deltaTime;
            if (thinkCooldown <= 0f)
            {
                pixelAnimator.SetTrigger("Think");
                thinkCooldown = thinkInterval + Random.Range(-2f, 3f);
            }
        }

        // Обработка паузы через GameManager (Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
            else
                Debug.LogWarning("GameManager.Instance не найден, пауза недоступна");
        }
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

        // Увеличение скорости каждые 5 рядов
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
        Debug.Log($"Уровень пройден! Очищено рядов: {rowsClearedTotal}, очки: {currentScore}");

        if (GameManager.Instance != null)
        {
            // Завершаем уровень через GameManager
            GameManager.Instance.CompleteLevel(levelIndex, currentScore);
            // Загружаем квиз для модуля (5-й модуль или индекс сцены квиза)
            GameManager.Instance.LoadQuizForCurrentModule(1);
        }
        else
        {
            Debug.LogError("GameManager.Instance отсутствует! Невозможно завершить уровень.");
        }
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("Game Over в тетрисе");

        if (GameManager.Instance != null)
            GameManager.Instance.ShowGameOver();
        else
            Debug.LogError("GameManager.Instance не найден для показа GameOver");
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
        if (flashImage == null) yield break;
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