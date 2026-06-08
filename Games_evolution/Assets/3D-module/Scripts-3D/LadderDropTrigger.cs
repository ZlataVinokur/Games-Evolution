using UnityEngine;

public class LadderDropTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hiddenLadder;
    [SerializeField] private AudioClip SolvedClip;


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

        if (SolvedClip != null)
            AudioSource.PlayClipAtPoint(SolvedClip, transform.position);

        // Показываем мост
        if (hiddenLadder != null)
            hiddenLadder.SetActive(true);
    }
}