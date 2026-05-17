using UnityEngine;

public class MoralChoiceMiniGame : MonoBehaviour
{
    private int energy = 0;
    public int energyNeeded = 5;
    public GameObject choiceDialogPrefab; // префаб диалога с двумя кнопками
    private bool isActive = false;
    private bool completed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (completed) return;
        if (other.CompareTag("Player") && !RPGLevelManager.Instance.metersActivated[2])
        {
            isActive = true;
            // Подписываемся на генераторы
            foreach (var gen in FindObjectsOfType<ClickableGenerator>())
            {
                gen.OnCollect += AddEnergy;
            }
            UnifiedInfoSystem.Instance.ShowTimedMessage("Собери энергию, взаимодействуя с кристаллами!", 2f);
        }
    }

    void AddEnergy()
    {
        if (!isActive) return;
        energy++;
        UnifiedInfoSystem.Instance.ShowTimedMessage($"Энергия: {energy}/{energyNeeded}", 0.5f);
        if (energy >= energyNeeded && !completed)
        {
            completed = true;
            // Показываем диалог выбора
            if (choiceDialogPrefab != null)
            {
                GameObject dialogObj = Instantiate(choiceDialogPrefab, FindObjectOfType<Canvas>().transform);
                ChoiceDialog dialog = dialogObj.GetComponent<ChoiceDialog>();
                dialog.OnChoice += (feed) =>
                {
                    RPGLevelManager.Instance.SetTamagotchiBuff(feed);
                    RPGLevelManager.Instance.ActivateMeter(2);
                    Destroy(dialogObj);
                };
            }
            // Отключаем зону
            GetComponent<Collider2D>().enabled = false;
        }
    }
}