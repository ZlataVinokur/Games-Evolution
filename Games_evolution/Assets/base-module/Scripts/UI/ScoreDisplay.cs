using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScore();
    }
    
    void Update()
    {
        UpdateScore();
    }
    
    void UpdateScore()
    {
        if (GameManager.Instance != null && scoreText != null)
        {
            scoreText.text = "ОЧКИ: " + GameManager.Instance.totalScore;
        }
    }
}