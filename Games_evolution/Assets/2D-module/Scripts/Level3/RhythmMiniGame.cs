using UnityEngine;

public class RhythmMiniGame : MonoBehaviour
{
    public float beatInterval = 1f;
    public int successesNeeded = 5;
    public int maxMistakes = 3;

    private float nextBeatTime;
    private bool isActive = false;
    private int successes = 0;
    private int mistakes = 0;
    private bool isCompleted = false;
    private SpriteRenderer visualHint;

    void Start() => visualHint = GetComponent<SpriteRenderer>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCompleted || !other.CompareTag("Player")) return;
        if (RPGLevelManager.Instance != null && RPGLevelManager.Instance.metersActivated[0]) return;

        isActive = true;
        nextBeatTime = Time.time + beatInterval;
        if (visualHint != null) visualHint.color = Color.cyan;
        UnifiedInfoSystem.Instance?.ShowTimedMessage("РИТМ-ИГРА: Нажимай ПРОБЕЛ в такт мигающему кругу!", 2f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive && !isCompleted)
        {
            isActive = false;
            if (visualHint != null) visualHint.color = Color.gray;
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Ты вышел из зоны ритм-игры. Прогресс сброшен.", 1f);
            successes = 0;
            mistakes = 0;
        }
    }

    void Update()
    {
        if (!isActive || isCompleted) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float diff = Mathf.Abs(Time.time - nextBeatTime);
            if (diff < 0.25f)
            {
                successes++;
                UnifiedInfoSystem.Instance?.ShowTimedMessage($"Успех! {successes}/{successesNeeded}", 0.5f);
                if (successes >= successesNeeded) Win();
                else nextBeatTime = Time.time + beatInterval;
            }
            else
            {
                mistakes++;
                UnifiedInfoSystem.Instance?.ShowTimedMessage($"Мимо! Ошибок: {mistakes}/{maxMistakes}", 1f);
                if (mistakes >= maxMistakes) Lose();
                else nextBeatTime = Time.time + beatInterval;
            }
        }
        if (visualHint != null && Time.time >= nextBeatTime - 0.1f && Time.time < nextBeatTime + 0.1f)
            visualHint.color = Color.red;
        else if (visualHint != null && isActive) visualHint.color = Color.cyan;
    }

    void Win()
    {
        isCompleted = true;
        isActive = false;
        if (visualHint != null) visualHint.color = Color.green;
        RPGLevelManager.Instance.ActivateMeter(0);
        UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Отличный ритм! Красный измеритель активирован." }, "encyclopedia", "happy");
        Destroy(gameObject, 1f);
    }

    void Lose()
    {
        successes = 0;
        mistakes = 0;
        UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Не получилось. Попробуй ещё раз, нажимай строго в такт!" }, "encyclopedia", "neutral");
        nextBeatTime = Time.time + beatInterval;
    }
}