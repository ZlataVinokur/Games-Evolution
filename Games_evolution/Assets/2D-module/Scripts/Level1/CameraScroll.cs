using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 300f;
    public float minX = -6f;
    public float maxX = 6f;
    public float edgeSize = 30f;
    private Camera cam;

    void Start() => cam = Camera.main;

    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.mousePosition.x < edgeSize && pos.x > minX)
            pos.x -= scrollSpeed * Time.deltaTime;
        if (Input.mousePosition.x > Screen.width - edgeSize && pos.x < maxX)
            pos.x += scrollSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}