using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_3 : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float smoothTime;

    [Header("Поворот персонажа")]
    [SerializeField] private float rotationSmoothTime;
    [SerializeField] private float maxRotationSpeed;

    [Header("Прыжки")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;

    [Header("Камеры")]
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    [Header("Взаимодействие")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private bool isFirstPerson = true;
    private Vector3 currentVelocity;
    private Vector3 moveDirection;
    private float currentSpeed;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isRunning;



    private float currentRotationY;
    private float rotationVelocity;
    private Transform cameraTransform;

    private CinemachinePanTilt firstPersonPanTilt;

    private InputSystem3D input;

    private void Awake()
    {
        input = new InputSystem3D();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        if (groundCheck == null)
            groundCheck = transform;

        cameraTransform = Camera.main.transform;

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
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speedPercent = moveInput.magnitude;
        if (isRunning && speedPercent > 0.1f)
            speedPercent = 1f;
        else if (speedPercent > 0.1f)
            speedPercent = 0.5f;

        animator.SetFloat("Speed", speedPercent);
        animator.SetBool("Grounded", isGrounded);
    }

    private void HandleRotation()
    {
        float targetRotation;

        if (isFirstPerson)
        {
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

        if (moveInput.magnitude > 0 || isFirstPerson == true)
        {
            currentRotationY = Mathf.SmoothDampAngle(
                currentRotationY,
                targetRotation,
                ref rotationVelocity,
                rotationSmoothTime,
                maxRotationSpeed
            );
        }


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

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 targetDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        moveDirection = Vector3.SmoothDamp(moveDirection, targetDirection, ref currentVelocity, smoothTime);

        Vector3 movement = targetDirection * currentSpeed * Time.deltaTime;
        movement.y = verticalVelocity * Time.deltaTime;
        controller.Move(movement);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    private void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }

    public void SwitchCamera()
    {
        isFirstPerson = !isFirstPerson;
        SetCamera(isFirstPerson);

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