using UnityEngine;

public class ScrollPickup : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string scrollText = "Текст свитка...";
    [SerializeField] private GameObject scrollPanel; // ссылка на панель UI
    [SerializeField] private TMPro.TextMeshProUGUI scrollTextUI; // текстовое поле внутри панели


    public static bool IsAnyScrollOpen = false;

    private void Start()
    {
        if (scrollPanel != null)
            scrollPanel.SetActive(false);
    }

    public void OpenScroll()
    {
        IsAnyScrollOpen = true;
        if (scrollPanel != null && scrollTextUI != null)
        {
            scrollTextUI.text = scrollText;
            scrollPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // пауза игры
        }

    }

    public void CloseScroll()
    {
        IsAnyScrollOpen = false;
        if (scrollPanel != null)
        {
            scrollPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}