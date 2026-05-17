using UnityEngine;

public class RPGLevelManager : MonoBehaviour
{
    public static RPGLevelManager Instance;

    [Header("Meters")]
    public bool[] metersActivated = new bool[3]; // 0-ритм, 1-логика, 2-мораль
    public GameObject[] meterIndicatorUI; // иконки на UI

    [Header("Boss Fight")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject barrierWall; // стена, блокирующая выход до активации

    [Header("Moral Choice Buff")]
    private bool hasTamagotchiBuff = false;

    [Header("References")]
    public UnifiedInfoSystem infoSystem;
    public GameManager gameManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        infoSystem = UnifiedInfoSystem.Instance;
        gameManager = GameManager.Instance;
        UpdateMeterUI();
        if (barrierWall != null) barrierWall.SetActive(true);
    }

    public void ActivateMeter(int index)
    {
        if (index < 0 || index >= metersActivated.Length) return;
        if (metersActivated[index]) return;

        metersActivated[index] = true;
        UpdateMeterUI();

        // Открываем статьи в справочнике через UnifiedInfoSystem
        string[] articleIds = { "rpg_rhythm", "rpg_logic", "rpg_moral" };
        infoSystem.UnlockArticle(articleIds[index]);

        // Проверяем, все ли активированы
        foreach (bool activated in metersActivated)
        {
            if (!activated) return;
        }
        // Все три активированы → запускаем бой
        StartBossFight();
    }

    void UpdateMeterUI()
    {
        for (int i = 0; i < meterIndicatorUI.Length; i++)
        {
            if (meterIndicatorUI[i] != null)
                meterIndicatorUI[i].SetActive(metersActivated[i]);
        }
    }

    void StartBossFight()
    {
        if (barrierWall != null) barrierWall.SetActive(false); // открываем проход
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        infoSystem.ShowDialogue(new string[] { "Портал открыт! Победи мини-босса, чтобы завершить модуль." }, "encyclopedia", "serious");
    }

    public void SetTamagotchiBuff(bool value)
    {
        hasTamagotchiBuff = value;
        if (value)
            infoSystem.TellFact(new string[] { "Тамагочи благодарен! В бою он поможет тебе наносить больше урона." }, "happy");
    }

    public bool HasTamagotchiBuff() => hasTamagotchiBuff;

    // Вызывается после победы над боссом
    public void OnBossDefeated()
    {
        gameManager.CompleteLevel("RPG_Module");
        gameManager.SetFlag("RPG_Completed", true);
        infoSystem.ShowDialogue(new string[] { "Поздравляю! Ты освоил эволюцию 2D-жанров. Теперь тебя ждёт итоговый квиз." }, "encyclopedia", "celebrate", () =>
        {
            // Запускаем квиз через QuizManager (допустим, он есть)
            //QuizManager.Instance.StartQuiz(2); // 2 = модуль двумерный
        });
    }
}