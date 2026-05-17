using UnityEngine;
using TMPro; 
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    public TMP_Text scoreText;      
    public TMP_Text dotsLeftText;
    [SerializeField] private LivesUI livesUI; // ссылка на LivesUI (сердечки)
    
    // Игровые переменные
    [HideInInspector] public int score;
    [HideInInspector] public int remainingDots;
    public bool isPowerUpMode { get; private set; }
    
    private float powerUpTimer;
    private float powerUpDuration = 8f;
    // Добавить в начало класса
    private bool gameStarted = false;

// Добавить метод
    public void StartGame()
    {
       gameStarted = true;
       Debug.Log("Game started!");
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        score = 0;
        
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        GameObject[] superPellets = GameObject.FindGameObjectsWithTag("SuperPellet");
        remainingDots = pellets.Length + superPellets.Length;
        
        Debug.Log($"Найдено обычных точек: {pellets.Length}, супер-точек: {superPellets.Length}, всего: {remainingDots}");
        
        if (livesUI == null)
            livesUI = FindObjectOfType<LivesUI>();
        
        isPowerUpMode = false;
        UpdateUI();
        gameStarted = false;
        Invoke("EnableGame", 0.1f);
    }
    void EnableGame()
    {
        gameStarted = true;
    }
    void Update()
    {
        if (isPowerUpMode)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0) DisablePowerUpMode();
        }
    }
    
    // ЕДИНСТВЕННЫЙ метод UpdateLives, вызываемый из PlayerController3
    public void UpdateLives(int lives)
    {
        if (livesUI != null)
            livesUI.UpdateLives(lives);
        else
            Debug.LogWarning("LivesUI не найден!");
    }
    
    public void EatDot(int points, bool isSuperDot)
    {
        score += points;
        remainingDots--;
        Debug.Log($"Съедена точка! +{points} очков. Осталось: {remainingDots}");
        
        if (isSuperDot) EnablePowerUpMode();
        UpdateUI();
        if (remainingDots <= 0) WinLevel();
    }
    
    void EnablePowerUpMode()
    {
        isPowerUpMode = true;
        powerUpTimer = powerUpDuration;
        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in ghosts) ghost.SetEdible(true);
        Debug.Log("Супер-режим активирован на " + powerUpDuration + " секунд!");
    }
    
    void DisablePowerUpMode()
    {
        isPowerUpMode = false;
        Ghost[] ghosts = FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in ghosts) ghost.SetEdible(false);
        Debug.Log("Супер-режим закончился!");
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "ОЧКИ: " + score;
        else
            Debug.LogWarning("Score Text не назначен!");
        
        if (dotsLeftText != null)
            dotsLeftText.text = "ОСТАЛОСЬ: " + remainingDots;
        else
            Debug.LogWarning("Dots Left Text не назначен!");
    }
    
    void WinLevel()
    {
        if (!gameStarted) return;
        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null) manager.ShowWin();
    // Можно отключить управление игроком или остановить время
    }
}