using UnityEngine;

public class ArcadeMachine : MonoBehaviour
{
    public Sprite brokenSprite;
    public Sprite fixedSprite;
    private SpriteRenderer sr;
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;

    void Start() => sr = GetComponent<SpriteRenderer>();

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
        sr.sprite = fixedSprite;
        GameManager.Instance.SetFlag("arcade_fixed", true);
        UnifiedInfoSystem.Instance.ShowDialogue(new[] {
            "Аркадный автомат запустился! Из монетоприёмника выпала монетка."
        }, speaker: "encyclopedia", emotion: "happy");

        Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        UnifiedInfoSystem.Instance?.UnlockArticle("article_arcade");
    }
}