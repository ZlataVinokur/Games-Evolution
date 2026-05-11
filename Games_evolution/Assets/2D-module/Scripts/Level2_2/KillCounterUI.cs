using UnityEngine;
using UnityEngine.UI;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private Text killText;
    private PlatformerController player;

    void Start()
    {
        player = PlatformerController.Instance;
    }

    void Update()
    {
        killText.text = $"Убито: {player.EnemiesKilled}/5";
    }
}