using UnityEngine;

public class IsometricMovement : MonoBehaviour
{
    public float speed = 4f;
    private Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(h, 0, v).normalized * speed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        // ������� ��������� �� �����������
        if (movement != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
    }
}