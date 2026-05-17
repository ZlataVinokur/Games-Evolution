using UnityEngine;
using UnityEngine.UI;

public class ChoiceDialog : MonoBehaviour
{
    public Button feedButton;
    public Button progressButton;
    public System.Action<bool> OnChoice; // true = накормить, false = прогресс

    void Start()
    {
        feedButton.onClick.AddListener(() => { OnChoice?.Invoke(true); Destroy(gameObject); });
        progressButton.onClick.AddListener(() => { OnChoice?.Invoke(false); Destroy(gameObject); });
    }
}