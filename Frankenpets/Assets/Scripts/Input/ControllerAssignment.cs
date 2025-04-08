using UnityEngine;
using UnityEngine.InputSystem;

// Static helper class that allows other scripts to check input state
// without needing a reference to the ControllerAssignment instance
public static class InputHelper
{
    public static bool IsKeyboardActive()
    {
        return PlayerPrefs.GetInt("UsingKeyboard", 0) == 1;
    }
    
    public static void SetKeyboardActive(bool isActive)
    {
        PlayerPrefs.SetInt("UsingKeyboard", isActive ? 1 : 0);
        PlayerPrefs.Save();
    }
}

public class ControllerAssignment : MonoBehaviour
{
    [SerializeField] private PlayerInput player1Input; // Cat
    [SerializeField] private PlayerInput player2Input; // Dog
    
    [Header("Control Schemes")]
    [SerializeField] private string player1Scheme = "GamepadPlayer1"; // Cat's scheme
    [SerializeField] private string player2Scheme = "GamepadPlayer2"; // Dog's scheme
    [SerializeField] private string keyboardScheme = "Keyboard"; // "Keyboard&Mouse"
    
    [Header("Settings")]
    [SerializeField] private bool useKeyboardFallback = true;
    [SerializeField] private bool reassignOnConnect = true;
    
    // Track initialization for other scripts to check
    private bool isInitialized = false;
    
    // Helper to get all connected gamepads
    private Gamepad[] gamepads => Gamepad.all.ToArray();

    private void Awake()
    {
        // Register for device connection/disconnection events
        InputSystem.onDeviceChange += HandleDeviceChange;
    }
    
    private void Start()
    {
        // Initial assignment of controllers
        AssignControllers();
        isInitialized = true;
    }

    public bool IsInitialized() {
        return isInitialized;
    }
    
    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if ((change == InputDeviceChange.Added || change == InputDeviceChange.Removed) && reassignOnConnect)
        {
            Debug.Log($"Input device change: {device.name} was {change}");
            AssignControllers();
        }
    }
    
    public void AssignControllers()
    {
        Debug.Log($"Found {gamepads.Length} connected gamepads");
        
        bool usingKeyboard = false;
        
        // Player 1 (Cat) always uses the controller with Player 1 light (first in array)
        if (gamepads.Length > 0)
        {
            player1Input.SwitchCurrentControlScheme(player1Scheme, gamepads[0]);
            Debug.Log($"Assigned {gamepads[0].name} to Player 1 (Cat)");
        }
        else if (useKeyboardFallback)
        {
            player1Input.SwitchCurrentControlScheme(keyboardScheme, Keyboard.current, Mouse.current);
            Debug.Log("Using keyboard fallback for Player 1 (Cat)");
            usingKeyboard = true;
        }
        
        // Player 2 (Dog) always uses the controller with Player 2 light (second in array)
        if (gamepads.Length > 1)
        {
            player2Input.SwitchCurrentControlScheme(player2Scheme, gamepads[1]);
            Debug.Log($"Assigned {gamepads[1].name} to Player 2 (Dog)");
        }
        else if (useKeyboardFallback)
        {
            player2Input.SwitchCurrentControlScheme(keyboardScheme, Keyboard.current, Mouse.current);
            Debug.Log("Using keyboard fallback for Player 2 (Dog)");
            usingKeyboard = true;
        }
        
        // Update the keyboard status for other scripts to reference
        InputHelper.SetKeyboardActive(usingKeyboard);
    }
    
    // UI uses this to determine gamepad vs. keycaps display
    public bool IsKeyboard()
    {  
        bool isKeyboard = (player1Input?.currentControlScheme == keyboardScheme || 
                          player2Input?.currentControlScheme == keyboardScheme);
        InputHelper.SetKeyboardActive(isKeyboard);
        return isKeyboard;
    }

    private void OnDestroy() {
        // Unregister event handler to prevent memory leaks
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }
}