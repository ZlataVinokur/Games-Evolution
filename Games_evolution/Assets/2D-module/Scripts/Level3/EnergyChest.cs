using UnityEngine;

public class EnergyChest : MonoBehaviour
{
    public int meterIndex;
    public string requiredItem;
    public Sprite closedSprite;
    public Sprite openedSprite;
    public AudioClip openSound;

    private SpriteRenderer sr;
    private bool isOpened = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public bool TryUseItem(string itemType)
    {
        if (isOpened) return false;
        if (itemType != requiredItem) return false;

        isOpened = true;
        if (openedSprite != null) sr.sprite = openedSprite;
        if (openSound != null) AudioSource.PlayClipAtPoint(openSound, transform.position);

        RPGLevelManager.Instance?.ActivateMeter(meterIndex);
        Destroy(gameObject, 0.5f);
        return true;
    }
}