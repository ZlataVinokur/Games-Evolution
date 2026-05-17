using UnityEngine;

public class RhythmMiniGame : MonoBehaviour
{
    public float beatInterval = 1f;
    private float nextBeatTime;
    private bool isActive = false;
    public int successesNeeded = 3;
    private int successes = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !RPGLevelManager.Instance.metersActivated[0])
        {
            isActive = true;
            nextBeatTime = Time.time + beatInterval;
            UnifiedInfoSystem.Instance.ShowTimedMessage("Нажимай ПРОБЕЛ в такт! (3 успеха)", 2f);
        }
    }

    void Update()
    {
        if (!isActive) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float diff = Mathf.Abs(Time.time - nextBeatTime);
            if (diff < 0.2f)
            {
                successes++;
                UnifiedInfoSystem.Instance.ShowTimedMessage("Отлично!", 0.5f);
                if (successes >= successesNeeded)
                {
                    RPGLevelManager.Instance.ActivateMeter(0);
                    isActive = false;
                    Destroy(gameObject);
                }
                else
                {
                    nextBeatTime = Time.time + beatInterval;
                }
            }
            else
            {
                UnifiedInfoSystem.Instance.ShowTimedMessage("Мимо! Попробуй ещё.", 1f);
                successes = 0; // сброс
            }
        }
    }
}