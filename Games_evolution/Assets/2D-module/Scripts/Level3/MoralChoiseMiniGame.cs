using UnityEngine;

public class MoralChoiceMiniGame : MonoBehaviour
{
    private int energy = 0;
    public int energyNeeded = 5;
    public GameObject choiceDialogPrefab; // префаб ChoiceDialog
    private bool dialogShown = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !RPGLevelManager.Instance.metersActivated[2])
        {
            // јктивируем генераторы
            foreach (var gen in FindObjectsOfType<ClickableGenerator>())
                gen.OnCollect += AddEnergy;
        }
    }

    void AddEnergy()
    {
        energy++;
        UnifiedInfoSystem.Instance.ShowTimedMessage($"Ёнерги€: {energy}/{energyNeeded}", 0.5f);
        if (energy >= energyNeeded && !dialogShown)
        {
            dialogShown = true;
            var dialog = Instantiate(choiceDialogPrefab).GetComponent<ChoiceDialog>();
            dialog.OnChoice += (feed) =>
            {
                RPGLevelManager.Instance.SetTamagotchiBuff(feed);
                RPGLevelManager.Instance.ActivateMeter(2);
                Destroy(gameObject);
            };
        }
    }
}