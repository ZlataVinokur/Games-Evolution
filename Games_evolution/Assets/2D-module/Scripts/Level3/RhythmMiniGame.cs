using UnityEngine;

public class RhythmMiniGame : MonoBehaviour
{
    public float beatInterval = 1f;
    private float nextBeatTime;
    private bool isActive = false;
    public int successesNeeded = 5;       // количество успехов для победы
    private int successes = 0;
    public int maxMistakes = 3;           // разрешённое количество ошибок
    private int mistakes = 0;
    private bool isCompleted = false;     // чтобы не активировать дважды

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCompleted) return;
        if (other.CompareTag("Player") && RPGLevelManager.Instance != null && !RPGLevelManager.Instance.metersActivated[0])
        {
            isActive = true;
            nextBeatTime = Time.time + beatInterval;
            UnifiedInfoSystem.Instance?.ShowTimedMessage($"Нажимай ПРОБЕЛ в такт! Нужно {successesNeeded} успехов. Ошибок можно {maxMistakes}.", 2f);
        }
    }

    void Update()
    {
        if (!isActive || isCompleted) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float diff = Mathf.Abs(Time.time - nextBeatTime);
            if (diff < 0.2f)
            {
                successes++;
                UnifiedInfoSystem.Instance?.ShowTimedMessage($"Успех! {successes}/{successesNeeded}", 0.5f);

                if (successes >= successesNeeded)
                {
                    Win();
                }
                else
                {
                    // Готовим следующий такт
                    nextBeatTime = Time.time + beatInterval;
                }
            }
            else
            {
                mistakes++;
                UnifiedInfoSystem.Instance?.ShowTimedMessage($"Мимо! Ошибок: {mistakes}/{maxMistakes}", 1f);

                if (mistakes >= maxMistakes)
                {
                    Lose();
                }
                else
                {
                    // Не сбрасываем успехи, но сдвигаем следующий такт
                    nextBeatTime = Time.time + beatInterval;
                }
            }
        }
    }

    public bool IsActive => isActive && !isCompleted;

    private void Win()
    {
        isCompleted = true;
        isActive = false;
        RPGLevelManager.Instance.ActivateMeter(0); // активируем красный измеритель
        UnifiedInfoSystem.Instance?.ShowDialogue(
            new string[] { "Отличный ритм! Ты освоил механику ритм-игр. Это настоящий вызов для координации." },
            "encyclopedia", "happy");
        Destroy(gameObject);
    }

    private void Lose()
    {
        isActive = false;
        successes = 0;
        mistakes = 0;
        UnifiedInfoSystem.Instance?.ShowDialogue(
            new string[] { "Не получилось с первого раза. Ритм-игры требуют практики! Попробуй ещё раз." },
            "encyclopedia", "neutral", () =>
            {
                // Перезапускаем мини-игру: снова активируем триггер
                isActive = true;
                nextBeatTime = Time.time + beatInterval;
            });
        // Не уничтожаем объект, чтобы игрок мог попробовать снова
    }
}