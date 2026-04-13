using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Поворот персонажа")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float maxRotationSpeed = 360f;

    [Header("Прыжки")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Камеры")]
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    private CharacterController controller;
    private Vector2 moveInput;
    private bool isFirstPerson = true;
    private Vector3 currentVelocity;
    private Vector3 moveDirection;
    private float currentSpeed;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isRunning;

    // Для плавного поворота персонажа
    private float currentRotationY;
    private float rotationVelocity;
    private Transform cameraTransform;

    private CinemachinePanTilt firstPersonPanTilt;

    // Input System
    private InputSystem3D input;

    private void Awake()
    {
        input = new InputSystem3D();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (groundCheck == null)
            groundCheck = transform;

        cameraTransform = Camera.main.transform;

        // Получаем компоненты камер
        firstPersonPanTilt = firstPersonCam.GetComponent<CinemachinePanTilt>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Gameplay3D.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Gameplay3D.Move.canceled += ctx => moveInput = Vector2.zero;
        input.Gameplay3D.SwitchCamera.performed += ctx => SwitchCamera();
        input.Gameplay3D.Run.performed += ctx => isRunning = true;
        input.Gameplay3D.Run.canceled += ctx => isRunning = false;
        input.Gameplay3D.Jump.performed += ctx => Jump();
    }

    private void Start()
    {
        SetCamera(true);
        currentRotationY = transform.eulerAngles.y;
    }

    private void Update()
    {
        CheckGrounded();
        HandleRotation();
        Move();
        ApplyGravity();
    }

    private void HandleRotation()
    {
        float targetRotation;

        if (isFirstPerson)
        {
            // Для первого лица - берем угол Pan из компонента PanTilt
            if (firstPersonPanTilt != null)
            {
                targetRotation = firstPersonPanTilt.PanAxis.Value;
            }
            else
            {
                targetRotation = cameraTransform.eulerAngles.y;
            }
        }
        else
        {
            targetRotation = cameraTransform.eulerAngles.y;
        }

        // Плавно поворачиваем персонажа к целевому углу
        currentRotationY = Mathf.SmoothDampAngle(
            currentRotationY,
            targetRotation,
            ref rotationVelocity,
            rotationSmoothTime,
            maxRotationSpeed
        );

        transform.rotation = Quaternion.Euler(0, currentRotationY, 0);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position + Vector3.down * 0.1f, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
    }

    private void Move()
    {
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Получаем направления относительно поворота персонажа
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Желаемое направление движения (локальное для персонажа)
        Vector3 targetDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // Плавное изменение направления движения
        moveDirection = Vector3.SmoothDamp(moveDirection, targetDirection, ref currentVelocity, smoothTime);

        // Применяем движение
        Vector3 movement = targetDirection * currentSpeed * Time.deltaTime;
        movement.y = verticalVelocity * Time.deltaTime;
        controller.Move(movement);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }

    public void SwitchCamera()
    {
        isFirstPerson = !isFirstPerson;
        SetCamera(isFirstPerson);

        // Синхронизируем текущий поворот персонажа с новой камерой
        if (isFirstPerson && firstPersonPanTilt != null)
        {
            firstPersonPanTilt.PanAxis.Value = currentRotationY;
        }
  
    }

    private void SetCamera(bool firstPerson)
    {
        if (firstPerson)
        {
            firstPersonCam.Priority = 10;
            thirdPersonCam.Priority = 0;
        }
        else
        {
            firstPersonCam.Priority = 0;
            thirdPersonCam.Priority = 10;
        }
    }

    private void OnDisable()
    {
        input.Disable();
    }

}