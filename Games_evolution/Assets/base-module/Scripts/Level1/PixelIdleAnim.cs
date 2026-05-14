using UnityEngine;
using System.Collections;

public class PixelIdleAnim : MonoBehaviour
{
    [Header("Параметры дыхания")]
    [SerializeField] private float pulseSpeed = 2f;      // скорость пульсации
    [SerializeField] private float pulseAmount = 0.05f;  // насколько изменяется масштаб
    [SerializeField] private bool alsoMoveY = true;      // слегка подниматься/опускаться
    [SerializeField] private float bobAmount = 0.02f;    // амплитуда движения по Y

    private Vector3 originalScale;
    private float originalY;

    void Start()
    {
        originalScale = transform.localScale;
        originalY = transform.localPosition.y;
        StartCoroutine(IdleAnimation());
    }

    private IEnumerator IdleAnimation()
    {
        float time = 0f;
        while (true) // бесконечно, пока жив объект
        {
            time += Time.deltaTime * pulseSpeed;
            // Плавное изменение масштаба по синусоиде
            float scaleFactor = 1f + Mathf.Sin(time) * pulseAmount;
            transform.localScale = originalScale * scaleFactor;

            if (alsoMoveY)
            {
                float yOffset = Mathf.Sin(time * 1.5f) * bobAmount;
                transform.localPosition = new Vector3(transform.localPosition.x, originalY + yOffset, transform.localPosition.z);
            }

            yield return null;
        }
    }
}