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
    }

    private void ShowTutorial()
    {
        Debug.Log("ShowTutorial called");
        if (levelData == null) { Debug.LogError("levelData is null!"); return; }
        if (levelData == null || levelData.tutorialMessages.Count == 0)
        {
            Debug.LogError("tutorialMessages list is empty!"); 
            StartGame();
            return;
        }
        Debug.Log("tutorialMessages count: " + levelData.tutorialMessages.Count);
        UnifiedInfoSystem.Instance.ShowSequentialMessages(levelData.tutorialMessages, () =>
        {
            Debug.Log("Tutorial completed callback");
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
        // Загрузить следующий уровень (например, Level2)
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_Level2");
    }
}