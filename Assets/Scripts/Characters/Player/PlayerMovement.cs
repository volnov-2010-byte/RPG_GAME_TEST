using UnityEngine;

public class PlayerMovement
{
    private readonly PlayerSettings settings;
    private readonly PlayerState state;
    private readonly Transform playerTransform;

    public PlayerMovement(PlayerSettings settings, PlayerState state, Transform playerTransform)
    {
        this.settings = settings;
        this.state = state;
        this.playerTransform = playerTransform;
    }

    public void CalculateMovement(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = CalculateMoveDirection(horizontalInput, verticalInput);
        state.HorizontalVelocity = moveDirection * state.CurrentSpeed;

        // Применяем горизонтальную скорость
        state.Velocity = new Vector3(
            state.HorizontalVelocity.x,
            state.Velocity.y,
            state.HorizontalVelocity.z
        );
    }

    private Vector3 CalculateMoveDirection(float horizontal, float vertical)
    {
        Vector3 direction = Vector3.zero;

        if (vertical != 0)
        {
            float multiplier = vertical > 0 ? 1f : settings.backwardMultiplier;
            direction += playerTransform.forward * vertical * multiplier;
        }

        if (horizontal != 0)
        {
            direction += playerTransform.right * horizontal * settings.strafeMultiplier;
        }

        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }

        return direction;
    }

    public void HandleRun(bool isRunningInput)
    {
        if (!state.IsCrouching)
        {
            state.IsRunning = isRunningInput;
            state.CurrentSpeed = state.IsRunning ? settings.runSpeed : settings.walkSpeed;
        }
    }
}
