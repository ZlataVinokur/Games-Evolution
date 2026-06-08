using UnityEngine;
using System.Collections;

public class RetortTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hiddenCandleOnRetort;
    [SerializeField] private Light[] lightsToChange;
    [SerializeField] private Color targetColor = new Color(1f, 0.3f, 0f, 1f);
    [SerializeField] private float colorChangeDuration = 3f;
    [SerializeField] private GameObject potionBottle;
    [SerializeField] private AudioSource boilAudioSource;
    [SerializeField] private AudioClip SolvedClip;
    [SerializeField] private GameEvents gameEvents;

    private Color originalColor;
    private bool activated = false;
    private Coroutine colorRoutine;

    private void Start()
    {
        if (hiddenCandleOnRetort != null)
            hiddenCandleOnRetort.SetActive(false);
        if (potionBottle != null)
            potionBottle.SetActive(false);

        if (lightsToChange.Length > 0 && lightsToChange[0] != null)
            originalColor = lightsToChange[0].color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Candle")) return;

        PickupableObject pickup = other.GetComponent<PickupableObject>();
        if (pickup == null) return;

        activated = true;

        Destroy(pickup.gameObject);

        if (SolvedClip != null)
            AudioSource.PlayClipAtPoint(SolvedClip, transform.position);

        if (hiddenCandleOnRetort != null)
            hiddenCandleOnRetort.SetActive(true);

        if (boilAudioSource != null)
        {
            boilAudioSource.Play();
        }

        if (colorRoutine != null)
            StopCoroutine(colorRoutine);
        colorRoutine = StartCoroutine(ColorPulseRoutine());

        if (potionBottle != null)
            StartCoroutine(ShowPotionDelayed(5f));

        if (gameEvents != null)
            gameEvents.TriggerLightFact();
    }

    private IEnumerator ColorPulseRoutine()
    {
        // К целевому
        yield return StartCoroutine(LerpAllLights(originalColor, targetColor, colorChangeDuration));

        yield return new WaitForSeconds(5f);

        // Обратно к исходному
        yield return StartCoroutine(LerpAllLights(targetColor, originalColor, colorChangeDuration));
    }

    private IEnumerator LerpAllLights(Color from, Color to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            foreach (Light light in lightsToChange)
            {
                if (light != null)
                    light.color = Color.Lerp(from, to, t);
            }
            yield return null;
        }

        foreach (Light light in lightsToChange)
        {
            if (light != null)
                light.color = to;
        }
    }

    private IEnumerator ShowPotionDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (potionBottle != null)
            potionBottle.SetActive(true);
    }
}