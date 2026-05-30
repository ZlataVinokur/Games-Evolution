using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RPGLevelManager : MonoBehaviour
{
    public static RPGLevelManager Instance;

    [Header("Meters")]
    public bool[] metersActivated = new bool[3];
    public GameObject[] meterIndicatorUI;

    private UnifiedInfoSystem infoSystem;
    private GameManager gameManager;
    private bool levelCompleted = false;

    void Awake() => Instance = this;

    void Start()
    {
        // Восстанавливаем состояние из RPGProgress
        for (int i = 0; i < 3; i++)
            if (RPGProgress.IsMeterActivated(i) && !metersActivated[i])
            {
                metersActivated[i] = true;
                UpdateMeterUI();
            }

        infoSystem = UnifiedInfoSystem.Instance ?? FindFirstObjectByType<UnifiedInfoSystem>();
        gameManager = GameManager.Instance ?? FindFirstObjectByType<GameManager>();
        UpdateMeterUI();

        // Приветственный диалог с заданием
        infoSystem?.ShowDialogue(new[] {
            "Твоя задача – активировать три измерителя.",
            "1. Пройди ритм-игру (мигающий круг).",
            "2. Найди провод, чип, батарею и положи на алтари в правильном порядке.",
            "3. Активируй третий измеритель – собери 5 кристаллов энергии или убей 5 багов.",
            "Когда все три загорятся, уровень завершится."
        }, "encyclopedia", "serious");
    }

    public void ActivateMeter(int index)
    {
        if (index < 0 || index >= metersActivated.Length) return;
        if (metersActivated[index]) return;

        metersActivated[index] = true;
        RPGProgress.ActivateMeter(index);
        UpdateMeterUI();
        Debug.Log($"Meter {index} activated. State: [{metersActivated[0]},{metersActivated[1]},{metersActivated[2]}]");

        string[] articleIds = { "rpg_rhythm", "rpg_logic", "rpg_moral" };
        infoSystem?.UnlockArticle(articleIds[index]);

        // Проверяем, все ли активированы
        if (metersActivated[0] && metersActivated[1] && metersActivated[2])
        {
            Debug.Log("All meters activated! Completing level...");
            CompleteLevel();
        }
    }

    void UpdateMeterUI()
    {
        for (int i = 0; i < meterIndicatorUI.Length; i++)
        {
            if (meterIndicatorUI[i] != null)
            {
                Image img = meterIndicatorUI[i].GetComponent<Image>();
                if (img != null)
                    img.color = metersActivated[i] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Принудительный вызов CompleteLevel() по клавише L");
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        Debug.Log("CompleteLevel: запускаем загрузку квиза");

        // Сохраняем прогресс в GameManager (если он есть)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel("RPG_Module");
            GameManager.Instance.SetFlag("RPG_Completed", true);
            // Загружаем сцену квиза через GameManager
            GameManager.Instance.LoadQuizForCurrentModule(1);
        }

    }
}
