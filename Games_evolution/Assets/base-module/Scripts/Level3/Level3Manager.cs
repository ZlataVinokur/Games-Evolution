using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    public Article levelData;          // перетащить Level3_Data
    public GameController gameController;   // скрипт, управляющий Pac-Man
    public PlayerController3 playerController; // скрипт управления персонажем

    void Start()
    {
        if (playerController != null) playerController.SetControlsEnabled(false);
        if (gameController != null) gameController.enabled = false; // блокируем логику игры

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
        if (levelData == null || levelData.tutorialMessages.Count == 0)
        {
            StartGame();
            return;
        }
        UnifiedInfoSystem.Instance.ShowSequentialMessages(levelData.tutorialMessages, () =>
        {
            PlayerPrefs.SetInt("Level3_TutorialShown", 1);
            PlayerPrefs.Save();
            StartGame();
        });
    }

    private void StartGame()
    {
        if (playerController != null) playerController.SetControlsEnabled(true);
        if (gameController != null) gameController.enabled = true;
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

    public void CompleteLevel()
    {
        SceneManager.LoadScene("4_Level4");
    }
}