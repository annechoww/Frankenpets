using UnityEngine;

public class PetMovement : MonoBehaviour
{
    public float walkSpeed = 0.8f;
    public float frontTurnSpeed = 1.5f;
    public float backTurnSpeed = 0.7f;

    // public float turnBoost = 0.5f;
    public Transform frontHalf;
    public Transform backHalf;
    // private FixedJoint fixedJoint;

    void Start()
    {
        // fixedJoint = frontHalf.GetComponent<FixedJoint>();
    }

    void Update()
    {
        // fixedJoint = frontHalf.GetComponent<FixedJoint>();
        setFrontHalfMovement();
        setBackHalfMovement();

    }

    // TODO: turn into one function with parameters for controls (because plays can switch halves)
    void setFrontHalfMovement() 
    {
        if (Input.GetKey(KeyCode.LeftArrow)) frontHalf.Rotate(0.0f, -frontTurnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.RightArrow)) frontHalf.Rotate(0.0f, frontTurnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.UpArrow)) frontHalf.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.DownArrow)) frontHalf.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }
    void setBackHalfMovement() 
    {
        if (Input.GetKey(KeyCode.A)) backHalf.Rotate(0.0f, -backTurnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.D)) backHalf.Rotate(0.0f, backTurnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.W)) backHalf.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.S)) backHalf.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    // void setSplitMovement(Vector3 frontHalfDirection, Vector3 backHalfDirection)
    // {
    //     if (Input.GetKey(KeyCode.LeftArrow)) frontHalf.Rotate(0.0f, 1.0f, 0.0f, Space.Self);
    //     if (Input.GetKey(KeyCode.RightArrow)) frontHalf.Rotate(0.0f, -1.0f, 0.0f, Space.Self);

    //     if (Input.GetKey(KeyCode.A)) backHalf.Rotate(0.0f, 1.0f, 0.0f, Space.Self);
    //     if (Input.GetKey(KeyCode.D)) backHalf.Rotate(0.0f, -1.0f, 0.0f, Space.Self);
        

    //     frontHalf.Translate(frontHalfDirection * Time.deltaTime, Space.Self);
    //     backHalf.Translate(backHalfDirection * Time.deltaTime, Space.Self);
    // }

    // void setConnectedMovement(Vector3 frontHalfDirection, Vector3 backHalfDirection)
    // {
    //     // Check if both halves are trying to turn together while connected
    //     if (bothHalvesTurningLeft()) 
    //     {
    //         // Faster turn speed if both halves are turning together
    //         backHalfDirection = new Vector3(0, backHalfDirection.y, backHalfDirection.z);
    //         frontHalfDirection += Vector3.left * turnBoost; 
    //     }

    //     if (bothHalvesTurningRight()) 
    //     {
    //     }
    //     if (bothHalvesTurningRight()) 
    //     {
    //         backHalfDirection = new Vector3(0, backHalfDirection.y, backHalfDirection.z);
    //         frontHalfDirection += Vector3.right * turnBoost; 
    //     }

    //     // if (bothHalvesTurningOpposite())
    //     // {
            
    //     // }

    //     frontHalf.Translate(frontHalfDirection * Time.deltaTime, Space.Self);
    //     backHalf.Translate(backHalfDirection * Time.deltaTime, Space.Self);
    // }

    // Set the direction vector for the front half. 
    // Do not use Time.deltaTime here; it's used in setMovement().
    // private Vector3 getFrontDirection(bool isSplit)
    // {
    //     Vector3 direction = new Vector3();

    //     if (Input.GetKey(KeyCode.LeftArrow) && !isSplit) direction += Vector3.left * turnSpeed;
    //     if (Input.GetKey(KeyCode.RightArrow) && !isSplit) direction += Vector3.right * turnSpeed;
    //     if (Input.GetKey(KeyCode.UpArrow)) direction += Vector3.forward * walkSpeed;
    //     if (Input.GetKey(KeyCode.DownArrow)) direction += Vector3.back * walkSpeed;

    //     return direction;
    // }

    // private Vector3 getBackDirection(bool isSplit)
    // {
    //     Vector3 direction = new Vector3();

    //     if (Input.GetKey(KeyCode.A) && !isSplit) direction += Vector3.left * turnSpeed;
    //     if (Input.GetKey(KeyCode.D) && !isSplit) direction += Vector3.right * turnSpeed;
    //     if (Input.GetKey(KeyCode.W)) direction += Vector3.forward * walkSpeed;
    //     if (Input.GetKey(KeyCode.S)) direction += Vector3.back * walkSpeed;

    //     return direction;
    // }

    // private bool bothHalvesTurningLeft()
    // {
    //     if (fixedJoint != null && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.A)) return true;
    //     if (fixedJoint != null && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.A)) return true;
        
    //     return false;
    // }

    // private bool bothHalvesTurningRight()
    // {
    //     if (fixedJoint != null && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.D)) return true;
        
    //     return false;
    // }

    // private bool bothHalvesTurningOpposite()
    // {
    //     if (fixedJoint != null && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.D)) return true;

    //     if (fixedJoint != null && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.A)) return true;

    //     return false;
    // }

}
