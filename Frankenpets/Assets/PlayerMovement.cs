using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player P1;
    public Player P2;

    private FixedJoint fixedJoint;
    public GameObject frontHalf;
    public GameObject backHalf;
    public GameObject player1;
    public GameObject player2;

    public float walkSpeed = 0.8f;
    public float frontTurnSpeed = 1.0f;
    public float backTurnSpeed = 0.7f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the players
        // P1.PlayerNumber = 1;
        // P1.IsFront = true;
        // P1.Species = "cat";
        // P2.PlayerNumber = 2;   
        // P2.IsFront = false;
        // P2.Species = "dog";
        P1 = GetComponent<Player>().P1;
        P2 = GetComponent<Player>().P2;
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedJoint != null && bothHalvesTurningOpposite())
        {
            while (fixedJoint != null && bothHalvesTurningOpposite())
            {
                frontHalf.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                backHalf.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }

            frontHalf.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            backHalf.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        }

        setPlayer1Movement();
        setPlayer2Movement();
    }

    private void setPlayer1Movement()
    {
        // GameObject half = P1.IsFront ? frontHalf : backHalf;
        GameObject half = player1.transform.GetChild(0).gameObject;
        float turnSpeed = P1.IsFront ? frontTurnSpeed : backTurnSpeed;
        
        if (Input.GetKey(KeyCode.A)) half.transform.Rotate(0.0f, -turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.D)) half.transform.Rotate(0.0f, turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.W)) half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.S)) half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    private void setPlayer2Movement()
    {
        // GameObject half = P2.IsFront ? frontHalf : backHalf;
        GameObject half = player2.transform.GetChild(0).gameObject;
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
