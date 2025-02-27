using System.Diagnostics;
using UnityEngine;

public class SplitLogic : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    private Quaternion initialRelativeRotation;

    private float splitTime = 1.0f;
    public float reconnectionDistance = 10.0f;
    public KeyCode toggleKey = KeyCode.Space;

    public GameObject frontHalf;
    public GameObject backHalf;
    public GameObject frontMagnet;
    public GameObject backMagnet;
    public FixedJoint fixedJoint;

    private Player P1;
    private Player P2;

    // void Awake()
    // {
    //     P1 = GetComponent<PlayerManager>().P1;
    //     frontHalf = P1.Half;
    //     frontMagnet = frontHalf.transform.GetChild(3).gameObject;

    //     P2 = GetComponent<PlayerManager>().P2;
    //     backHalf = P2.Half;
    //     backMagnet = backHalf.transform.GetChild(3).gameObject;

    //     fixedJoint = P1.Half.GetComponent<FixedJoint>();
    // }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // frontHalf = gameObject.transform.GetChild(0).gameObject;
        // fixedJoint = frontHalf.GetComponent<FixedJoint>();
        initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        runSplitLogic();

        if (canReconnect())
        {
            tryReconnect();
        }
    }


    private void runSplitLogic() 
    {
        // Left vs. right 
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.A)) tryStartSplit();
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.A)) cancelSplit();

        // Left vs. right 
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.D)) tryStartSplit();
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D)) cancelSplit();

        // Up vs. down
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.S)) tryStartSplit();
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.S)) cancelSplit();

        // Up vs. down
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.W)) tryStartSplit();
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.W)) cancelSplit();

        tryFinishSplit();
    }

    private void tryStartSplit()
    {
        stopwatch.Start();
    }

    private void cancelSplit()
    {
        stopwatch.Reset();
    }

    private void tryFinishSplit()
    {
        if ((stopwatch.Elapsed.TotalSeconds > splitTime) && (fixedJoint != null)) 
        {
            stopwatch.Reset();
            Destroy(fixedJoint); // Split the halves
            fixedJoint = null;

            UnityEngine.Debug.Log("Halves disconnected.");
        }      
    }

    private bool canReconnect()
    {
        return (fixedJoint == null) && Input.GetKeyDown(toggleKey);
    }

    private void tryReconnect()
    {
        float distance = Vector3.Distance(frontMagnet.transform.position, backMagnet.transform.position);
        if (distance < reconnectionDistance)
        {
            // Temporarily disable bottomHalf physics
            Rigidbody bottomRb = backHalf.GetComponent<Rigidbody>();
            bool originalKinematic = bottomRb.isKinematic;
            bottomRb.isKinematic = true;

            // Align orientation and position
            backHalf.transform.rotation = frontHalf.transform.rotation * initialRelativeRotation;
            Vector3 positionOffset = frontMagnet.transform.position - backMagnet.transform.position;
            backHalf.transform.position += positionOffset;

            // Re-enable physics
            bottomRb.isKinematic = originalKinematic;

            // Create a new FixedJoint
            fixedJoint = frontHalf.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

            // Apply animation

            UnityEngine.Debug.Log("Halves reconnected.");
        }
    }

    public void reconnect()
    {
        // Temporarily disable bottomHalf physics
        Rigidbody bottomRb = backHalf.GetComponent<Rigidbody>();
        bool originalKinematic = bottomRb.isKinematic;
        bottomRb.isKinematic = true;

        // Align orientation and position
        backHalf.transform.rotation = frontHalf.transform.rotation * initialRelativeRotation;
        Vector3 positionOffset = frontMagnet.transform.position - backMagnet.transform.position;
        backHalf.transform.position += positionOffset;

        // Re-enable physics
        bottomRb.isKinematic = originalKinematic;

        // Create a new FixedJoint
        fixedJoint = frontHalf.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();
    }
}
