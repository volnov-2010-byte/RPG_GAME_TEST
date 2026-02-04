using UnityEngine;

public class PlayerInputHandler
{
    public float GetHorizontal() => Input.GetAxis("Horizontal");
    public float GetVertical() => Input.GetAxis("Vertical");
    public bool GetJumpDown() => Input.GetButtonDown("Jump");
    public bool GetCrouchDown() => Input.GetKeyDown(KeyCode.LeftControl);
    public bool GetRun() => Input.GetKey(KeyCode.LeftShift);
    public float GetMouseX() => Input.GetAxis("Mouse X");
    public float GetMouseY() => Input.GetAxis("Mouse Y");
}
