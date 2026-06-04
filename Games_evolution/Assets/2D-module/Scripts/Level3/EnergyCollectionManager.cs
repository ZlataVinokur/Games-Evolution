using UnityEngine;

public class EnergyCollectionManager : MonoBehaviour
{
    private int currentEnergy = 0;
    public int energyRequired = 5;

    void Start()
    {
        UnifiedInfoSystem.Instance?.ShowTimedMessage($"Нужно полить {energyRequired} грибочков с помощью лейки (нажми E)", 1.5f);
    }

    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        var toast = GetComponent<WorldToast>();
        if (toast != null) toast.Show($"Энергия: {currentEnergy}/{energyRequired}", 1.5f);
        if (currentEnergy >= energyRequired && !RPGLevelManager.Instance.metersActivated[2])
        {
            RPGLevelManager.Instance.ActivateMeter(2);
            UnifiedInfoSystem.Instance?.ShowDialogue(new[] { "Мы собрали достаточно энергии! Измеритель активирован." }, "encyclopedia", "happy");
        }
    }
}