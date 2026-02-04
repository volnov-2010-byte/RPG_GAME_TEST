using UnityEngine;

public class PlayerState
{
    // Основные состояния
    public bool IsGrounded { get; set; }
    public bool IsRunning { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsJumpBoosted { get; set; }

    // Физика
    public Vector3 Velocity { get; set; }
    public Vector3 HorizontalVelocity { get; set; }

    // Скорости
    public float CurrentSpeed { get; set; }

    // Вращение камеры
    public float CameraXRotation { get; set; }

    public PlayerState()
    {
        Velocity = Vector3.zero;
        HorizontalVelocity = Vector3.zero;
        CurrentSpeed = 5f; // Начальная скорость
    }
}