using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = (i < currentLives) ? fullHeartSprite : emptyHeartSprite;
        }
    }

    public void AnimateEmptyHeart(int heartIndex)
    {
        if (heartIndex >= 0 && heartIndex < heartImages.Length)
            StartCoroutine(PulseHeart(heartImages[heartIndex]));
    }

    private IEnumerator PulseHeart(Image heart)
    {
        if (heart == null) yield break;
        Vector3 originalScale = heart.transform.localScale;
        Vector3 targetScale = originalScale * 0.6f;
        float duration = 0.1f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            heart.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            heart.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        heart.transform.localScale = originalScale;
    }
}