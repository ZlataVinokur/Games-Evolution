using UnityEngine;
using TMPro;

public class WorldToast : MonoBehaviour
{
    public TextMeshProUGUI tmp; // это UI-версия
    public Vector3 offset = new Vector3(0, 0.5f, 0);
    private float lifeTimer = 0f;

    public void Show(string message, float duration = 1.5f)
    {
        if (tmp == null)
        {
            Debug.LogError("WorldToast: tmp не назначен", this);
            return;
        }
        tmp.text = message;
        lifeTimer = duration;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (lifeTimer > 0)
        {
            lifeTimer -= Time.unscaledDeltaTime;
            if (lifeTimer <= 0) gameObject.SetActive(false);
        }
        if (transform.parent != null)
            transform.localPosition = offset;
    }
}