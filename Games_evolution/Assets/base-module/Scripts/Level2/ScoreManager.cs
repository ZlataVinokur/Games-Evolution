using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [SerializeField] private TMP_Text scoreText;
    private int currentScore = 0;
    
    void Awake()
    {
        // Простой синглтон для этого уровня
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        UpdateUI();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
        Debug.Log($"Score: +{points}, Total: {currentScore}");
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "ОЧКИ: " + currentScore;
    }
    
    public int GetCurrentScore() => currentScore;
}