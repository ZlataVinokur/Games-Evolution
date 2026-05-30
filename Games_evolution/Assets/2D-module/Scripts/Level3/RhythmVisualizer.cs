using UnityEngine;
using UnityEngine.UI;

public class RhythmVisualizer : MonoBehaviour
{
    public Image beatImage;
    public AudioSource metronomeSound;
    private RhythmMiniGame miniGame;
    private float beatInterval = 1f;
    private float nextBeatTime;
    private bool visualizerActive = false;

    void Start()
    {
        miniGame = FindFirstObjectByType<RhythmMiniGame>();
        if (miniGame != null)
            beatInterval = miniGame.beatInterval;
        nextBeatTime = Time.time + beatInterval;
        if (beatImage != null)
            beatImage.color = Color.white;
    }

    void Update()
    {
        // Не скрываем объект, просто проверяем активность
        if (miniGame == null || !miniGame.IsActive)
        {
            // Можно сделать изображение полупрозрачным или просто ничего не делать
            if (beatImage != null && beatImage.color != Color.gray)
                beatImage.color = Color.gray;
            return;
        }

        // Если мини-игра активна – анимируем
        if (beatImage != null && beatImage.color == Color.gray)
            beatImage.color = Color.white;

        float timeToBeat = nextBeatTime - Time.time;
        if (timeToBeat > 0)
        {
            float scale = Mathf.Lerp(0.8f, 1.5f, (beatInterval - timeToBeat) / beatInterval);
            beatImage.rectTransform.localScale = Vector3.one * scale;
            beatImage.color = Color.Lerp(Color.white, Color.green, 1 - timeToBeat / beatInterval);
        }
        else if (timeToBeat <= 0 && timeToBeat > -0.05f)
        {
            if (metronomeSound != null && !metronomeSound.isPlaying)
                metronomeSound.Play();
            beatImage.rectTransform.localScale = Vector3.one * 1.5f;
            beatImage.color = Color.red;
            nextBeatTime = Time.time + beatInterval;
        }
        else if (timeToBeat <= -0.05f)
        {
            nextBeatTime = Time.time + beatInterval;
        }
    }
}