using UnityEngine;

public class BackMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float turnSpeed = 15f;

    void Update()
    {
        backMovement();
        
        ////////////////////////////////////////////////
        // // WASD control
        // float horizontal = 0f;
        // float vertical = 0f;

        // // Check for input (WASD)
        // if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        // if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        // if (Input.GetKey(KeyCode.W)) vertical = 1f;
        // if (Input.GetKey(KeyCode.S)) vertical = -1f;

        // Vector3 movement = (Vector3.right * horizontal + Vector3.forward * vertical) * walkSpeed * Time.deltaTime;
        // transform.Translate(movement, Space.World);  // Ensures movement is in world space
        ////////////////////////////////////////////////
    }

    void backMovement()
    {
        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.A)) direction += Vector3.left * turnSpeed;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right * turnSpeed;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward * walkSpeed;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back * walkSpeed;

        direction *= Time.deltaTime;

        transform.Translate(direction, Space.Self); 
    }
}
