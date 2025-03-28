using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // Store player input values
    private Vector2 moveInput;
    private Vector2 cameraMoveInput;
    private bool jumpPressed;
    private bool specialActionPressed;
    private bool reconnectPressed;
    private bool switchPressed;
    private bool controlsMenuPressed;
    private bool glowPressed;
    private bool soundTailPressed;
    private bool respawnPressed;

    // one shot event flags
    private bool soundTailJustPressed;
    private bool glowJustPressed;
    private bool controlsMenuJustPressed;

    private bool jumpJustPressed;

    // Event methods called by PlayerInput component
    public void OnMove(InputAction.CallbackContext context)
    {
        // Read Vector2 value from the input and store it
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        // Read Vector2 value from the input and store it
        cameraMoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // Button just pressed
            jumpJustPressed = true;
            jumpPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            jumpPressed = false;
        }
    }

    public void OnSoundTail(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // Button just pressed
            soundTailJustPressed = true;
            soundTailPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            soundTailPressed = false;
        }
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

    public void OnControlsMenu(InputAction.CallbackContext context)
    {
        // controlsMenuPressed = context.ReadValueAsButton();
        // Debug.Log($"Controls menu pressed: {controlsMenuPressed}");
        if (context.phase == InputActionPhase.Started)
        {
            controlsMenuJustPressed = true;
            controlsMenuPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            controlsMenuPressed = false;
        }
    }

    public void OnGlowPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            glowJustPressed = true;
            glowPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            glowPressed = false;
        }
    }

    public void OnRespawnPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            respawnPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            respawnPressed = false;
        }
    }


    // Accessor methods for PlayerManager to get input values
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public Vector2 GetCameraInput()
    {
        // Dampen the movement due to range of camera
        return cameraMoveInput * 0.005f;
    }

    public bool GetJumpPressed()
    {
        return jumpPressed;
    }

    public bool GetSoundTailPressed()
    {
        return soundTailPressed;
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

    public bool GetGlowPressed()
    {
        return glowPressed;
    }

    public bool GetControlsMenuPressed()
    {
        return controlsMenuPressed;
    }

    public bool GetRespawnPressed()
    {
        return respawnPressed;
    }

    // One-shot getters
    public bool GetSoundTailJustPressed()
    {
        if (soundTailJustPressed)
        {
            soundTailJustPressed = false; // Reset flag after reading
            return true;
        }
        return false;
    }

    public bool GetGlowJustPressed()
    {
        if (glowJustPressed)
        {
            glowJustPressed = false; // Reset flag after reading
            return true;
        }
        return false;
    }


    public bool GetJumpJustPressed()
    {
        if (jumpJustPressed)
        {
            jumpJustPressed = false; // Reset flag after reading
            return true;
        }
        return false;
    }

    public bool GetControlsMenuJustPressed()
    {
        if (controlsMenuJustPressed)
        {
            controlsMenuJustPressed = false; // Reset flag after reading
            return true;
        }
        return false;
    }
}
