using UnityEngine;


public class Shield : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    public bool isActive = false;

    public bool IsActive => isActive;

    public void Activate()
    {
        isActive = true;
        Invoke(nameof(Deactivate), duration);
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
