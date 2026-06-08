using UnityEngine;

public class ScalesTrigger : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private GameObject hiddenMouseOnScales;
    [SerializeField] private Animator scalesAnimator;
    [SerializeField] private AudioClip SolvedClip;
    [SerializeField] private AudioSource GearAudioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Mouse")) return;

        PickupableObject pickup = other.GetComponent<PickupableObject>();
        if (pickup == null) return;

        // Удаляем мышь в руках
        Destroy(pickup.gameObject);

        // Показываем мышь на весах
        if (hiddenMouseOnScales != null)
            hiddenMouseOnScales.SetActive(true);

        // Запускаем анимацию весов
        if (scalesAnimator != null)
            scalesAnimator.SetTrigger("Solve");

        if (SolvedClip != null)
            AudioSource.PlayClipAtPoint(SolvedClip, transform.position);

        if (GearAudioSource != null)
        {
            StartCoroutine(PlayGearSoundDelayed(3f));
        }

        // Диалог
        if (gameEvents != null)
            gameEvents.TriggerMovementFact();
    }

    private System.Collections.IEnumerator PlayGearSoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        GearAudioSource.Play();
    }
}