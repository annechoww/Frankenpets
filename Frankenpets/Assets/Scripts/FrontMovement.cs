using UnityEngine;

public class Front2Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed

    void Update()
    {
        // Arrow key control
        float horizontal = 0f;
        float vertical = 0f;

        // Check for input (Arrow keys)
        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f; // Move left (X-axis)
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f; // Move right (X-axis)
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f; // Move forward (Z-axis)
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f; // Move backward (Z-axis)

        Vector3 movement = (Vector3.right * horizontal + Vector3.forward * vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);  // Ensures movement is in world space
    }
}
