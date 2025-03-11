using UnityEngine;

/// <summary>
/// Controls the dragging behavior of objects using a virtual tether system.
/// This applies a spring-damper force to pull the object toward a designated grab point.
/// </summary>
public class DragController : MonoBehaviour
{
    [Header("Tether Settings")]
    [Tooltip("How strongly the object is pulled toward the grab point")]
    public float springConstant = 1000f;
    
    [Tooltip("How quickly oscillations are dampened")]
    public float dampingConstant = 50f;
    
    [Tooltip("Maximum force that can be applied to the object")]
    public float maxForce = 2000f;
    
    [Tooltip("Minimum distance before forces are applied (prevents jitter)")]
    public float minDistance = 0.0001f;
    
    [Header("References")]
    [Tooltip("The transform that the object is being pulled toward (usually the mouth)")]
    public Transform targetTransform;
    
    [Tooltip("The point on the object where the force is applied")]
    public Vector3 forceApplicationPoint;
    
    [Header("Debug")]
    public bool showDebugLines = false;
    
    // Runtime variables
    private Rigidbody objectRigidbody;
    private bool isActive = false;
    private Vector3 lastVelocity = Vector3.zero;
    
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        if (objectRigidbody == null)
        {
            Debug.LogError($"DragController on {gameObject.name} requires a Rigidbody component.");
            enabled = false;
        }
    }
    
    /// <summary>
    /// Initialize the drag controller with the target transform (usually the dog's mouth)
    /// </summary>
    /// <param name="target">Transform that the object should be pulled toward</param>
    /// <param name="grabPoint">Local point on the object where the force is applied</param>
    public void Initialize(Transform target, Vector3 grabPoint)
    {
        targetTransform = target;
        forceApplicationPoint = grabPoint;
        isActive = true;
        
        // Store initial velocity to prevent initial jerks
        lastVelocity = objectRigidbody.linearVelocity;
    }
    
    /// <summary>
    /// Stop the dragging behavior
    /// </summary>
    public void StopDragging()
    {
        isActive = false;
    }
    
    /// <summary>
    /// Apply spring-damper forces in FixedUpdate for consistent physics
    /// </summary>
    private void FixedUpdate()
    {
        if (!isActive || targetTransform == null) return;
        
        // Calculate the world position of the force application point on the object
        Vector3 worldForcePoint = transform.TransformPoint(forceApplicationPoint);
        
        // Calculate direction and distance
        Vector3 toTarget = targetTransform.position - worldForcePoint;
        float distance = toTarget.magnitude;
        
        // Only apply forces if beyond minimum distance (prevents jitter)
        if (distance > minDistance)
        {
            Vector3 direction = toTarget.normalized;
            
            // Calculate spring force (F = -kx)
            Vector3 springForce = direction * springConstant * distance;
            
            // Calculate damping force (F = -cv)
            Vector3 relativeVelocity = objectRigidbody.GetPointVelocity(worldForcePoint) - 
                                      (targetTransform.parent ? targetTransform.parent.GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero : Vector3.zero);
            Vector3 dampingForce = -relativeVelocity * dampingConstant;
            
            // Combine forces and clamp to max force
            Vector3 totalForce = springForce + dampingForce;
            if (totalForce.magnitude > maxForce)
            {
                totalForce = totalForce.normalized * maxForce;
            }
            
            // Apply the force at the force application point
            objectRigidbody.AddForceAtPosition(totalForce, worldForcePoint);
            
            // Draw debug lines if enabled
            if (showDebugLines)
            {
                Debug.DrawLine(worldForcePoint, targetTransform.position, Color.yellow);
                Debug.DrawRay(worldForcePoint, springForce / 100f, Color.green);
                Debug.DrawRay(worldForcePoint, dampingForce / 100f, Color.red);
            }
        }
    }
    
    /// <summary>
    /// Configure the dragging parameters based on the object's properties
    /// </summary>
    /// <param name="weight">Object weight value (0-10 scale)</param>
    /// <param name="resistance">Object resistance value (0-1 scale)</param>
    public void ConfigureFromProperties(float springConstant, float dampingConstant, float maxForce)
    {
        this.springConstant = springConstant;
        this.dampingConstant = dampingConstant;
        this.maxForce = maxForce;
    }
    
    /// <summary>
    /// Draw gizmos to visualize the force application point
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!isActive) return;
        
        Gizmos.color = Color.blue;
        Vector3 worldForcePoint = transform.TransformPoint(forceApplicationPoint);
        Gizmos.DrawSphere(worldForcePoint, 0.03f);
        
        if (targetTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(worldForcePoint, targetTransform.position);
        }
    }
}