using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractableSwitch : MonoBehaviour
{
    [SerializeField] private List<Light> lightsToToggle;
    [SerializeField] private float interactionDistance = 3f;

    private Transform playerCamera;
    private InputSystem3D input;
    private PickupableObject currentHeldObject;
    private bool isHolding = false;

    private void Awake()
    {
        input = new InputSystem3D();
    }

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void OnEnable()
    {
        input.Enable();
        input.Gameplay3D.Interact.performed += OnInteractPressed;
        input.Gameplay3D.Interact.canceled += OnInteractReleased;
    }

    private void OnDisable()
    {
        input.Gameplay3D.Interact.performed -= OnInteractPressed;
        input.Gameplay3D.Interact.canceled -= OnInteractReleased;
        input.Disable();
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PickupableObject pickupable = hit.transform.GetComponent<PickupableObject>();
            if (pickupable != null && currentHeldObject == null)
            {
                GameObject holder = new GameObject("ItemHolder");
                holder.transform.SetParent(playerCamera);
                holder.transform.localPosition = Vector3.zero;
                holder.transform.localRotation = Quaternion.identity;

                pickupable.PickUp(holder.transform);
                currentHeldObject = pickupable;
                isHolding = true;
                return;
            }

            if (hit.transform == transform)
            {
                ToggleLights();
            }
        }
    }

    private void OnInteractReleased(InputAction.CallbackContext context)
    {
        if (currentHeldObject != null)
        {
            currentHeldObject.Drop();

            

            currentHeldObject = null;
            isHolding = false;
        }
    }

    private void ToggleLights()
    {
        foreach (Light light in lightsToToggle)
        {
            if (light != null)
                light.enabled = !light.enabled;
        }
    }

    private void OnGUI()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            PickupableObject pickupable = hit.transform.GetComponent<PickupableObject>();
            if (pickupable != null && currentHeldObject == null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                         "Удерживайте E чтобы нести");
                return;
            }

            if (hit.transform == transform && currentHeldObject == null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                         "Нажмите E для взаимодействия");
            }
        }

        if (currentHeldObject != null)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                     "Отпустите E чтобы бросить");
        }
    }
}