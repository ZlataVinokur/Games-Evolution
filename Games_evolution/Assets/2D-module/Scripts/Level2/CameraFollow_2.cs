using UnityEngine;

public class CameraFollow_2 : MonoBehaviour
{
    [SerializeField] private Transform target;            // игрок
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float fixedX = 0f;           // зафиксированная позиция по X
    private float currentVelocityY;

    void LateUpdate()
    {
        if (target == null) return;

        // Новая позиция камеры: X жёстко фиксирован, Y – только при подъёме игрока
        float targetY = Mathf.Max(transform.position.y, target.position.y);
        float smoothedY = Mathf.SmoothDamp(transform.position.y, targetY, ref currentVelocityY, smoothSpeed);

        transform.position = new Vector3(fixedX, smoothedY, -10f);
    }
}