using UnityEngine;

public class MoralChoiceMiniGame : MonoBehaviour
{
    private int energy = 0;
    public int energyNeeded = 5;
    private bool isActive = false;
    private bool completed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (completed) return;
        if (other.CompareTag("Player") && !RPGLevelManager.Instance.metersActivated[2])
        {
            isActive = true;
            foreach (var gen in FindObjectsByType<ClickableGenerator>(FindObjectsSortMode.None))
            {
                gen.OnCollect += AddEnergy;
            }
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Собери энергию, взаимодействуя с кристаллами! (5 шт.)", 2f);
        }
    }

    void AddEnergy()
    {
        if (!isActive) return;
        energy++;
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Энергия: {energy}/{energyNeeded}", 0.5f);
        if (energy >= energyNeeded && !completed)
        {
            completed = true;
            // Активируем измеритель сразу, без диалога
            RPGLevelManager.Instance.ActivateMeter(2);
            // Отключаем зону, чтобы нельзя было собрать ещё раз
            GetComponent<Collider2D>().enabled = false;
            // Дополнительно можно показать сообщение
            UnifiedInfoSystem.Instance?.ShowTimedMessage("Ты накопил достаточно энергии! Измеритель активирован.", 2f);
        }
    }
}