using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider objectCollider;
    private bool isPickedUp = false;
    private Transform holder;

    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float holdHeight = -0.5f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float throwForce = 10f;

    [Header("“€жЄлый предмет")]
    [SerializeField] private bool isHeavy = false;
    [SerializeField] private float heavyMoveSpeed = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!isPickedUp || holder == null) return;

        Vector3 targetPosition;

        if (isHeavy)
        {
            // “€жЄлый предмет: остаЄтс€ на земле, еле двигаетс€
            Vector3 flatForward = holder.forward;
            flatForward.y = 0;
            flatForward.Normalize();

            Vector3 desiredPos = holder.position + flatForward * holdDistance;
            desiredPos.y = transform.position.y; // не взлетает


            targetPosition = desiredPos;
            transform.position = Vector3.Lerp(transform.position, targetPosition, heavyMoveSpeed * Time.deltaTime);
        }
        else
        {
            targetPosition = holder.position + holder.forward * holdDistance + holder.up * holdHeight;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, holder.rotation, smoothSpeed * Time.deltaTime);
        }
    }

    public void PickUp(Transform newHolder)
    {
        if (isPickedUp) return;
        holder = newHolder;
        isPickedUp = true;

        if (isHeavy)
        {
            // “€жЄлый: не отключаем физику полностью, только замораживаем rotation
            if (rb != null)
            {
                rb.isKinematic = true; // чтобы не падал, но позицию контролируем мы
            }
        }
        else
        {
            rb.isKinematic = true;
        }

        if (objectCollider != null)
            objectCollider.enabled = false;
    }

    public void Drop()
    {
        if (!isPickedUp) return;

        isPickedUp = false;
        holder = null;

        rb.isKinematic = false;
        objectCollider.enabled = true;

        Camera cam = Camera.main;
        if (cam != null)
        {
            rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    public void MakeLight()
    {
        isHeavy = false;

    }
}