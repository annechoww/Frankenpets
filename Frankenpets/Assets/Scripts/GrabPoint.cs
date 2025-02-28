using UnityEngine;

/// <summary>
/// Attach this component to child objects that serve as grab points.
/// </summary>
public class GrabPoint : MonoBehaviour
{
    public enum GrabBehavior
    {
        Draggable,  // Object stays on ground and is dragged (like a rug)
        Portable    // Object moves to mouth and is carried (like a book)
    }
    
    [Tooltip("The parent object's Rigidbody that will be affected when this point is grabbed")]
    public Rigidbody parentRigidbody;
    
    [Tooltip("Whether this object is draggable or portable")]
    public GrabBehavior grabBehavior = GrabBehavior.Draggable;
    
    [Tooltip("How heavy this object feels when grabbed (0=Light, 10=Very Heavy)")]
    [Range(0, 10)]
    public float grabWeight = 5f;
    
    [Tooltip("How much this object slows down the player when grabbed (0-1)")]
    [Range(0, 1)]
    public float movementPenalty = 0.5f;
    
    [Tooltip("How much the object resists being moved (0-1)")]
    [Range(0, 1)]
    public float dragResistance = 0.5f;
    
    private void Awake()
    {
        // Auto-find parent rigidbody if not set
        if (parentRigidbody == null && transform.parent != null)
        {
            parentRigidbody = transform.parent.GetComponent<Rigidbody>();
            
            if (parentRigidbody == null)
            {
                Debug.LogWarning($"GrabPoint on {gameObject.name} could not find a Rigidbody on its parent. Please assign one manually.");
            }
        }
        
        // Make sure this object has a collider and is properly tagged
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"GrabPoint on {gameObject.name} has no Collider. Add a collider for it to be grabbable.");
        }
    }
    
    /// <summary>
    /// Configures a joint when this grab point is grabbed
    /// </summary>
    public void ConfigureJoint(ConfigurableJoint joint)
    {
        if (joint == null) return;
        
        // Base spring and damper values
        float baseSpring = 1000f;
        float baseDamper = 20f;
        
        // Adjust based on weight
        float springForce = baseSpring * (1f - (grabWeight / 15f)); // Heavier = less spring
        float damperForce = baseDamper + (grabWeight * 5f); // Heavier = more damping
        
        // Create drive settings
        JointDrive drive = new JointDrive
        {
            positionSpring = springForce,
            positionDamper = damperForce,
            maximumForce = float.MaxValue
        };
        
        // Apply to all axes
        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;
        
        // Configure based on grabBehavior
        if (grabBehavior == GrabBehavior.Draggable)
        {
            // Draggable objects (like rugs) stay on the ground
            
            // Reduce vertical spring force to keep on ground
            JointDrive yDrive = drive;
            yDrive.positionSpring = springForce * 0.5f;
            joint.yDrive = yDrive;
            
            // Limit vertical movement
            SoftJointLimit yLimit = new SoftJointLimit();
            yLimit.limit = 0.1f;
            joint.linearLimit = yLimit;
            
            // Apply the limit
            joint.yMotion = ConfigurableJointMotion.Limited;
            
            // Allow rotation for draggable objects
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;
        }
        else // Portable
        {
            // Portable objects (like books) are carried in mouth
            
            // Stronger connection for carried objects
            JointDrive strongDrive = new JointDrive
            {
                positionSpring = springForce * 1.5f, // Stronger spring
                positionDamper = damperForce,
                maximumForce = float.MaxValue
            };
            
            joint.xDrive = strongDrive;
            joint.yDrive = strongDrive;
            joint.zDrive = strongDrive;
            
            // Restrict rotation for carried objects to keep them oriented properly
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;
            
            joint.angularXDrive = strongDrive;
            joint.angularYZDrive = strongDrive;
        }
        
        // Apply drag resistance
        if (parentRigidbody != null)
        {
            parentRigidbody.linearDamping = dragResistance * 10f; // Scale to useful range
        }
    }
    
    private void OnDrawGizmos()
    {
        // Visualize grab points in the editor - different colors based on behavior
        Gizmos.color = grabBehavior == GrabBehavior.Draggable ? Color.yellow : Color.green;
        Gizmos.DrawSphere(transform.position, 0.05f);
        
        // Draw line to parent to show connection
        if (transform.parent != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.parent.position);
        }
    }
}