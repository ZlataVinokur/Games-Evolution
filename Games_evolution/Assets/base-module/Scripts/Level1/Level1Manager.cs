using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public BallController ball;
    public PaddleController paddle;

    void Start()
    {
       
        if (paddle != null) paddle.SetControlsEnabled(false);
        if (ball != null) ball.SetGameStarted(false);

    
            ShowTutorial();
        
    }

    private void ShowTutorial()
    {
        List<string> messages = new List<string>
        {
            "ПИКСЕЛЬ, ТЫ ТОЛЬКО НАЧИНАЕШЬ СВОЙ ПУТЬ. ПОСМОТРИ: ЗДЕСЬ ЕСТЬ ПЛАТФОРМА И МЯЧ. ТВОЯ ЗАДАЧА — ОТБИВАТЬ МЯЧ ТАК, ЧТОБЫ ОН РАЗРУШАЛ БЛОКИ. ЭТО ПРОСТЕЙШАЯ МЕХАНИКА: ДВИЖЕНИЕ, ОТСКОК, ЦЕЛЬ. ИМЕННО С ТАКИХ ИГР НАЧИНАЛАСЬ ИСТОРИЯ ВИДЕОИГР.",
            "УГОЛ ОТСКОКА ЗАВИСИТ ОТ ТОГО, КУДА МЯЧ ПОПАДЁТ ПО ПЛАТФОРМЕ. В ЦЕНТР — ЛЕТИТ ПРЯМО, В КРАЙ — КРУЧЕ. ЭТО ДОБАВЛЯЕТ МАСТЕРСТВО. ИГРОК УЧИТСЯ ПРЕДСКАЗЫВАТЬ ТРАЕКТОРИЮ. ТАК РОЖДАЕТСЯ ГЛУБИНА ИЗ ПРОСТОТЫ.",
            "НЕКОТОРЫЕ БЛОКИ ПРОЧНЕЕ — ИХ НУЖНО БИТЬ НЕСКОЛЬКО РАЗ. А ЕСЛИ ПОВЕЗЁТ, ИЗ БЛОКА ВЫПАДЕТ БОНУС. ЛОВИ ЕГО ПЛАТФОРМОЙ — ОН МОЖЕТ УВЕЛИЧИТЬ ПЛАТФОРМУ ИЛИ ЗАМЕДЛИТЬ МЯЧ. ЭТО ПЕРВЫЕ В ИСТОРИИ «УЛУЧШЕНИЯ» В ИГРАХ. ЗАПОМНИ ЭТОТ МОМЕНТ, ПИКСЕЛЬ.",
            "ТЫ СПРАВИШЬСЯ, ЕСЛИ БУДЕШЬ ВНИМАТЕЛЕН. ЭТА МЕХАНИКА — ДВИЖЕНИЕ И ОТСКОК — ЛЕЖИТ В ОСНОВЕ МНОГИХ СЛОЖНЫХ ИГР. ПОТОМ ТЫ ВСТРЕТИШЬ СТРЕЛЬБУ, ЛАБИРИНТЫ, ГОЛОВОЛОМКИ… НО ВСЁ НАЧИНАЕТСЯ С ПРОСТОГО УДАРА МЯЧА. ВПЕРЁД!"
        };

        EncyclopediaManager.Instance.ShowSequentialMessages(messages, () => {
            PlayerPrefs.SetInt("Level1_TutorialShown", 1);
            PlayerPrefs.Save();
            StartGame();
        });
    }

    private void StartGame()
    {
        if (paddle != null) paddle.SetControlsEnabled(true);
        if (ball != null) ball.SetGameStarted(true);

        StartCoroutine(TimedHints());
    }

    private IEnumerator TimedHints()
    {
        yield return new WaitForSeconds(10f);
        EncyclopediaManager.Instance.ShowTimedMessage("ПОДСКАЗКА: УГОЛ ОТСКОКА ЗАВИСИТ ОТ МЕСТА ПОПАДАНИЯ ПО ПЛАТФОРМЕ. ПОПРОБУЙ НАПРАВИТЬ МЯЧ В НУЖНУЮ СТОРОНУ!", 5f);
        
        yield return new WaitForSeconds(15f);
        EncyclopediaManager.Instance.ShowTimedMessage("БОНУСЫ ВЫПАДАЮТ ИЗ НЕКОТОРЫХ БЛОКОВ. ЛОВИ ИХ ПЛАТФОРМОЙ!", 5f);
        
        yield return new WaitForSeconds(15f);
        EncyclopediaManager.Instance.ShowTimedMessage("ЧЕМ БОЛЬШЕ БЛОКОВ РАЗРУШИШЬ, ТЕМ БЛИЖЕ ПОБЕДА. УДАЧИ, ПИКСЕЛЬ!", 5f);
    }
}