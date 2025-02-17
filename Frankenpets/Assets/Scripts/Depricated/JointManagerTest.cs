using UnityEngine;

public class JointManagerTest : MonoBehaviour
{
    [Header("References")]
    public Transform frontHalf;
    public Transform bottomHalf;
    public Transform frontMagnet;
    public Transform bottomMagnet;

    [Header("Settings")]
    public float reconnectionDistance = 1.0f;
    public KeyCode toggleKey = KeyCode.Space;

    private FixedJoint fixedJoint;
    private Quaternion initialRelativeRotation;
    private bool shouldReconnect = false;

    void Start()
    {
        initialRelativeRotation = Quaternion.Inverse(frontHalf.rotation) * bottomHalf.rotation;
        fixedJoint = frontHalf.GetComponent<FixedJoint>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
                fixedJoint = null;
                Debug.Log("Halves disconnected.");
            }
            else
            {
                // Flag that we want to attempt reconnection on the next FixedUpdate
                shouldReconnect = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (shouldReconnect)
        {
            float distance = Vector3.Distance(frontMagnet.position, bottomMagnet.position);
            if (distance < reconnectionDistance)
            {
                // Temporarily disable bottomHalf physics
                Rigidbody bottomRb = bottomHalf.GetComponent<Rigidbody>();
                bool originalKinematic = bottomRb.isKinematic;
                bottomRb.isKinematic = true;

                // Align orientation and position
                bottomHalf.rotation = frontHalf.rotation * initialRelativeRotation;
                Vector3 positionOffset = frontMagnet.position - bottomMagnet.position;
                bottomHalf.position += positionOffset;

                // Re-enable physics
                bottomRb.isKinematic = originalKinematic;

                // Re-establish joint
                fixedJoint = frontHalf.gameObject.AddComponent<FixedJoint>();
                if (bottomRb != null)
                {
                    fixedJoint.connectedBody = bottomRb;
                }

                Debug.Log("Halves reconnected along magnet faces.");
                shouldReconnect = false;  // reset flag after reconnection
            }
            else
            {
                Debug.Log("Magnet points are not close enough to reconnect.");
                shouldReconnect = false;  // reset flag after failed reconnection
            }
        }
    }
}
