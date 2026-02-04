using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
    [Header("Основные скорости")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;

    [Header("Модификаторы движения")]
    public float strafeMultiplier = 0.8f;
    public float backwardMultiplier = 0.6f;

    [Header("Прыжок")]
    public float jumpHeight = 1f;
    public float gravity = -9.81f;
    public float runJumpBoost = 1.2f;
    public float runJumpForwardBoost = 1.5f;
    public float runJumpBoostDuration = 0.3f;

    [Header("Камера")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    public float crouchCameraHeight = -0.4f;
    public float cameraTransitionTime = 0.15f;

    [Header("Проверки")]
    public float groundCheckDistance = 0.2f;
}