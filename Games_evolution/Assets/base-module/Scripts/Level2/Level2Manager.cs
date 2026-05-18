using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public Article levelData;          // перетащить Level2_Data
    public WaveSpawner waveSpawner;
    public PlayerController playerController;

    void Start()
    {
        if (playerController != null) playerController.SetControlsEnabled(false);
        if (waveSpawner != null) waveSpawner.enabled = false;

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
        if (levelData == null || levelData.tutorialMessages.Count == 0)
        {
            StartGame();
            return;
        }
        UnifiedInfoSystem.Instance.ShowSequentialMessages(levelData.tutorialMessages, () =>
        {
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
            waveSpawner.StartGame(); // предполагается, что в WaveSpawner есть этот метод
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
        // Действия при завершении уровня (например, загрузить следующий)
        SceneManager.LoadScene("3_Level3"); // название сцены уровня 3
    }
}