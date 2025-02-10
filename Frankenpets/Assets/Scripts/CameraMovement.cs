using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform frontHalf;

    [Header("Camera Details")]
    public int fieldOfViewConnected = 49;
    public int fieldOfViewDisconnected = 120;
    public Vector3 zoomOutPosition = new Vector3(-16.759f, 0.767f, -22.652f);
    public Vector3 zoomInAdjustment = new Vector3(1.0f, 0.8f, 2.25f);

    private FixedJoint fixedJoint;
    private bool zoomOut = false;

    // Update is called once per frame
    void Update()
    {
        fixedJoint = frontHalf.GetComponent<FixedJoint>();

        // If connected, camera zooms in and follows the pet
        if (fixedJoint != null)
        {
            mainCamera.fieldOfView = fieldOfViewConnected;
            mainCamera.transform.position = frontHalf.position + zoomInAdjustment;
        }
        else
        {
            zoomOut = true;
        }
    }

    void FixedUpdate()
    {
        if (zoomOut)
        {
            // If no FixedJoint, zoom out the camera and set its position
            mainCamera.fieldOfView = fieldOfViewDisconnected;
            
            // Move the camera to the specified position
            mainCamera.transform.position = zoomOutPosition;
            zoomOut = false;
        }
    }
}
