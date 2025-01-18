using UnityEngine;

public class JointManager : MonoBehaviour
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

    void Start()
    {
        // Attempt to get an existing FixedJoint on frontHalf at startup
        fixedJoint = frontHalf.GetComponent<FixedJoint>();
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
                if (distance < reconnectionDistance)
                {
                    fixedJoint = frontHalf.gameObject.AddComponent<FixedJoint>();
                    Rigidbody bottomRb = bottomHalf.GetComponent<Rigidbody>();
                    if (bottomRb != null)
                    {
                        fixedJoint.connectedBody = bottomRb;
                    }
                    Debug.Log("Halves reconnected.");
                }
                else
                {
                    Debug.Log("Halves too far to reconnect.");
                }
            }
        }
    }
}
