using UnityEngine;

public class LivesDisplay : MonoBehaviour
{
    private int currentLives = 3;
    [SerializeField] private LivesUI livesUI;

    void Start()
    {
        if (livesUI == null)
            livesUI = FindObjectOfType<LivesUI>();
        UpdateLivesDisplay();
    }

    public void LoseLife()
    {
        int heartToAnimate = currentLives - 1;
        currentLives--;
        UpdateLivesDisplay();
        if (livesUI != null && heartToAnimate >= 0)
            livesUI.AnimateEmptyHeart(heartToAnimate);
        if (currentLives <= 0)
            GameOver();
    }

    public void AddLife()
    {
        if (currentLives < 3)
        {
            currentLives++;
            UpdateLivesDisplay();
        }
    }

    public int GetCurrentLives() => currentLives;

    void UpdateLivesDisplay()
    {
        if (livesUI != null)
            livesUI.UpdateLives(currentLives);
    }

    void GameOver() 
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.ShowGameOver();
    }
}