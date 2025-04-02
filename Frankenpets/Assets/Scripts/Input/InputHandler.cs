using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InputHandler : MonoBehaviour
{
    public bool rumbleEnabled = true; // Flag to enable/disable rumble
    private bool isRumbling = false; // Flag to check if rumble is active
    private Gamepad activeGamepad = null; // Reference to the active gamepad
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
    private bool glowPressedLastFrame;
    private bool specialActionPressedLastFrame;
    private bool controlsMenuJustPressed;
    private bool jumpPressedLastFrame;

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
        if (context.phase == InputActionPhase.Started)
        {
            specialActionPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            specialActionPressed = false;
        }
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

    // Rumble
    public void TriggerRumble(float lowFrequency, float highFrequency, float duration)
    {
        if (!rumbleEnabled) return;

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            return;
            
        foreach (var device in playerInput.devices)
        {
            if (device is Gamepad gamepad)
            {
                // Set rumble
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
                
                // Schedule turning it off after duration
                StartCoroutine(StopRumbleAfterDuration(gamepad, duration));
                break; // Only need to rumble one gamepad per player
            }
        }
    }

    private IEnumerator StopRumbleAfterDuration(Gamepad gamepad, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (gamepad != null)
            gamepad.SetMotorSpeeds(0f, 0f);

    }

    public void StartContinuousRumble(float lowFrequency, float highFrequency) {
        if (!rumbleEnabled) return;

        // Stop any existing rumble first
        StopRumble();
        
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            return;
            
        foreach (var device in playerInput.devices)
        {
            if (device is Gamepad gamepad)
            {
                // Store the gamepad reference
                activeGamepad = gamepad;
                
                // Set rumble
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
                isRumbling = true;
                
                // Debug log
                Debug.Log($"Started continuous rumble at {lowFrequency}/{highFrequency}");
                break; // Only need to rumble one gamepad per player
            }
        }
    }

    public void StopRumble()
    {
        if (isRumbling && activeGamepad != null)
        {
            activeGamepad.SetMotorSpeeds(0f, 0f);
            isRumbling = false;
            Debug.Log("Stopped rumble");
        }
    }

    private void OnDisable()
    {
        StopRumble();
    }

    private void OnDestroy()
    {
        StopRumble();
    }

    // If player toggles rumble off, stop any active rumble
    public void SetRumbleEnabled(bool enabled)
    {
        rumbleEnabled = enabled;
        if (!rumbleEnabled)
        {
            StopRumble();
        }
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
        bool justPreseed = glowPressed && !glowPressedLastFrame;
        glowPressedLastFrame = glowPressed;
        return justPreseed;

    }


    public bool GetJumpJustPressed()
    {
        bool justPreseed = jumpPressed && !jumpPressedLastFrame;
        jumpPressedLastFrame = jumpPressed;
        return justPreseed;
    }

    public bool GetSpecialActionJustPressed()
    {
        bool justPressed = specialActionPressed && !specialActionPressedLastFrame;
        specialActionPressedLastFrame = specialActionPressed;
        return justPressed;
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

    // Reset methods
    public void ResetJumpState()
    {
        jumpPressedLastFrame = jumpPressed;
    }
}
