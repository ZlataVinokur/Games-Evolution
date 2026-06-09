using UnityEngine;

public class ArcadeMachine : Interactable
{
    public Sprite brokenSprite;
    public Sprite fixedSprite;
    private SpriteRenderer sr;
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Настраиваем возможности: осмотр + возможность действия (починка через флаги)
        capabilities = InteractionCapabilities.Look | InteractionCapabilities.Use;
        dialogueOnLook = new string[] { "Старый аркадный автомат. Не работает. Нужно его починить." };
        // Статья откроется при первом любом взаимодействии (осмотр или попытка починить)
        articleOnFirstInteractId = "article_arcade";
    }

    void Update()
    {
        if (!GameManager.Instance.GetFlag("arcade_fixed") &&
            GameManager.Instance.GetFlag("bush_opened") &&
            GameManager.Instance.GetFlag("wire_plugged"))
        {
            FixMachine();
        }
    }

    void FixMachine()
    {
        if (GameManager.Instance.GetFlag("arcade_fixed")) return;

        sr.sprite = fixedSprite;
        GameManager.Instance.SetFlag("arcade_fixed", true);

        UnifiedInfoSystem.Instance.ShowDialogue(new[] {
            "Аркадный автомат запустился! Из монетоприёмника выпала монетка."
        }, "encyclopedia", "happy");

        Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        // Статья уже открыта через TryFirstInteraction, но если нет – дубляж не страшен
        // (UnlockArticle внутри защищён от повторного открытия)
        UnifiedInfoSystem.Instance?.UnlockArticle("article_arcade");

        // После починки автомат больше не активен для взаимодействия
        capabilities = InteractionCapabilities.Look;
        dialogueOnLook = new string[] { "Автомат работает, но больше ничего не даёт." };
    }
}