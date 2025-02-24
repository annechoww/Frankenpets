using UnityEngine;

public class SelfRighting : MonoBehaviour
{
    [Header("References")]
    public Transform petRoot;
    public Rigidbody petRigidbody;

    [Header("Orientation Settings")]
    public float maxUprightAngle = 45f;
    public float checkInterval = 0.5f;
    public float rotationSpeed = 1f;

    private float timer = 0f;

    void Start()
    {
        if (petRigidbody == null)
        {
            petRigidbody = petRoot.GetComponent<Rigidbody>();
            if (petRigidbody == null)
            {
                Debug.LogError("SelfRighting: No Rigidbody found on petRoot.");
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckOrientation();
        }
    }

    void CheckOrientation()
    {
        // Determine current tilt relative to desired upright direction
        float angle = Vector3.Angle(petRoot.up, Vector3.up);
        if (angle > maxUprightAngle)
        {
            RightPet();
        }
    }

    void RightPet()
    {
        // Define the target upright rotation as X:0, Y:0, Z:90
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);

        // Smoothly interpolate from the current rotation to the target rotation
        Quaternion newRotation = Quaternion.Slerp(petRigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Apply the new rotation in a physics-friendly manner
        petRigidbody.MoveRotation(newRotation);

        Debug.Log("SelfRighting: Rotating pet toward (0,0,90) orientation.");
    }
}
