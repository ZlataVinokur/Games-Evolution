using UnityEngine;

public class EnergyCollectionManager : MonoBehaviour
{
    private int currentEnergy = 0;
    public int energyRequired = 5;

    void Start()
    {
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Нужно полить {energyRequired} грибочков с помощью лейки (нажми E)", 3f);
    }

    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Энергия: {currentEnergy}/{energyRequired}", 1f);
        if (currentEnergy >= energyRequired && !RPGLevelManager.Instance.metersActivated[2])
        {
            RPGLevelManager.Instance.ActivateMeter(2);
            UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Достаточно энергии! Зелёный измеритель активирован." }, "encyclopedia", "happy");
        }
    }
}