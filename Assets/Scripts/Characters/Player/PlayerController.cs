using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour,
    PlayerJump.ICoroutineRunner,
    PlayerCamera.ICoroutineRunner
{
    [Header("Настройки")]
    [SerializeField] private PlayerSettings settings;

    [Header("Ссылки")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform groundCheck;

    // Компоненты
    private CharacterController controller;
    private PlayerState state;
    private PlayerInputHandler input;

    // Модули
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerCrouch crouch;
    private PlayerCamera playerCameraController;

    void Start()
    {
        InitializeComponents();
        SetupModules();
        SetupCursor();
    }

    void Update()
    {
        UpdateGroundCheck();
        HandleInput();
        ApplyGravity();
        ApplyMovement();
    }

    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        state = new PlayerState();
        input = new PlayerInputHandler();

        CreateMissingObjects();
    }

    private void SetupModules()
    {
        movement = new PlayerMovement(settings, state, transform);

        // Передаем this как ICoroutineRunner для прыжков и камеры
        jump = new PlayerJump(settings, state, this);
        crouch = new PlayerCrouch(settings, state, controller, transform);
        playerCameraController = new PlayerCamera(settings, state, playerCamera, cameraHolder, this);
    }

    private void CreateMissingObjects()
    {
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = check.transform;
        }

        if (cameraHolder == null && playerCamera != null)
        {
            cameraHolder = playerCamera.parent ?? CreateCameraHolder();
        }
    }

    private Transform CreateCameraHolder()
    {
        GameObject holder = new GameObject("CameraHolder");
        holder.transform.SetParent(transform);
        holder.transform.localPosition = Vector3.zero;
        playerCamera.SetParent(holder.transform);
        playerCamera.localPosition = Vector3.zero;
        return holder.transform;
    }

    private void SetupCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateGroundCheck()
    {
        if (groundCheck == null) return;

        state.IsGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            settings.groundCheckDistance
        );

        Debug.DrawRay(groundCheck.position, Vector3.down * settings.groundCheckDistance,
                     state.IsGrounded ? Color.green : Color.red);
    }

    private void HandleInput()
    {
        // Камера
        playerCameraController.HandleMouseLook(
            input.GetMouseX(),
            input.GetMouseY(),
            transform
        );

        // Движение
        movement.CalculateMovement(
            input.GetHorizontal(),
            input.GetVertical()
        );

        // Бег
        movement.HandleRun(input.GetRun());

        // Прыжок
        if (jump.ShouldJump(state.IsGrounded, input.GetJumpDown()))
        {
            jump.PerformJump();
        }

        // Приседание
        if (crouch.ShouldCrouch(input.GetCrouchDown()))
        {
            crouch.ApplyCrouch(true);
            playerCameraController.MoveCameraForCrouch(true);
        }
        else if (crouch.ShouldStandUp(input.GetCrouchDown()))
        {
            crouch.ApplyCrouch(false);
            playerCameraController.MoveCameraForCrouch(false);
        }
    }

    private void ApplyGravity()
    {
        if (state.IsGrounded && state.Velocity.y < 0)
        {
            state.Velocity = new Vector3(state.Velocity.x, -2f, state.Velocity.z);
            state.IsJumpBoosted = false; // Сбрасываем флаг прыжка при приземлении
        }

        state.Velocity = new Vector3(
            state.Velocity.x,
            state.Velocity.y + settings.gravity * Time.deltaTime,
            state.Velocity.z
        );
    }

    private void ApplyMovement()
    {
        controller.Move(state.Velocity * Time.deltaTime);
    }

    // Реализация интерфейса ICoroutineRunner для PlayerJump
    void PlayerJump.ICoroutineRunner.StartCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    // Реализация интерфейса ICoroutineRunner для PlayerCamera
    Coroutine PlayerCamera.ICoroutineRunner.StartCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    void PlayerCamera.ICoroutineRunner.StopCoroutine(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    // Геттеры для других скриптов
    public bool IsGrounded() => state.IsGrounded;
    public bool IsRunning() => state.IsRunning;
    public bool IsCrouching() => state.IsCrouching;
    public float GetCurrentSpeed() => state.CurrentSpeed;
    public bool IsJumpBoosted() => state.IsJumpBoosted;
}