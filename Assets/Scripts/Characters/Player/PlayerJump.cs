using UnityEngine;
using System.Collections;

public class PlayerJump
{
    private readonly PlayerSettings settings;
    private readonly PlayerState state;
    private readonly ICoroutineRunner coroutineRunner;

    public interface ICoroutineRunner
    {
        void StartCoroutine(IEnumerator routine);
    }

    public PlayerJump(PlayerSettings settings, PlayerState state, ICoroutineRunner coroutineRunner)
    {
        this.settings = settings;
        this.state = state;
        this.coroutineRunner = coroutineRunner;
    }

    public bool ShouldJump(bool isGrounded, bool jumpInput)
    {
        return isGrounded && jumpInput;
    }

    public void PerformJump()
    {
        float jumpForce = Mathf.Sqrt(settings.jumpHeight * -2f * settings.gravity);

        if (state.IsRunning && state.HorizontalVelocity.magnitude > settings.walkSpeed * 0.9f)
        {
            jumpForce *= settings.runJumpBoost;

            Vector3 forwardDirection = GetJumpDirection();
            state.Velocity += forwardDirection * settings.runJumpForwardBoost;
            state.IsJumpBoosted = true;

            // Запускаем корутину через runner
            coroutineRunner.StartCoroutine(ResetJumpBoost());
        }

        state.Velocity = new Vector3(state.Velocity.x, jumpForce, state.Velocity.z);
    }

    private Vector3 GetJumpDirection()
    {
        if (state.HorizontalVelocity.magnitude > 0.1f)
            return state.HorizontalVelocity.normalized;

        return Vector3.forward;
    }

    private IEnumerator ResetJumpBoost()
    {
        yield return new WaitForSeconds(settings.runJumpBoostDuration);
        state.IsJumpBoosted = false;
    }
}