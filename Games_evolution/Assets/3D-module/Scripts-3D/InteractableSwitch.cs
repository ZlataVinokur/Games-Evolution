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
    private bool isHolding = false;
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
        {
            Debug.LogWarning(" LightmapSwitcher не найден на сцене!");
        }
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
            // Проверяем, можно ли поднять предмет
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

            // Если это выключатель
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
            isHolding = false;
        }
    }

    private void Toggle()
    {
        isActive = !isActive;

        // Переключаем lightmap
        if (lightmapSwitcher != null && lightmapSetIndex >= 0)
        {
            if (isActive)
                lightmapSwitcher.LoadLightmapSet(lightmapSetIndex);
            else
                lightmapSwitcher.LoadLightmapSet(0);
        }

        // Переключаем дополнительные realtime источники
        foreach (Light light in lightsToToggle)
        {
            if (light != null)
                light.enabled = isActive;
        }
    }

    private void OnGUI()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Показываем подсказку для предметов
            PickupableObject pickupable = hit.transform.GetComponent<PickupableObject>();
            if (pickupable != null && currentHeldObject == null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                         "Удерживайте E чтобы нести");
                return;
            }

            // Показываем подсказку для выключателя
            if (hit.transform == transform && currentHeldObject == null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                         "Нажмите E для взаимодействия");
            }
        }

        // Показываем подсказку, если держим предмет
        if (currentHeldObject != null)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 200, 30),
                     "Отпустите E чтобы бросить");
        }
    }
}