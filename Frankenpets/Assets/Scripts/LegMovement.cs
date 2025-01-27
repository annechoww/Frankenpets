using UnityEngine;

public class LegMovement : MonoBehaviour
{
    public Transform frontLeftTarget;
    public Transform frontRightTarget;
    public Transform backLeftTarget;
    public Transform backRightTarget;

    public Transform body; // The body of the cat to track its movement

    public float stepDistance = 0.5f; // How far a leg moves before stepping
    public float stepHeight = 0.2f; // How high the leg lifts
    public float stepSpeed = 5f; // Speed of the leg movement
    private Vector3[] defaultPositions; // Default positions for each leg
    private bool[] isStepping; // Tracks if a leg is stepping

    void Start()
    {
        // Save default positions for the leg targets
        defaultPositions = new Vector3[4];
        defaultPositions[0] = frontLeftTarget.position;
        defaultPositions[1] = frontRightTarget.position;
        defaultPositions[2] = backLeftTarget.position;
        defaultPositions[3] = backRightTarget.position;

        // Initialize stepping states
        isStepping = new bool[4];
    }

    void Update()
    {
        // Update the procedural movement for each leg
        UpdateLeg(frontLeftTarget, 0);
        UpdateLeg(frontRightTarget, 1);
        UpdateLeg(backLeftTarget, 2);
        UpdateLeg(backRightTarget, 3);
    }

    void UpdateLeg(Transform target, int legIndex)
    {
        // Calculate the desired position for the leg
        Vector3 desiredPosition = defaultPositions[legIndex];
        desiredPosition += body.TransformDirection(Vector3.forward) * Mathf.Sin(Time.time * stepSpeed + (legIndex * Mathf.PI / 2)) * stepDistance;

        // Check if the leg needs to take a step
        if (!isStepping[legIndex] && Vector3.Distance(target.position, desiredPosition) > stepDistance)
        {
            isStepping[legIndex] = true;
            StartCoroutine(StepLeg(target, legIndex, desiredPosition));
        }
    }

    System.Collections.IEnumerator StepLeg(Transform target, int legIndex, Vector3 desiredPosition)
    {
        Vector3 startPosition = target.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * stepSpeed;

            // Interpolate position and add height for a stepping arc
            Vector3 position = Vector3.Lerp(startPosition, desiredPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * stepHeight;

            target.position = position;
            yield return null;
        }

        // Finish the step
        target.position = desiredPosition;
        isStepping[legIndex] = false;
    }
}
