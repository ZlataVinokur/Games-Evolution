using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    [Header("Settings")]
    public float defaultDuration = 2f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message, float duration = -1)
    {
        if (duration <= 0) duration = defaultDuration;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowCoroutine(message, duration));
    }

    private IEnumerator ShowCoroutine(string message, float duration)
    {
        if (notificationText != null)
            notificationText.text = message;

        if (notificationPanel != null)
            notificationPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(duration);

        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        currentCoroutine = null;
    }
}