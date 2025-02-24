using UnityEngine;

public class GrabScript : MonoBehaviour
{
    // public float maxHitDistance = 50;
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;
    private GameObject grabText;
    private Vector3 mouthPosition;
    private Vector3 mouthDirection;
    private FixedJoint draggableJoint;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabText = GameObject.FindGameObjectWithTag("GrabText");
    }

    // Update is called once per frame
    void Update()
    {
        // FOR DEBUGGING: Make sure mouthPosition and mouthDirection match the one at line 54, and comment out the variables on lines 54 & 55
        // Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
        // Vector3 mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward); //Vector3.forward * 0.34f + 
        // Debug.DrawLine(mouthPosition, Vector3.forward + Vector3.up, Color.red, 2, false);
        // Debug.DrawLine(mouthPosition, mouthDirection, Color.red, 2, false);

        if (Input.GetKeyDown(KeyCode.M) && isGrabbing)
        {
            Debug.Log("Released item");
            Physics.IgnoreLayerCollision(10, 9, false);
            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), false);

            if (grabbableObject.CompareTag("Draggable"))
            {
                grabbableObject.transform.parent.SetParent(null);
                grabbableObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                grabbableObject.transform.SetParent(null);
                grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }

            isGrabbing = false;

        } else if (Input.GetKeyDown(KeyCode.M) && canGrab)
        {
            Debug.Log("Grabbed item");
            
            mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
            // mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward);

            Physics.IgnoreLayerCollision(10, 9, true);
            // Physics.IgnoreCollision(grabbableObject, transform.GetComponent<Collider>(), true);

            // GameObjects tagged as Draggable are actually just small target points of a parent GameObject; dog will use drag animation
            // Examples of Draggable items: carpets
            // GameObjects tagged as Grabbable are directly grabbed by the dog
            // Examples of Grabbable: small items like vases, books
            if (grabbableObject.CompareTag("Draggable"))
            {                                
                grabbableObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbableObject.transform.parent.SetParent(transform);
                grabbableObject.transform.parent.position += transform.TransformDirection(Vector3.up * 0.05f);

                // bite animation
            } 
            else
            {
                grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbableObject.transform.SetParent(transform);
                grabbableObject.transform.position = mouthPosition;
            }

            isGrabbing = true;
            grabText.SetActive(false);

        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something is grabbable.");
        if ((other.CompareTag("Grabbable") || other.CompareTag("Draggable")) && !isGrabbing)
        {
            grabbableObject = other;
            
            // Show UI Popover
            grabText.transform.position = grabbableObject.transform.position + (Vector3.up * 0.30f);
            grabText.SetActive(true);
            
            canGrab = true;
        }

        // if (other.CompareTag("Carpet") && !isGrabbing)
        // {
        //     carpetJoint = other.gameObject.AddComponent<FixedJoint>();
        //     carpetJoint.connectedBody = GetComponent<Rigidbody>(); // Attach carpet to the dog

        //     other.transform.position = mouthPosition;

        //     // bite animation
        // }
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

        if (other.CompareTag("Draggable"))
        {
            // Hide UI Popover
            grabText.SetActive(false);
            canGrab = false;
            
            // if (draggableJoint != null)
            // {
            //     Destroy(draggableJoint);
            // }
        }
    }
}
