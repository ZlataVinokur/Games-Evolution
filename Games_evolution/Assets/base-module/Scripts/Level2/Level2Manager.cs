using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level2Manager : MonoBehaviour
{
    [Header("Настройки уровня")]
    [SerializeField] private int levelIndex = 1;
    
    [Header("Ссылки на компоненты")]
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private PlayerController playerController;
    
    private bool levelCompleted = false;
    
    void Start()
    {
        // Если ссылки не назначены в инспекторе, попробуем найти
        if (waveSpawner == null) waveSpawner = GetComponent<WaveSpawner>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        
        // Блокируем управление и спавн до обучения
        if (playerController != null) playerController.SetControlsEnabled(false);
        if (waveSpawner != null) waveSpawner.enabled = false;
        
        // Показываем обучение (один раз)
        if (PlayerPrefs.GetInt("Level2_TutorialShown", 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            StartGame();
        }
    }
    
    private void ShowTutorial()
    {
        List<string> messages = new List<string>
        {
            "ПИКСЕЛЬ, ТЕПЕРЬ ТЫ МОЖЕШЬ ХОДИТЬ ВЛЕВО-ВПРАВО И СТРЕЛЯТЬ ВВЕРХ. ЭТО ОСНОВА АРКАДНОГО ШУТЕРА. ВРАГИ ДВИГАЮТСЯ ВОЛНАМИ: ЧЕМ МЕНЬШЕ ВРАГОВ, ТЕМ БЫСТРЕЕ ОНИ ДВИЖУТСЯ. ТАК СОЗДАЁТСЯ НАПРЯЖЕНИЕ. ТЫ ДОЛЖЕН ОДНОВРЕМЕННО УКЛОНЯТЬСЯ И АТАКОВАТЬ.",
            "КОГДА УНИЧТОЖИШЬ ВСЕХ ВРАГОВ — НАЧИНАЕТСЯ НОВАЯ ВОЛНА, И ОНА СЛОЖНЕЕ. ЭТО ПРОГРЕССИЯ СЛОЖНОСТИ. С КАЖДОЙ ВОЛНОЙ ВРАГИ УСКОРЯЮТСЯ. ТАК ИГРА НЕ ДАЁТ РАССЛАБИТЬСЯ И ТРЕНИРУЕТ ТВОЮ РЕАКЦИЮ. ЗАПОМНИ ЭТОТ ПРИЁМ — ОН ИСПОЛЬЗУЕТСЯ ВО МНОГИХ ЖАНРАХ.",
            "ЭТИ МЕХАНИКИ — ДВИЖЕНИЕ И СТРЕЛЬБА, ВОЛНЫ, УСКОРЕНИЕ — ИЗМЕНИЛИ ИГРОВУЮ ИНДУСТРИЮ. ОНИ ПЕРЕКОЧЕВАЛИ В ШУТЕРЫ, ЭКШЕНЫ, РОГЛАЙТЫ. ТЫ НЕ ПРОСТО ИГРАЕШЬ — ТЫ ИЗУЧАЕШЬ ИСТОРИЮ. ГОТОВ ПОКАЗАТЬ, ЧЕМУ НАУЧИЛСЯ?"
        };
        
        EncyclopediaManager.Instance.ShowSequentialMessages(messages, () => {
            PlayerPrefs.SetInt("Level2_TutorialShown", 1);
            PlayerPrefs.Save();
            StartGame();
        });
    }
    
    private void StartGame()
    {
        if (playerController != null) playerController.SetControlsEnabled(true);
        if (waveSpawner != null)
        {
            waveSpawner.enabled = true;
            waveSpawner.StartGame(); // вызываем метод старта волн
        }
        
        StartCoroutine(TimedHints());
    }
    
    private IEnumerator TimedHints()
    {
        yield return new WaitForSeconds(10f);
        EncyclopediaManager.Instance.ShowTimedMessage("СТРЕЛЯЙ ПО ВРАГАМ, НО НЕ ЗАСТЫВАЙ НА МЕСТЕ — ПОСТОЯННО ДВИГАЙСЯ, ЧТОБЫ УКЛОНЯТЬСЯ ОТ ПУЛЬ.", 5f);
        
        yield return new WaitForSeconds(20f);
        EncyclopediaManager.Instance.ShowTimedMessage("УНИЧТОЖАЙ ВРАГОВ БЫСТРЕЕ — С КАЖДОЙ ВОЛНОЙ ОНИ СТАНОВЯТСЯ ВСЁ БЫСТРЕЕ. ПРОГРЕССИЯ СЛОЖНОСТИ — ЭТО КЛЮЧЕВОЙ ПРИЁМ ГЕЙМДИЗАЙНА.", 5f);
        
        yield return new WaitForSeconds(30f);
        EncyclopediaManager.Instance.ShowTimedMessage("СКОРОСТЬ ВРАГОВ ПОСТОЯННО РАСТЁТ. ЭТО ТРЕНИРУЕТ ТВОЮ РЕАКЦИЮ. БЕЗ ЭТОЙ МЕХАНИКИ ИГРЫ БЫЛИ БЫ СЛИШКОМ ЛЁГКИМИ.", 5f);
    }
    
    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        
        int levelBonus = 1000;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(levelBonus);
            GameManager.Instance.CompleteLevel(levelIndex, levelBonus);
        }
        
        Debug.Log("Level 2 completed! Score added: " + levelBonus);
        
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
            gameOverManager.ShowLevelComplete();
        else
            Debug.LogWarning("GameOverManager not found");
    }
}