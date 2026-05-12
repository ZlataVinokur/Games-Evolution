using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float scale = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }
}