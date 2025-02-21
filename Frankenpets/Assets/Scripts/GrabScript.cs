using UnityEngine;

public class GrabScript : MonoBehaviour
{
    public float maxHitDistance = 50;
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;
    private GameObject grabText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabText = GameObject.FindGameObjectWithTag("GrabText");
    }

    // Update is called once per frame
    void Update()
    {
        // FOR DEBUGGING
        // Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f + Vector3.right * 0.2f); //0.24f
        // Vector3 mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward); //Vector3.forward * 0.34f + 
        // Debug.DrawLine(mouthPosition, Vector3.forward + Vector3.up, Color.red, 2, false);
        // Debug.DrawLine(mouthPosition, mouthDirection, Color.red, 2, false);

        if (Input.GetKeyDown(KeyCode.M) && isGrabbing)
        {
            Debug.Log("Released item");
            grabbableObject.transform.SetParent(null);
            
            grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Physics.IgnoreLayerCollision(10, 9, false);

            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), false);
            // grabbableObject.gameObject.GetComponent<Rigidbody>().detectCollisions = true;

            isGrabbing = false;

        } else if (Input.GetKeyDown(KeyCode.M) && canGrab)
        {
            Debug.Log("Grabbed item");

            ////////////////////////////////////////////////////////
            // CODE FOR FIXED JOINT STUFF BUT CURRENTLY NOT USING 
            // FixedJoint grabbableObjectJoint = grabbableObject.transform.GetChild(0).transform.GetComponent<FixedJoint>();
            // grabbableObjectJoint.connectedBody = transform.GetChild(0).transform.GetComponent<Rigidbody>();
            ////////////////////////////////////////////////////////
            
            Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.34f + Vector3.up * 0.3f + Vector3.right * 0.2f); //0.24f
            Vector3 mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward);

            // Debug.DrawLine(mouthPosition, Vector3.forward + Vector3.up, Color.red, 2, false);
            // Debug.DrawLine(mouthPosition, mouthDirection, Color.red, 2, false);

            // Future: maybe need to Raycast (double hit) to calculate mouth opening angle 
            // if (Physics.Raycast(mouthPosition, grabbableObject.transform.position, out hit, maxHitDistance))
            // {
            //     Debug.DrawLine(mouthPosition, hit.point, Color.red, 2, false);
            //     Debug.Log("Raycast");
            //     grabbableObject.transform.GetChild(0).transform.GetChild
            // } 
            // else 
            // {
            //     Debug.Log("No raycast");
            //     grabbableObject.transform.position = mouthPosition;
            // }
            // grabbableObject.transform.GetChild(0).transform.position = mouthPosition;

            grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Physics.IgnoreLayerCollision(10, 9, true);

            // grabbableObject.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), true);

            grabbableObject.transform.SetParent(transform);
            grabbableObject.transform.position = mouthPosition;

            isGrabbing = true;
            grabText.SetActive(false);
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something is grabbable.");
        if (other.CompareTag("Grabbable") && !isGrabbing)
        {
            grabbableObject = other;
            
            // Show UI Popover
            grabText.transform.position = grabbableObject.transform.position + (Vector3.up * 0.30f);
            grabText.SetActive(true);
            
            canGrab = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Not within grabbable range anymore.");
        if (other.CompareTag("Grabbable"))
        {
            // Hide UI Popover
            grabText.SetActive(false);
            canGrab = false;
        }
    }
}
