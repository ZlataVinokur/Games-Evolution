using UnityEngine;

public class ArcadeMachine : MonoBehaviour
{
    public Sprite brokenSprite;
    public Sprite fixedSprite;
    private SpriteRenderer sr;
    public GameObject coinPrefab; // монетка для спавна
    public Transform coinSpawnPoint;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        // Проверяем условия фиксации
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
        DialogueSystem.Instance.ShowDialogue(new[] {
            "Аркадный автомат запустился! Из монетоприёмника выпала монетка."
        });
        Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        // Можно сразу добавить монету в инвентарь или сделать её интерактивным объектом
    }
}