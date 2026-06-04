using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    private PlatformerController player;

    void Start()
    {
        player = PlatformerController.Instance;
    }

    void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < player.Health ? fullHeart : emptyHeart;
        }
    }
}