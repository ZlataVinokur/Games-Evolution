using UnityEngine;

public class ScalesTrigger : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private GameObject hiddenMouseOnScales;
    [SerializeField] private Animator scalesAnimator;  // ссылка на Animator весов

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

        // Диалог
        if (gameEvents != null)
            gameEvents.TriggerMovementFact();
    }
}