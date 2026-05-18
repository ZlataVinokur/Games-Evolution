using UnityEngine;

public class ClickableGenerator : MonoBehaviour
{
    public System.Action OnCollect;
    public bool isActive = true;

    void OnMouseDown()
    {
        if (!isActive) return;
        isActive = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
        OnCollect?.Invoke();
    }
}