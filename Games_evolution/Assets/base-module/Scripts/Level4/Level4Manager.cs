using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4Manager : MonoBehaviour
{
    public Article levelData;          // перетащить Level4_Data
    public TetrisGameManager tetrisGameManager;

    void Start()
    {
        if (tetrisGameManager != null) tetrisGameManager.SetControlsEnabled(false);

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
        if (levelData == null || levelData.tutorialMessages.Count == 0)
        {
            StartGame();
            return;
        }
        UnifiedInfoSystem.Instance.ShowSequentialMessages(levelData.tutorialMessages, () =>
        {
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
            tetrisGameManager.StartGame(); // запускает игру (спавн фигур)
        }
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
        // Здесь запускается квиз (как у вас уже реализовано)
        // Если квиз уже вызывается из TetrisGameManager, то здесь ничего не делаем
        // Или можно вызвать квиз напрямую.
    }
}