using UnityEngine;
using TMPro;

public class InteractionHint : MonoBehaviour
{
    public static InteractionHint Instance;

    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private string pickupHint = "Удерживайте E чтобы нести";
    [SerializeField] private string interactHint = "Нажмите E для взаимодействия";
    [SerializeField] private string readHint = "Нажмите E чтобы прочитать";
    [SerializeField] private string drinkHint = "Нажмите E чтобы выпить";

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowPickup() => Show(pickupHint);
    public void ShowInteract() => Show(interactHint);
    public void ShowRead() => Show(readHint);
    public void ShowDrink() => Show(drinkHint);
    public void Hide() => hintText.text = "";

    private void Show(string text)
    {
        if (ScrollPickup.IsAnyScrollOpen) return;
        hintText.text = text;
    }
}