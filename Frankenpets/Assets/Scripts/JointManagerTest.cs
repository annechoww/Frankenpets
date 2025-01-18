using UnityEngine;

public class JointManagerTest : MonoBehaviour
{
    [Tooltip("Reference to the front half of the pet.")]
    public Transform frontHalf;

    [Tooltip("Reference to the bottom half of the pet.")]
    public Transform bottomHalf;

    [Tooltip("Maximum distance within which reconnection can occur.")]
    public float reconnectionDistance = 3f;

    [Tooltip("Key used to toggle connection/disconnection.")]
    public KeyCode toggleKey = KeyCode.Space;

    private FixedJoint fixedJoint;

    // Store initial relative position and rotation
    private Vector3 initialRelativePosition;
    private Quaternion initialRelativeRotation;

    // Store original joint anchor configuration
    private Vector3 originalAnchor;
    private Vector3 originalConnectedAnchor;
    private bool hasStoredOriginalConfig = false;

    void Start()
    {
        // Store the initial relative transform between halves
        initialRelativePosition = bottomHalf.position - frontHalf.position;
        initialRelativeRotation = Quaternion.Inverse(frontHalf.rotation) * bottomHalf.rotation;

        // Attempt to get an existing FixedJoint on frontHalf at startup
        fixedJoint = frontHalf.GetComponent<FixedJoint>();
        if (fixedJoint != null)
        {
            // Store original joint anchor configurations
            originalAnchor = fixedJoint.anchor;
            fixedJoint.autoConfigureConnectedAnchor = false;
            originalConnectedAnchor = fixedJoint.connectedAnchor;
            hasStoredOriginalConfig = true;
        }
    }

    void Update()
    {
        // Check if the toggle key (Space) is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            // If currently connected, disconnect the halves
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
                fixedJoint = null;
                Debug.Log("Halves disconnected.");
            }
            // If currently disconnected, attempt to reconnect if close enough
            else
            {
                float distance = Vector3.Distance(frontHalf.position, bottomHalf.position);
                if (distance < reconnectionDistance && hasStoredOriginalConfig)
                {
                    // Snap bottomHalf back to its original relative position and rotation
                    bottomHalf.position = frontHalf.position + initialRelativePosition;
                    bottomHalf.rotation = frontHalf.rotation * initialRelativeRotation;

                    // Recreate the FixedJoint on frontHalf and configure it
                    fixedJoint = frontHalf.gameObject.AddComponent<FixedJoint>();
                    Rigidbody bottomRb = bottomHalf.GetComponent<Rigidbody>();
                    if (bottomRb != null)
                    {
                        fixedJoint.connectedBody = bottomRb;
                        fixedJoint.autoConfigureConnectedAnchor = false;
                        fixedJoint.anchor = originalAnchor;
                        fixedJoint.connectedAnchor = originalConnectedAnchor;
                    }
                    Debug.Log("Halves reconnected.");
                }
                else
                {
                    Debug.Log("Halves too far to reconnect or original config not stored.");
                }
            }
        }
    }
}
