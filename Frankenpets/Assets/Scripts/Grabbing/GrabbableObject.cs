using UnityEngine;

/// <summary>
/// Attach this component to objects that can be grabbed directly.
/// </summary>
public class GrabbableObject : MonoBehaviour
{
    public enum GrabBehavior
    {
        Draggable,  // Object stays on ground and is dragged (like a rug)
        Portable    // Object moves to mouth and is carried (like a book)
    }
    
    [Tooltip("Whether this object is draggable or portable")]
    public GrabBehavior grabBehavior = GrabBehavior.Portable; // Default to portable for direct grabbables
    
    [Tooltip("How heavy this object feels when grabbed (0=Light, 10=Very Heavy)")]
    [Range(0, 10)]
    public float grabWeight = 2f;
    
    [Tooltip("How much this object slows down the player when grabbed (0-1)")]
    [Range(0, 1)]
    public float movementPenalty = 0.2f;
    
    [Tooltip("How much the object resists being moved (0-1)")]
    [Range(0, 1)]
    public float dragResistance = 0.3f;

    [Header("Movement Restrictions")]
    [Tooltip("How much the object restricts turning (0=No restriction, 1=Cannot turn)")]
    [Range(0, 1)]
    public float turnRestriction = 0f;

    [Tooltip("Should this object completely prevent turning when grabbed?")]
    public bool preventTurning = false;
    
    [Header("Virtual Tether Settings (For Draggable Objects)")]
    [Tooltip("Spring constant for the virtual tether (higher = stiffer connection)")]
    public float springConstant = 1000f;
    
    [Tooltip("Damping constant for the virtual tether (higher = less oscillation)")]
    public float dampingConstant = 50f;
    
    [Tooltip("Maximum force that can be applied to the object via tether")]
    public float maxTetherForce = 2000f;
    
    private Rigidbody objectRigidbody;
    
    private void Awake()
    {
        // Get the rigidbody
        objectRigidbody = GetComponent<Rigidbody>();
        
        // Make sure this object has a rigidbody
        if (objectRigidbody == null)
        {
            Debug.LogWarning($"GrabbableObject on {gameObject.name} has no Rigidbody. Adding one.");
            objectRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        // Make sure it has a collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"GrabbableObject on {gameObject.name} has no Collider. Add a collider for it to be grabbable.");
        }
        
        // Make sure it's tagged properly
        if (!CompareTag("Grabbable") && !CompareTag("Draggable"))
        {
            Debug.LogWarning($"GrabbableObject on {gameObject.name} should be tagged as 'Grabbable' or 'Draggable'.");
            
            // Auto-tag based on behavior
            if (grabBehavior == GrabBehavior.Draggable)
            {
                gameObject.tag = "Draggable";
            }
            else
            {
                gameObject.tag = "Grabbable";
            }
            
            Debug.LogWarning($"Auto-tagged {gameObject.name} as '{gameObject.tag}'.");
        }
    }
    
    /// <summary>
    /// Configures a joint when this object is grabbed
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
        if (objectRigidbody != null)
        {
            objectRigidbody.linearDamping = dragResistance * 10f; // Scale to useful range
        }
    }
}