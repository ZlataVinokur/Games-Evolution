using UnityEngine;

public class Portal : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (PlatformerController.Instance.EnemiesKilled >= 5 && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            // Открываем статью
            GameManager.Instance.UnlockArticle("platformer_portal");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CompleteLevel("Level2_Platformer_2");
            SceneLoader.LoadScene("Level3_RPG");
        }
    }
}