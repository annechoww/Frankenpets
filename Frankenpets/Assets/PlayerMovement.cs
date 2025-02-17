using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player P1;
    public Player P2;

    public GameObject P1Half;
    public GameObject P2Half;

    private FixedJoint fixedJoint;

    public float walkSpeed = -0.8f;
    public float frontTurnSpeed = 0.2f;
    public float backTurnSpeed = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (P1.IsFront) fixedJoint = GetComponent<PlayerManager>().getFixedJoint(P1);
        else fixedJoint = GetComponent<PlayerManager>().getFixedJoint(P2);

        if (fixedJoint != null && bothHalvesTurningOpposite())
        {
            while (fixedJoint != null && bothHalvesTurningOpposite())
            {
                P1.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                P2.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }

            P1.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            P2.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        }

        setPlayer1Movement();
        setPlayer2Movement();
    }

    private void setPlayer1Movement()
    {
        // GameObject half = P1.IsFront ? frontHalf : backHalf;
        // GameObject half = player1.transform.GetChild(0).gameObject;
        GameObject half = P1.Half;
        float turnSpeed = P1.IsFront ? frontTurnSpeed : backTurnSpeed;
        
        if (Input.GetKey(KeyCode.A)) half.transform.Rotate(0.0f, -turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.D)) half.transform.Rotate(0.0f, turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.W)) half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.S)) half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    private void setPlayer2Movement()
    {
        // GameObject half = P2.IsFront ? frontHalf : backHalf;
        GameObject half = P2.Half;
        float turnSpeed = P2.IsFront ? frontTurnSpeed : backTurnSpeed;
        
        if (Input.GetKey(KeyCode.LeftArrow)) half.transform.Rotate(0.0f, -turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.RightArrow)) half.transform.Rotate(0.0f, turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.UpArrow)) half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.DownArrow)) half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    private bool bothHalvesTurningOpposite()
    {
        if (fixedJoint != null && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.D)) return true;

        if (fixedJoint != null && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.A)) return true;

        return false;
    }
}
