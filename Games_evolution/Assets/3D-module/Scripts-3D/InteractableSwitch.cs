using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractableSwitch : MonoBehaviour
{
    [Header("Свет")]
    [SerializeField] private int lightmapSetIndex = -1;
    [SerializeField] private List<Light> lightsToToggle;

    [Header("Взаимодействие")]
    [SerializeField] private float interactionDistance = 3f;

    private Transform playerCamera;
    private InputSystem3D input;
    private LightmapSwitcher lightmapSwitcher;
    private PickupableObject currentHeldObject;
    private bool isActive = false;

    private void Awake()
    {
        input = new InputSystem3D();
    }

    private void Start()
    {
        playerCamera = Camera.main.transform;
        lightmapSwitcher = FindObjectOfType<LightmapSwitcher>();
        if (lightmapSwitcher == null)
            Debug.LogWarning("LightmapSwitcher не найден на сцене!");
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

    private void Update()
    {
        UpdateHint();
    }

    private void UpdateHint()
    {
        if (InteractionHint.Instance == null) return;


        // Рейкаст
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Зелье
            if (hit.transform.CompareTag("Potion"))
            {
                InteractionHint.Instance.ShowDrink();
                return;
            }

            // Свиток
            ScrollPickup scroll = hit.transform.GetComponent<ScrollPickup>();
            if (scroll != null)
            {
                InteractionHint.Instance.ShowRead();
                return;
            }

            // Подбираемый предмет
            PickupableObject pickupable = hit.transform.GetComponent<PickupableObject>();
            if (pickupable != null)
            {
                InteractionHint.Instance.ShowPickup();
                return;
            }

            // Выключатель
            if (hit.transform == transform)
            {
                InteractionHint.Instance.ShowInteract();
                return;
            }
        }

        // Ничего не нашли
        InteractionHint.Instance.Hide();
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Зелье
            if (hit.transform.CompareTag("Potion"))
            {
                PotionBottle potion = hit.transform.GetComponent<PotionBottle>();
                if (potion != null)
                {
                    potion.Drink();
                    return;
                }
            }

            // Свиток
            ScrollPickup scroll = hit.transform.GetComponent<ScrollPickup>();
            if (scroll != null && currentHeldObject == null)
            {
                scroll.OpenScroll();
                return;
            }

            // Подбираемый предмет
            PickupableObject pickupable = hit.transform.GetComponent<PickupableObject>();
            if (pickupable != null && currentHeldObject == null)
            {
                GameObject holder = new GameObject("ItemHolder");
                holder.transform.SetParent(playerCamera);
                holder.transform.localPosition = Vector3.zero;
                holder.transform.localRotation = Quaternion.identity;

                pickupable.PickUp(holder.transform);
                currentHeldObject = pickupable;
                return;
            }

            // Выключатель
            if (hit.transform == transform)
            {
                Toggle();
            }
        }
    }

    private void OnInteractReleased(InputAction.CallbackContext context)
    {
        if (currentHeldObject != null)
        {
            currentHeldObject.Drop();
            currentHeldObject = null;
        }
    }

    private void Toggle()
    {
        isActive = !isActive;

        if (lightmapSwitcher != null && lightmapSetIndex >= 0)
        {
            if (isActive)
                lightmapSwitcher.LoadLightmapSet(lightmapSetIndex);
            else
                lightmapSwitcher.LoadLightmapSet(0);
        }

        foreach (Light light in lightsToToggle)
        {
            if (light != null)
                light.enabled = isActive;
        }
    }
}