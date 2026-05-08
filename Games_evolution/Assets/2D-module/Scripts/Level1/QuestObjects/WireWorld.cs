using static UnityEditor.Progress;

public class WireWorld : Interactable
{
    void Start()
    {
        interactionType = InteractionType.PickUp;
        itemId = "wire";
    }
}