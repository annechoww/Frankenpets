using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // Store player input values
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool specialActionPressed;
    private bool reconnectPressed;
    private bool switchPressed;

    // Event methods called by PlayerInput component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Read Vector2 value from the input and store it
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Get boolean value (pressed/released) from the input
        jumpPressed = context.ReadValueAsButton();
        Debug.Log($"Jump pressed: {jumpPressed}");
    }

    public void OnSpecialAction(InputAction.CallbackContext context)
    {
        specialActionPressed = context.ReadValueAsButton();
        Debug.Log($"Special action pressed: {specialActionPressed}");
    }

    public void OnReconnect(InputAction.CallbackContext context)
    {
        reconnectPressed = context.ReadValueAsButton();
        Debug.Log($"Reconnect pressed: {reconnectPressed}");
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        switchPressed = context.ReadValueAsButton();
        Debug.Log($"Switch pressed: {switchPressed}");
    }

    // Accessor methods for PlayerManager to get input values
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool GetJumpPressed()
    {
        return jumpPressed;
    }

    public bool GetSpecialActionPressed()
    {
        return specialActionPressed;
    }

    public bool GetReconnectPressed()
    {
        return reconnectPressed;
    }

    public bool GetSwitchPressed()
    {
        return switchPressed;
    }
}