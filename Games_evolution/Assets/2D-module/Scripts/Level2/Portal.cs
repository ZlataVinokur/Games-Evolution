using UnityEngine;


public class Portal : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "Level3_RPG";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Завершить уровень
                GameManager.Instance.CompleteLevel("Level2");
                SceneLoader.LoadScene(nextSceneName); // предполагается использование SceneLoader
            }
        }
    }
