using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public Article levelData; // перетащить Level1_Data
    public BallController ball;
    public PaddleController paddle;

    void Start()
    {
        // Блокируем управление
        if (paddle != null) paddle.SetControlsEnabled(false);
        if (ball != null) ball.SetGameStarted(false);

        if (PlayerPrefs.GetInt("Level1_TutorialShown", 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            StartGame();
        }
        if (GameManager.Instance != null)
            GameManager.Instance.ResetLevelBrickCounter();
            GameManager.Instance.ResetCurrentLevelScore();
    }

    private void ShowTutorial()
    {
        if (levelData == null || levelData.tutorialMessages.Count == 0)
        {
            StartGame();
            return;
        }
        UnifiedInfoSystem.Instance.ShowSequentialMessages(levelData.tutorialMessages, () =>
        {
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
        if (levelData == null) yield break;
        foreach (var hint in levelData.timedHints)
        {
            yield return new WaitForSeconds(hint.delayAfterStart);
            UnifiedInfoSystem.Instance.ShowTimedMessage(hint.message, hint.duration);
        }
    }

    // Вызови этот метод при победе (например, когда разрушены все блоки)
    public void CompleteLevel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ShowWin();
    }
}