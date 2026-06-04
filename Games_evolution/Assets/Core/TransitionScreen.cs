using UnityEngine;
using TMPro;

public class TransitionScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            if (scoreText != null)
            {
                scoreText.text = "Total Score: " + GameManager.Instance.totalScore;
            }
        }
        
        // Можно добавить разные сообщения в зависимости от уровня
        if (levelCompleteText != null)
        {
            levelCompleteText.text = "Level 1 Complete!";
        }
    }
}
