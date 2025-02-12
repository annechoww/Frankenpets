using UnityEngine;

public class GrabScript : MonoBehaviour
{
    public float maxHitDistance = 50;
    public float sphereRadius = 100;
    private Vector3 boxContraints;
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxContraints = GetComponent<BoxCollider>().size;
        boxContraints += new Vector3(5, 5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouthPosition1 = transform.position + transform.TransformDirection(Vector3.forward * 0.24f + Vector3.up * 0.3f + Vector3.right * 0.2f);
        Vector3 mouthPositionEnd = mouthPosition1 + Vector3.forward;

        // Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward * 0.24f + Vector3.up * 0.3f + Vector3.right * 0.2f), Color.red, 2, false);
        // Debug.DrawLine(mouthPosition1, mouthPositionEnd, Color.red, 2, false);

        if (Input.GetKeyDown(KeyCode.M) && isGrabbing)
        {
            Debug.Log("Released item");
            grabbableObject.transform.SetParent(null);
            grabbableObject.GetComponent<Rigidbody>().isKinematic = false;

            isGrabbing = false;

        } else if (Input.GetKeyDown(KeyCode.M) && canGrab)
        {
            Debug.Log("Grabbed item");

            // FixedJoint grabbableObjectJoint = grabbableObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<FixedJoint>();
            // grabbableObjectJoint.connectedBody = transform.GetChild(0).transform.GetComponent<Rigidbody>();

            // Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.24f + Vector3.up * 0.3f + Vector3.right * 0.2f);
            Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.54f + Vector3.up * 0.3f + Vector3.right * 0.2f);
            Vector3 mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward);

            // Debug.DrawLine(mouthPosition, Vector3.forward + Vector3.up, Color.red, 2, false);
            // Debug.DrawLine(mouthPosition, mouthDirection, Color.red, 2, false);


            // if (Physics.BoxCast(mouthPosition, boxContraints, mouthDirection, out hit, Quaternion.identity, maxHitDistance))
            // {
            //     Debug.DrawLine(mouthPosition, hit.point, Color.green, 2, false);
            //     Debug.Log("Boxcast");

            // } else {

            // }

            grabbableObject.transform.SetParent(transform);

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
            grabbableObject.transform.position = mouthPosition;
            grabbableObject.GetComponent<Rigidbody>().isKinematic = true;

            isGrabbing = true;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something is grabbable.");
        if (other.CompareTag("Grabbable"))
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
