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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
    }

    public void PickUp(Transform newHolder)
    {
        if (isPickedUp) return;

        holder = newHolder;
        isPickedUp = true;

        rb.isKinematic = true;
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

    private void Update()
    {
        if (isPickedUp && holder != null)
        {
            Vector3 targetPosition = holder.position + holder.forward * holdDistance + holder.up * holdHeight;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, holder.rotation, smoothSpeed * Time.deltaTime);
        }
    }
}