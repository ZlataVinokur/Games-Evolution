using UnityEngine;

public class EnergyCollectionManager : MonoBehaviour
{
    private int currentEnergy = 0;
    public int energyRequired = 5;

    void Start()
    {
        NotificationManager.Instance?.ShowNotification($"Нужно полить {energyRequired} грибочков лейкой (нажми E)", 3f);
    }

    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        NotificationManager.Instance?.ShowNotification($"Энергия: {currentEnergy}/{energyRequired}", 1f);
        if (currentEnergy >= energyRequired && !RPGLevelManager.Instance.metersActivated[2])
        {
            RPGLevelManager.Instance.ActivateMeter(2);
            UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Достаточно энергии! Зелёный измеритель активирован." }, "encyclopedia", "happy");
        }
    }
}