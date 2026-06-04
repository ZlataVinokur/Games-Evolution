using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float leftBound = -7.5f;
    [SerializeField] private float rightBound = 7.5f;
    
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    private bool controlsEnabled = true;
    void Start()
    {
        originalScale = transform.localScale;
    }
    
    void Update()
    {
        if (!controlsEnabled) return;
        float move = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + new Vector3(move * speed * Time.deltaTime, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        transform.position = newPosition;
    }
    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
    // Метод для бонуса "Расширение платформы"
    public void ExpandTemporarily(float multiplier, float duration)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        transform.localScale = originalScale * multiplier;
        scaleCoroutine = StartCoroutine(ResetScaleAfterDelay(duration));
    }
    
    private IEnumerator ResetScaleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.localScale = originalScale;
    }
}