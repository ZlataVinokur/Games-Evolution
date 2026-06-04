using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level3Manager : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    [SerializeField] private PlayerController3 playerController;
    [SerializeField] private GameController gameController;

    void Start()
    {
        // Блокируем управление
        if (playerController != null) playerController.SetControlsEnabled(false);

        // Показываем обучение, если ещё не показывали
        if (PlayerPrefs.GetInt("Level3_TutorialShown", 0) == 0)
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
            "ПИКСЕЛЬ, ТЕПЕРЬ ТЫ ПОПАЛ В ЛАБИРИНТ. ЦЕЛЬ — СОБРАТЬ ВСЕ ТОЧКИ, ИЗБЕГАЯ ПРИВИДЕНИЙ. ЭТО МЕХАНИКА СБОРА ПРЕДМЕТОВ И НАВИГАЦИИ. ПРИВИДЕНИЯ ПРЕСЛЕДУЮТ ТЕБЯ ПО-РАЗНОМУ: КТО-ТО ПРЯМО, КТО-ТО УСТРАИВАЕТ ЗАСАДУ. УЧИСЬ ИХ ЧИТАТЬ!",
            "В УГЛАХ ЛАБИРИНТА ЕСТЬ СУПЕР-ТОЧКИ. СЪЕВ ИХ, ТЫ СМОЖЕШЬ ЕСТЬ ПРИВИДЕНИЙ НА КОРОТКОЕ ВРЕМЯ. ЗА КАЖДОЕ СЪЕДЕННОЕ ПРИВИДЕНИЕ ДАЁТСЯ ВСЁ БОЛЬШЕ ОЧКОВ. ЭТО МЕХАНИКА «НАГРАДА ЗА РИСК».",
            "ТАКЖЕ ПО БОКАМ ЕСТЬ ТУННЕЛИ, КОТОРЫЕ ТЕЛЕПОРТИРУЮТ ТЕБЯ НА ДРУГУЮ СТОРОНУ. ЭТО ПОМОГАЕТ СПАСТИСЬ ОТ ПОГОНИ. ЗАПОМНИ: ПРИВИДЕНИЯ НЕ ДУМАЮТ, А ДЕЙСТВУЮТ ПО ШАБЛОНУ. НАУЧИСЬ ИХ ОБМАНЫВАТЬ!",
            "ЭТИ МЕХАНИКИ — СБОР ПРЕДМЕТОВ, РАЗНОЕ ПОВЕДЕНИЕ ВРАГОВ, ТУННЕЛИ — ИЗМЕНИЛИ ПРЕДСТАВЛЕНИЕ О ТОМ, КАКИМИ МОГУТ БЫТЬ ИГРЫ. ОНИ ПЕРЕКОЧЕВАЛИ В КВЕСТЫ, RPG, ОТКРЫТЫЕ МИРЫ. ТЫ НЕ ПРОСТО ИГРАЕШЬ — ТЫ ИЗУЧАЕШЬ ИСТОРИЮ. ГОТОВ ПОКАЗАТЬ, ЧЕМУ НАУЧИЛСЯ?"
        };

        // Прямой вызов UnifiedInfoSystem
        if (UnifiedInfoSystem.Instance != null)
        {
            UnifiedInfoSystem.Instance.ShowSequentialMessages(messages, () => {
                PlayerPrefs.SetInt("Level3_TutorialShown", 1);
                PlayerPrefs.Save();
                StartGame();
            });
        }
        else
        {
            Debug.LogError("UnifiedInfoSystem.Instance не найден! Обучение не будет показано.");
            // fallback
            StartGame();
        }
    }

    private void StartGame()
    {
        if (playerController != null) playerController.SetControlsEnabled(true);
        if (gameController != null) gameController.StartGame();

        StartCoroutine(TimedHints());
    }

    private IEnumerator TimedHints()
    {
        yield return new WaitForSeconds(10f);
        if (UnifiedInfoSystem.Instance != null)
            UnifiedInfoSystem.Instance.ShowTimedMessage("СУПЕР-ТОЧКИ ДЕЛАЮТ ПРИВИДЕНИЙ УЯЗВИМЫМИ. ЕШЬ ИХ В ЭТОМ РЕЖИМЕ — ПОЛУЧИШЬ МНОГО ОЧКОВ!", 5f);

        yield return new WaitForSeconds(20f);
        if (UnifiedInfoSystem.Instance != null)
            UnifiedInfoSystem.Instance.ShowTimedMessage("ТУННЕЛИ ПО БОКАМ ЛАБИРИНТА ПОМОГАЮТ БЫСТРО СМЕНИТЬ ПОЗИЦИЮ, ЕСЛИ ПРИВИДЕНИЯ ЗАЖАЛИ В УГЛУ.", 5f);

        yield return new WaitForSeconds(25f);
        if (UnifiedInfoSystem.Instance != null)
            UnifiedInfoSystem.Instance.ShowTimedMessage("У КАЖДОГО ПРИВИДЕНИЯ СВОЁ ПОВЕДЕНИЕ. НАБЛЮДАЙ — ЭТО ПОМОЖЕТ ИЗБЕГАТЬ ВСТРЕЧ.", 5f);
    }
}