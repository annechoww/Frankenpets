using UnityEngine;
using UnityEngine.InputSystem;
public class ControllerAssignment: MonoBehaviour
{
    [SerializeField] private PlayerInput player1Input;
    [SerializeField] private PlayerInput player2Input;
    
    [Header("Control Schemes")]
    [SerializeField] private string player1Scheme = "GamepadPlayer1";
    [SerializeField] private string player2Scheme = "GamepadPlayer2";
    [SerializeField] private string keyboardScheme = "Keyboard&Mouse";
    
    [Header("Settings")]
    [SerializeField] private bool useKeyboardFallback = true;
    [SerializeField] private bool reassignOnConnect = true;

    public static ControllerAssignment Instance {get; private set;}
    
    private bool swapped = false;
    private bool finalized = false;
    
    private Gamepad[] gamepads => Gamepad.all.ToArray();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Register for device changes
        InputSystem.onDeviceChange += HandleDeviceChange;
    }
    
    private void Start()
    {
        // Initial assignment
        AssignControllers();
    }
    
    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!finalized && (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) && reassignOnConnect)
        {
            Debug.Log($"Input device change: {device.name} was {change}");
            AssignControllers();
        }
    }
    
    public void AssignControllers()
    {
        Debug.Log($"Found {gamepads.Length} connected gamepads");
        
        // Assign first gamepad to Player 1
        if (gamepads.Length > 0)
        {
            if (swapped && gamepads.Length > 1)
            {
                player1Input.SwitchCurrentControlScheme(player2Scheme, gamepads[1]);
                Debug.Log($"(Swapped) Assigned {gamepads[1].name} to Player 1 (Cat)");
            }
            else
            {
                player1Input.SwitchCurrentControlScheme(player1Scheme, gamepads[0]);
                Debug.Log($"Assigned {gamepads[0].name} to Player 1 (Cat)");
            }
        }
        else if (useKeyboardFallback)
        {
            player1Input.SwitchCurrentControlScheme(keyboardScheme, Keyboard.current, Mouse.current);
            Debug.Log("Using keyboard fallback for Player 1");
        }
        
        // Assign second gamepad to Player 2
        if (gamepads.Length > 1)
        {
            if (swapped)
            {
                player2Input.SwitchCurrentControlScheme(player1Scheme, gamepads[0]);
                Debug.Log($"(Swapped) Assigned {gamepads[0].name} to Player 2 (Dog)");
            }
            else
            {
                player2Input.SwitchCurrentControlScheme(player2Scheme, gamepads[1]);
                Debug.Log($"Assigned {gamepads[1].name} to Player 2 (Dog)");
            }
        }
        else if (useKeyboardFallback)
        {
            player2Input.SwitchCurrentControlScheme(keyboardScheme, Keyboard.current, Mouse.current);
            Debug.Log("Using keyboard fallback for Player 2");
        }
    }

    public void FinalizeAssignment(bool player1ShouldbeCat) {
        if (finalized) {
            Debug.Log("assignment already finalized");
            return;
        }

        if (!player1ShouldbeCat) {
            swapped = true;
        }

        AssignControllers();

        finalized = true;
        Debug.Log("Controller assignments have been finalized");
    }
    // UI uses this to determine gamepad vs. keycaps display
    public bool IsKeyboard()
    {  
        return player1Input.currentControlScheme == keyboardScheme || player2Input.currentControlScheme == keyboardScheme;
    }

    public bool isFinalized() {
        return finalized;
    }
    public bool isSwapped() {
        return swapped;
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= HandleDeviceChange;

        if (Instance == this) {
            Instance = null;
        }
    }
}
