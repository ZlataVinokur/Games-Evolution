using UnityEngine;

public class LadderDropTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hiddenLadder; // заранее размещённая лестница-мост

    private void Start()
    {
        if (hiddenLadder != null)
            hiddenLadder.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ladder")) return;

        PickupableObject pickup = other.GetComponent<PickupableObject>();
        if (pickup == null) return;

        // Удаляем лестницу в руках
        Destroy(pickup.gameObject);

        // Показываем мост
        if (hiddenLadder != null)
            hiddenLadder.SetActive(true);
    }
}