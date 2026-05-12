using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level4Manager : MonoBehaviour
{
    [Header("Ссылки на компоненты Tetris")]
    [SerializeField] private TetrisGameManager tetrisGameManager; // изменён тип

    void Start()
    {
        if (tetrisGameManager != null)
            tetrisGameManager.SetControlsEnabled(false);

        if (PlayerPrefs.GetInt("Level4_TutorialShown", 0) == 0)
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
            "ПИКСЕЛЬ, ЭТО ОСОБЕННЫЙ УРОВЕНЬ. ЗДЕСЬ НЕТ ВРАГОВ, НЕТ ВРЕМЕНИ, НО ЕСТЬ ЧИСТОЕ ПРОСТРАНСТВЕННОЕ МЫШЛЕНИЕ. СВЕРХУ ПАДАЮТ ФИГУРЫ. ТЫ МОЖЕШЬ ИХ ВРАЩАТЬ, ДВИГАТЬ И УСКОРЯТЬ. ЗАДАЧА — ЗАПОЛНЯТЬ ГОРИЗОНТАЛЬНЫЕ РЯДЫ. ЗАПОЛНЕННЫЙ РЯД ИСЧЕЗАЕТ — ЭТО ТВОЯ ПОБЕДА.",
            "ЧЕМ БОЛЬШЕ РЯДОВ ЗА ОДИН РАЗ, ТЕМ БОЛЬШЕ ОЧКОВ. 4 РЯДА СРАЗУ — ГЛАВНЫЙ НАВЫК. СКОРОСТЬ ПАДЕНИЯ РАСТЁТ С ОЧКАМИ. ЭТО МЕХАНИКА НАРАСТАЮЩЕЙ СЛОЖНОСТИ. НУЖНО ВИДЕТЬ НА НЕСКОЛЬКО ХОДОВ ВПЕРЁД.",
            "ФИГУРЫ ПРИХОДЯТ В СЛУЧАЙНОМ ПОРЯДКЕ — КАЖДАЯ ПАРТИЯ УНИКАЛЬНА. ТЫ УЧИШЬСЯ АДАПТИРОВАТЬСЯ И БЫСТРО ПРИНИМАТЬ РЕШЕНИЯ. ЭТА МЕХАНИКА — ОСНОВА ВСЕХ СОВРЕМЕННЫХ ГОЛОВОЛОМОК.",
            "ЭТА ИГРА ИЗМЕНИЛА ПРЕДСТАВЛЕНИЕ О ТОМ, КАКОЙ МОЖЕТ БЫТЬ ИГРА: НИКАКОГО СЮЖЕТА, НИКАКИХ ГЕРОЕВ — ТОЛЬКО ФИГУРЫ, РЯДЫ И УСКОРЕНИЕ. ЭТО ЧИСТАЯ МЕХАНИКА. ДАВАЙ, ПИКСЕЛЬ, ПОКАЖИ, КАК ТЫ УМЕЕШЬ ДУМАТЬ!"
        };

        EncyclopediaManager.Instance.ShowSequentialMessages(messages, () => {
            PlayerPrefs.SetInt("Level4_TutorialShown", 1);
            PlayerPrefs.Save();
            StartGame();
        });
    }

    private void StartGame()
    {
        if (tetrisGameManager != null)
        {
            tetrisGameManager.SetControlsEnabled(true);
            tetrisGameManager.StartGame(); // ВАЖНО: запускаем игру (спавн фигуры)
        }

        StartCoroutine(TimedHints());
    }

    private IEnumerator TimedHints()
    {
        yield return new WaitForSeconds(15f);
        EncyclopediaManager.Instance.ShowTimedMessage("СТАРАЙСЯ УКЛАДЫВАТЬ ФИГУРЫ РОВНО, БЕЗ ПУСТОТ. ТАК ЛЕГЧЕ ЗАПОЛНЯТЬ РЯДЫ.", 5f);

        yield return new WaitForSeconds(25f);
        EncyclopediaManager.Instance.ShowTimedMessage("СМОТРИ НА СЛЕДУЮЩУЮ ФИГУРУ В МАЛЕНЬКОМ ОКНЕ. ЭТО ПОМОЖЕТ ПЛАНИРОВАТЬ НАПЕРЁД.", 5f);

        yield return new WaitForSeconds(35f);
        EncyclopediaManager.Instance.ShowTimedMessage("ПОВОРАЧИВАЙ ФИГУРЫ ЗАРАНЕЕ — НЕ ЖДИ, ПОКА ОНИ УПАДУТ НИЗКО. СКОРОСТЬ БУДЕТ РАСТИ!", 5f);
    }
}