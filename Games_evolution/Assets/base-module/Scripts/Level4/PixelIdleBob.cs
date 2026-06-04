using UnityEngine;

public class PixelIdleBob : MonoBehaviour
{
    public float bobSpeed = 1.5f;   // скорость покачивания
    public float bobAmount = 0.03f; // амплитуда (высота покачивания)

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // Новое положение по Y: исходное + синусоидальное смещение
        float newY = startLocalPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);
    }
}