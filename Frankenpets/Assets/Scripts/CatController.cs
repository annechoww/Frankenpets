using UnityEngine;
using UnityEngine.InputSystem;

public class CatController : MonoBehaviour, CatControls.IGameplayActions
{
    [Header("Climbing Settings")]
    public float climbSpeed = 2f;
    public float climbCheckDistance = 0.5f;
    
    private Rigidbody rb;
    private bool isNearClimbable = false;
    private bool isClimbing = false;
    private Vector3 climbDirection;
    private CatControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new CatControls();
        
        // Subscribe to input events through the interface
        controls.Gameplay.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void OnClimb(InputAction.CallbackContext context)
    {
        // Start climbing when button is pressed
        if (context.started && isNearClimbable)
        {
            StartClimbing();
        }
        // Stop climbing when button is released
        else if (context.canceled && isClimbing)
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        // Zero out current velocities
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Freeze rotation while climbing
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            // Apply climbing movement in FixedUpdate for smooth physics
            rb.linearVelocity = climbDirection * climbSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearClimbable = true;
            // Get the surface normal to determine climb direction
            if (Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, climbCheckDistance))
            {
                // Project the up vector onto the surface plane to get the climb direction
                climbDirection = Vector3.ProjectOnPlane(Vector3.up, hit.normal).normalized;
            }
            else
            {
                climbDirection = Vector3.up;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearClimbable = false;
            if (isClimbing)
            {
                StopClimbing();
            }
        }
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }
}