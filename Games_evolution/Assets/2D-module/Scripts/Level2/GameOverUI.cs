using UnityEngine;
using UnityEngine.UI;

 
public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button skipButton;

        private Level2Manager levelManager;

        private void Start()
        {
            levelManager = FindObjectOfType<Level2Manager>();
            gameOverPanel.SetActive(false);
            restartButton.onClick.AddListener(OnRestart);
            skipButton.onClick.AddListener(OnSkip);
        }

        public void Show()
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // пауза
        }

        void OnRestart()
        {
            Time.timeScale = 1f;
            levelManager.RestartLevel();
        }

        void OnSkip()
        {
            Time.timeScale = 1f;
            levelManager.SkipLevel();
        }
    }
 