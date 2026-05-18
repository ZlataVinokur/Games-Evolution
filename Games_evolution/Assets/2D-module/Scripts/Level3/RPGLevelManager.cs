using UnityEngine;
using UnityEngine.SceneManagement;

public class RPGLevelManager : MonoBehaviour
{
    public static RPGLevelManager Instance;

    [Header("Meters")]
    public bool[] metersActivated = new bool[3];
    public GameObject[] meterIndicatorUI;

    [Header("Boss Fight")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject barrierWall;

    private bool hasTamagotchiBuff = false;
    private UnifiedInfoSystem infoSystem;
    private GameManager gameManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Восстанавливаем состояние из GameManager
        bool[] savedMeters = GameManager.Instance.GetRPGMeters();
        if (savedMeters != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (savedMeters[i] && !metersActivated[i])
                {
                    metersActivated[i] = true;
                    UpdateMeterUI();
                    // Если нужно восстановить артефакты (алтари и т.д.) – дополнительно
                }
            }
        }

        infoSystem = UnifiedInfoSystem.Instance ?? FindObjectOfType<UnifiedInfoSystem>();
        gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
        UpdateMeterUI();
        if (barrierWall != null) barrierWall.SetActive(true);
    }

    public void ActivateMeter(int index)
    {
        if (metersActivated[index]) return;
        metersActivated[index] = true;
        UpdateMeterUI();
        GameManager.Instance.SetRPGMeter(index, true); // сохраняем

        if (index < 0 || index >= metersActivated.Length) return;
        if (metersActivated[index]) return;

        metersActivated[index] = true;
        UpdateMeterUI();

        string[] articleIds = { "rpg_rhythm", "rpg_logic", "rpg_moral" };
        if (infoSystem != null) infoSystem.UnlockArticle(articleIds[index]);

        foreach (bool activated in metersActivated)
            if (!activated) return;

        StartBossFight();
    }

    void UpdateMeterUI()
    {
        for (int i = 0; i < meterIndicatorUI.Length; i++)
            if (meterIndicatorUI[i] != null)
                meterIndicatorUI[i].SetActive(metersActivated[i]);
    }

    void StartBossFight()
    {
        if (barrierWall != null) barrierWall.SetActive(false);
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        if (infoSystem != null)
            infoSystem.ShowDialogue(new string[] { "Портал открыт! Победи мини-босса, чтобы завершить модуль." }, "encyclopedia", "serious");
    }

    public void SetTamagotchiBuff(bool value)
    {
        hasTamagotchiBuff = value;
        if (value && infoSystem != null)
            infoSystem.TellFact(new string[] { "Тамагочи благодарен! В бою он поможет тебе наносить больше урона." }, "happy");
    }

    public bool HasTamagotchiBuff() => hasTamagotchiBuff;

    public void OnBossDefeated()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel("RPG_Module");
            GameManager.Instance.SetFlag("RPG_Completed", true);
        }
        UnifiedInfoSystem.Instance.ShowDialogue(
            new string[] { "Поздравляю! Ты освоил эволюцию 2D-жанров. Теперь тебя ждёт итоговый квиз." },
            "encyclopedia", "celebrate", () =>
            {
                GameManager.Instance.LoadQuizForCurrentModule(SceneManager.GetActiveScene().buildIndex);
            }
        );
    }
}