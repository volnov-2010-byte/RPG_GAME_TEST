using UnityEngine;

public class PlayerCrouch
{
    private readonly PlayerSettings settings;
    private readonly PlayerState state;
    private readonly CharacterController controller;
    private readonly Transform playerTransform;

    public PlayerCrouch(PlayerSettings settings, PlayerState state,
                       CharacterController controller, Transform playerTransform)
    {
        this.settings = settings;
        this.state = state;
        this.controller = controller;
        this.playerTransform = playerTransform;
    }

    public bool ShouldCrouch(bool crouchInput)
    {
        return crouchInput && !state.IsCrouching;
    }

    public bool ShouldStandUp(bool crouchInput)
    {
        return crouchInput && state.IsCrouching && CanStandUp();
    }

    public void ApplyCrouch(bool shouldCrouch)
    {
        state.IsCrouching = shouldCrouch;

        if (shouldCrouch)
        {
            controller.height = 0.8f;
            controller.center = new Vector3(0, -0.5f, 0);
            state.CurrentSpeed = settings.crouchSpeed;
        }
        else
        {
            controller.height = 1.75f;
            controller.center = Vector3.zero;
            state.CurrentSpeed = settings.walkSpeed;
        }
    }

    private bool CanStandUp()
    {
        Vector3 rayStart = playerTransform.position + Vector3.up * 0.5f;
        float rayLength = 1.0f;

        //Закомментировать/Раскомментировать строку ниже для отображения луча - проверки встал ли персонаж.
        //Debug.DrawRay(rayStart, Vector3.up * rayLength, Color.blue, 1f);

        return !Physics.Raycast(rayStart, Vector3.up, rayLength);
    }
}
