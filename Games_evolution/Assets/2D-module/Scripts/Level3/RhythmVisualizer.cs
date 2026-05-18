using UnityEngine;
using UnityEngine.UI;

public class RhythmVisualizer : MonoBehaviour
{
    public Image beatImage;
    public AudioSource metronomeSound;
    public float beatInterval = 1f;
    private float nextBeatTime;
    private bool isActive = false;
    private RhythmMiniGame miniGame;

    void Start()
    {
        miniGame = FindObjectOfType<RhythmMiniGame>();
        if (miniGame != null) beatInterval = miniGame.beatInterval;
        nextBeatTime = Time.time + beatInterval;
        beatImage.color = Color.white;
    }

    void Update()
    {
        if (miniGame == null || !miniGame.IsActive) return;

        // Анимация приближения удара
        float timeToBeat = nextBeatTime - Time.time;
        if (timeToBeat > 0)
        {
            float scale = Mathf.Lerp(0.5f, 1.5f, (beatInterval - timeToBeat) / beatInterval);
            beatImage.rectTransform.localScale = Vector3.one * scale;
            beatImage.color = Color.Lerp(Color.white, Color.green, 1 - timeToBeat / beatInterval);
        }
        else
        {
            // Удар! Звук и сброс анимации
            if (timeToBeat <= 0 && timeToBeat > -0.05f)
            {
                metronomeSound.Play();
                beatImage.rectTransform.localScale = Vector3.one * 1.5f;
                beatImage.color = Color.red;
                nextBeatTime = Time.time + beatInterval;
            }
        }
    }
}