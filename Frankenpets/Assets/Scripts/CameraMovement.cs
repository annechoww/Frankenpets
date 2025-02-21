using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Camera p1Camera;
    public Camera p2Camera;
    public Transform frontHalf;

    private FixedJoint fixedJoint;
    private bool splitScreen = false;

    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        p1Camera.gameObject.SetActive(false);
        p2Camera.gameObject.SetActive(false);
    }

    void Update()
    {
        fixedJoint = frontHalf.GetComponent<FixedJoint>();
        
        if (fixedJoint != null)
        {
            mainCamera.gameObject.SetActive(true);
            p1Camera.gameObject.SetActive(false);
            p2Camera.gameObject.SetActive(false);
        }
        else
        {
            splitScreen = true;
        }
    }

    void FixedUpdate()
    {
        if (splitScreen)
        {
            mainCamera.gameObject.SetActive(false);
            p1Camera.gameObject.SetActive(true);
            p2Camera.gameObject.SetActive(true);
            splitScreen = false;
        }
    }
}
