using UnityEngine;

public class FrontMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float turnSpeed = 15f;
    public Rigidbody myRigidBody;
    private FixedJoint fixedJoint;

    void Start() 
    {
        fixedJoint = GetComponent<FixedJoint>();
    }

    void Update()
    {
        frontMovement();
        inPlace();

        ////////////////////////////////////////////////
        // // Arrow key control
        // float horizontal = 0f;
        // float vertical = 0f;

        // // Check for input (Arrow keys)
        // if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f; // Move left (X-axis)
        // if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f; // Move right (X-axis)
        // if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f; // Move forward (Z-axis)
        // if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f; // Move backward (Z-axis)

        // Vector3 movement = (Vector3.right * horizontal + Vector3.forward * vertical) * walkSpeed * Time.deltaTime;
        // transform.Translate(movement, Space.World);  // Ensures movement is in world space
        ////////////////////////////////////////////////
    }

    void frontMovement()
    {
        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.LeftArrow)) direction += Vector3.left * turnSpeed;
        if (Input.GetKey(KeyCode.RightArrow)) direction += Vector3.right * turnSpeed;
        if (Input.GetKey(KeyCode.UpArrow)) direction += Vector3.forward * walkSpeed;
        if (Input.GetKey(KeyCode.DownArrow)) direction += Vector3.back * walkSpeed;

        direction *= Time.deltaTime;

        transform.Translate(direction, Space.Self);
    }

    // temporary method for developing the backside's swinging motion
    void inPlace()
    {
        if (Input.GetKey(KeyCode.F)) myRigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void checkBackTurn(Vector3 direction)
    {
        if (fixedJoint != null && Input.GetKey(KeyCode.A)) direction -= Vector3.left;

        if (fixedJoint != null && Input.GetKey(KeyCode.A)) direction -= Vector3.right;
    }
}
