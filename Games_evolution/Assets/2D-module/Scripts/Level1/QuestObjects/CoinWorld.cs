using static UnityEditor.Progress;

public class CoinWorld : Interactable
{
    void Start()
    {
        interactionType = InteractionType.PickUp;
        itemId = "coin";
    }
}