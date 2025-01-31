using UnityEngine;

public class DogController : MonoBehaviour
{
    public Transform backHalf;
    public Rigidbody rb;

    private Quaternion initialRotation;
    private Quaternion standingRotation;

    public float standRotationAngle = 150f;
    public float rotationSpeed = 5f;
    
    private bool isStanding = false;


    void Start()
    {
        rb = backHalf.GetComponent<Rigidbody>();

        initialRotation = rb.rotation;
        standingRotation = Quaternion.Euler(-standRotationAngle, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isStanding = true;
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            isStanding = false;
        }
    }

    void FixedUpdate()
    {
        if (isStanding)
        {
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, standingRotation, Time.fixedDeltaTime * rotationSpeed));
        }
        else
        {
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, initialRotation, Time.fixedDeltaTime * rotationSpeed));
        }
    }
}
