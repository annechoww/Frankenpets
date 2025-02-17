using System.Diagnostics;
using UnityEngine;

public class SwitchLogic : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    public float switchTime = 1.5f;

    private Player P1;
    private Player P2;

    private FixedJoint fixedJoint;
    public GameObject frontHalf;
    public GameObject backHalf;


    [Header("Pet Halves")]
    public GameObject catFront;
    public GameObject dogFront;
    public GameObject catBack;
    public GameObject dogBack;

    void Awake()
    {
        P1 = GetComponent<PlayerMovement>().P1;
        P2 = GetComponent<PlayerMovement>().P2;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Debug.Log(P1.IsFront);
        fixedJoint = frontHalf.GetComponent<FixedJoint>();

    }

    // Update is called once per frame
    void Update()
    {
        runSwitchLogic();
    }

    private void runSwitchLogic()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift)) tryStartSwitch();
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) cancelSwitch();

        tryFinishSwitch();
    }

    private void tryStartSwitch()
    {
        if (fixedJoint == null)
        {
            // not allowed; show text to players saying they must be together 

        }
        else
        {
            stopwatch.Start();
        }
    }

    private void cancelSwitch()
    {
        stopwatch.Reset();
    }

    private void tryFinishSwitch()
    {
        if ((stopwatch.Elapsed.TotalSeconds > switchTime) && (fixedJoint != null))
        {
            stopwatch.Reset();

            // Switch which half the players are controlling
            P1.IsFront = !P1.IsFront;
            P2.IsFront = !P2.IsFront;

            // Switch the half to the player's species
            if (P1.IsFront)
            {
                UnityEngine.Debug.Log("P1 is now front");
                if (P1.Species == "cat")
                {
                    // front half = cat = P1
                    // back half = dog = P2

                    // set the position before changing the parent 
                    // o/w it will use world space instead of local (relative to parent) space!!!
                    catFront.transform.position = frontHalf.transform.position; 
                    dogBack.transform.position = backHalf.transform.position;

                    catFront.transform.SetParent(transform.GetChild(0));
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(1));

                    catFront.SetActive(true);
                    catBack.SetActive(false);
                    dogFront.SetActive(false);
                    dogBack.SetActive(true);

                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P1
                    // back half = cat = P2
                    catBack.transform.position = backHalf.transform.position;
                    dogFront.transform.position = frontHalf.transform.position;

                    catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(1));
                    dogFront.transform.SetParent(transform.GetChild(0));
                    dogBack.transform.SetParent(null);

                    catFront.SetActive(false);
                    catBack.SetActive(true);
                    dogFront.SetActive(true);
                    dogBack.SetActive(false);
                    
                    frontHalf = dogFront;
                    backHalf = catBack;
                }
            }
            else // if P2.IsFront
            {
                UnityEngine.Debug.Log("P2 is now front");
                if (P2.Species == "cat")
                {
                    // front half = cat = P2
                    // back half = dog = P1
                    catFront.transform.position = frontHalf.transform.position;
                    dogBack.transform.position = backHalf.transform.position;

                    catFront.transform.SetParent(transform.GetChild(1));
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(0));

                    catFront.SetActive(true);
                    catBack.SetActive(false);
                    dogFront.SetActive(false);
                    dogBack.SetActive(true);

                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P2
                    // back half = cat = P1
                    catBack.transform.position = backHalf.transform.position;
                    dogFront.transform.position = frontHalf.transform.position;
                    
                    catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(0));
                    dogFront.transform.SetParent(transform.GetChild(1));
                    dogBack.transform.SetParent(null);

                    catFront.SetActive(false);
                    catBack.SetActive(true);
                    dogFront.SetActive(true);
                    dogBack.SetActive(false);


                    frontHalf = dogFront;
                    backHalf = catBack;
                }
            }
           
            // Restablish the fixed joint
            fixedJoint = frontHalf.GetComponent<FixedJoint>();

            UnityEngine.Debug.Log("curr fixed joint: ", fixedJoint);

            if (fixedJoint == null) fixedJoint = frontHalf.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

            // SplitLogic splitLogicScript = GetComponent<SplitLogic>();
            // splitLogicScript.reconnect();
            
            UnityEngine.Debug.Log("Switched!");
        }
    }
}
