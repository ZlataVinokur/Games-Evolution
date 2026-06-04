using UnityEngine;
using UnityEngine.SceneManagement;

public class RPGLevelManager : MonoBehaviour
{
    public static RPGLevelManager Instance;

    public GameObject[] meterIndicatorUI;
    public bool[] metersActivated = new bool[3];

    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject winPortalPrefab;

    private UnifiedInfoSystem infoSystem;
    private GameManager gameManager;
    private bool levelCompleted = false;
    private bool bossDefeated = false;

    void Awake() => Instance = this;

    void Start()
    {
        for (int i = 0; i < 3; i++)
            if (RPGProgress.IsMeterActivated(i) && !metersActivated[i])
            {
                metersActivated[i] = true;
                UpdateMeterUI();
            }

        infoSystem = UnifiedInfoSystem.Instance ?? FindFirstObjectByType<UnifiedInfoSystem>();
        gameManager = GameManager.Instance ?? FindFirstObjectByType<GameManager>();
        UpdateMeterUI();

        // Приостанавливаем игру
        Time.timeScale = 0f;

        // Показываем диалог с колбэком на возобновление
        if (infoSystem != null)
        {
            infoSystem.ShowDialogue(
                new[] {
                "Твоя задача – активировать три измерителя.",
                "1. Пройди ритм-игру (мигающий круг).",
                "2. Найди провод, чип, батарею и положи на алтари В ПРАВИЛЬНОМ ПОРЯДКЕ (провод → чип → батарея).",
                "3. Найди лейку и полей 5 грибочков, чтобы накопить энергию."
                },
                "encyclopedia",
                "serious",
                onComplete: () => {
                    Time.timeScale = 1f; // возобновляем игру
                }
            );
        }
        else
        {
            // Если нет InfoSystem, всё равно надо разморозить, иначе игра навсегда виснет
            Time.timeScale = 1f;
        }
    }

    public void ActivateMeter(int index)
    {
        if (index < 0 || index >= metersActivated.Length) return;
        if (metersActivated[index]) return;

        metersActivated[index] = true;
        RPGProgress.ActivateMeter(index);
        UpdateMeterUI();
        Debug.Log($"Meter {index} activated.");

        string[] articleIds = { "rpg_rhythm", "rpg_logic", "rpg_energy" };
        infoSystem?.UnlockArticle(articleIds[index]);

        ApplyRPGUpgrade(index);

        if (metersActivated[0] && metersActivated[1] && metersActivated[2])
        {
            SpawnBoss();
        }
    }

    private void ApplyRPGUpgrade(int index)
    {
        var player = FindFirstObjectByType<IsometricPlayerController>();
        if (player == null) return;

        switch (index)
        {
            case 0:
                player.moveSpeed += 1f;
                infoSystem?.ShowTimedMessage("Бонус: скорость увеличена!", 2f);
                break;
            case 1:
                player.damageBonus += 5;
                infoSystem?.ShowTimedMessage("Бонус: урон увеличен!", 2f);
                break;
            case 2:
                player.maxHealth += 20;
                player.HealFull();
                infoSystem?.ShowTimedMessage("Бонус: здоровье увеличено и восстановлено!", 2f);
                break;
        }
    }

    void UpdateMeterUI()
    {
        for (int i = 0; i < meterIndicatorUI.Length; i++)
        {
            if (meterIndicatorUI[i] != null)
            {
                var img = meterIndicatorUI[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = metersActivated[i] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
    }

    void SpawnBoss()
    {
        if (levelCompleted || bossDefeated) return;
        levelCompleted = true;

        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            infoSystem?.ShowDialogue(new[] { "Ты активировал все измерители! Теперь сразись с финальным стражем." }, "encyclopedia", "serious");
        }
        else
        {
            Debug.LogError("BossPrefab или BossSpawnPoint не назначены!");
            CompleteLevel();
        }
    }

    public void OnBossDefeated()
    {
        if (bossDefeated) return;
        bossDefeated = true;

        infoSystem?.ShowDialogue(new[] { "Страж повержен! Ты доказал свою силу. Портал открыт." }, "encyclopedia", "happy");

        if (winPortalPrefab != null)
        {
            Instantiate(winPortalPrefab, bossSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            CompleteLevel();
        }
    }

    public void CompleteLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel("RPG_Module");
            GameManager.Instance.SetFlag("RPG_Completed", true);
            GameManager.Instance.LoadQuizForCurrentModule(7);
        }
        else
        {
            SceneManager.LoadScene("Quiz");
        }
    }
}