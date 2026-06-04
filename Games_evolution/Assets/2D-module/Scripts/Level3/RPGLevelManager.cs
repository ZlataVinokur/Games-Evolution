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
    private bool levelCompleted = false;
    private bool bossDefeated = false;

    void Awake() => Instance = this;

    void Start()
    {
        for (int i = 0; i < 3; i++)
            if (RPGProgress.IsMeterActivated(i) && !metersActivated[i])
                metersActivated[i] = true;

        infoSystem = UnifiedInfoSystem.Instance ?? FindFirstObjectByType<UnifiedInfoSystem>();
        UpdateMeterUI();

        Time.timeScale = 0f;
        infoSystem?.ShowDialogue(
            new[] {
                "Твоя задача – активировать три измерителя.",
                "1. Пройди ритм-игру (мигающий круг).",
                "2. Найди кассету, хард-драйв и диск, положи в сундуки.",
                "3. Найди лейку и полей 5 грибочков."
            },
            "encyclopedia", "serious",
            onComplete: () => Time.timeScale = 1f
        );
    }

    public void ActivateMeter(int index)
    {
        if (index < 0 || index >= metersActivated.Length) return;
        if (metersActivated[index]) return;

        metersActivated[index] = true;
        RPGProgress.ActivateMeter(index);
        UpdateMeterUI();

        string[] articleIds = { "rpg_rhythm", "rpg_logic", "rpg_energy" };
        infoSystem?.UnlockArticle(articleIds[index]);
        ApplyRPGUpgrade(index);

        if (metersActivated[0] && metersActivated[1] && metersActivated[2])
            SpawnBoss();
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
                infoSystem?.ShowTimedMessage("Бонус: здоровье увеличено!", 2f);
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
            infoSystem?.ShowDialogue(new[] { "Ты активировал все измерители! Сразись с финальным стражем." }, "encyclopedia", "serious");
        }
        else CompleteLevel();
    }

    public void OnBossDefeated()
    {
        if (bossDefeated) return;
        bossDefeated = true;
        infoSystem?.ShowDialogue(new[] { "Страж повержен! Портал открыт." }, "encyclopedia", "happy");
        if (winPortalPrefab != null) Instantiate(winPortalPrefab, bossSpawnPoint.position, Quaternion.identity);
        else CompleteLevel();
    }

    public void CompleteLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel("RPG_Module");
            GameManager.Instance.SetFlag("RPG_Completed", true);
            GameManager.Instance.LoadQuizForCurrentModule(7);
        }
        else SceneManager.LoadScene("Quiz");
    }
}