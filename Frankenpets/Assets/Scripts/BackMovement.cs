using UnityEngine;

public class BackMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed

    void Update()
    {
        // WASD control
        float horizontal = 0f;
        float vertical = 0f;

        // Check for input (WASD)
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        if (Input.GetKey(KeyCode.W)) vertical = 1f;
        if (Input.GetKey(KeyCode.S)) vertical = -1f;

        Vector3 movement = (Vector3.right * horizontal + Vector3.forward * vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);  // Ensures movement is in world space
    }
}
