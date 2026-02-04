using UnityEngine;
using System.Collections;

public class PlayerCamera
{
    private readonly PlayerSettings settings;
    private readonly PlayerState state;
    private readonly Transform cameraTransform;
    private readonly Transform cameraHolder;
    private readonly ICoroutineRunner coroutineRunner;

    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }

    private Coroutine cameraCoroutine;
    private float standingCameraHeight;

    public PlayerCamera(PlayerSettings settings, PlayerState state,
                       Transform cameraTransform, Transform cameraHolder,
                       ICoroutineRunner coroutineRunner)
    {
        this.settings = settings;
        this.state = state;
        this.cameraTransform = cameraTransform;
        this.cameraHolder = cameraHolder;
        this.coroutineRunner = coroutineRunner;

        if (cameraHolder != null)
        {
            standingCameraHeight = cameraHolder.localPosition.y;
        }
    }

    public void HandleMouseLook(float mouseX, float mouseY, Transform playerTransform)
    {
        if (cameraTransform == null) return;

        playerTransform.Rotate(Vector3.up * mouseX * settings.mouseSensitivity);

        state.CameraXRotation -= mouseY * settings.mouseSensitivity;
        state.CameraXRotation = Mathf.Clamp(
            state.CameraXRotation,
            -settings.maxLookAngle,
            settings.maxLookAngle
        );

        cameraTransform.localRotation = Quaternion.Euler(state.CameraXRotation, 0f, 0f);
    }

    public void MoveCameraForCrouch(bool shouldCrouch)
    {
        if (cameraHolder == null) return;

        float targetHeight = shouldCrouch ?
            settings.crouchCameraHeight :
            standingCameraHeight;

        MoveCameraTo(targetHeight);
    }

    private void MoveCameraTo(float targetHeight)
    {
        if (cameraCoroutine != null)
        {
            coroutineRunner.StopCoroutine(cameraCoroutine);
        }

        cameraCoroutine = coroutineRunner.StartCoroutine(
            MoveCameraCoroutine(targetHeight)
        );
    }

    private IEnumerator MoveCameraCoroutine(float targetHeight)
    {
        Vector3 startPos = cameraHolder.localPosition;
        Vector3 endPos = new Vector3(startPos.x, targetHeight, startPos.z);
        float elapsedTime = 0f;

        while (elapsedTime < settings.cameraTransitionTime)
        {
            float t = elapsedTime / settings.cameraTransitionTime;
            cameraHolder.localPosition = Vector3.Lerp(startPos, endPos, SmoothStep(t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraHolder.localPosition = endPos;
        cameraCoroutine = null;
    }

    private float SmoothStep(float t) => t * t * (3f - 2f * t);
}