using UnityEngine;

public class AltarVisual : MonoBehaviour
{
    public SpriteRenderer itemSpriteRenderer;
    public Color defaultColor = Color.white;
    public Color activatedColor = Color.green;
    public AudioSource activateSound;
    private SpriteRenderer altarRenderer;

    void Start()
    {
        altarRenderer = GetComponent<SpriteRenderer>();
        if (itemSpriteRenderer != null) itemSpriteRenderer.sprite = null;
    }

    public void PlaceItem(Sprite itemIcon)
    {
        if (itemSpriteRenderer != null) itemSpriteRenderer.sprite = itemIcon;
        if (altarRenderer != null) altarRenderer.color = activatedColor;
        activateSound?.Play();
    }

    public void ResetAltar()
    {
        if (itemSpriteRenderer != null) itemSpriteRenderer.sprite = null;
        if (altarRenderer != null) altarRenderer.color = defaultColor;
    }
}