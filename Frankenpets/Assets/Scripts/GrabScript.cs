using UnityEngine;

public class GrabScript : MonoBehaviour
{
    public float maxHitDistance = 50;
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && isGrabbing)
        {
            Debug.Log("Released item");
            grabbableObject.transform.SetParent(null);
            
            grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), false);
            // transform.GetComponent<Rigidbody>().detectCollisions = true;
            Physics.IgnoreLayerCollision(8, 9, false);
            // grabbableObject.gameObject.GetComponent<Rigidbody>().detectCollisions = true;


            // grabbableObject.gameObject.layer = 8;
            // grabbableObject.gameObject.AddComponent<MeshCollider>();
            // grabbableObject.gameObject.GetComponent<MeshCollider>().convex = true;
            // try adding cat as layer ovvertide

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
            // Draw a line protruding from the mouth of the dog
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
                        Physics.IgnoreLayerCollision(8, 9, true);

            // grabbableObject.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), true);
            // transform.GetComponent<Rigidbody>().detectCollisions = false;
            // grabbableObject.gameObject.layer = 0;

            // Destroy(grabbableObject.gameObject.GetComponent<MeshCollider>());
            grabbableObject.transform.SetParent(transform);
            grabbableObject.transform.position = mouthPosition;

            isGrabbing = true;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something is grabbable.");
        if (other.CompareTag("Grabbable") && !isGrabbing)
        {
            grabbableObject = other;
            canGrab = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Not within grabbable range anymore.");
        if (other.CompareTag("Grabbable"))
        {
            canGrab = false;
        }
    }
}
