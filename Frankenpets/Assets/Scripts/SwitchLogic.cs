using System.Diagnostics;
using UnityEngine;

public class SwitchLogic : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    public float switchTime = 1.5f;

    public Player P1;
    public Player P2;

    [Header("Pet Halves")]
    public GameObject catFront;
    public GameObject dogFront;
    public GameObject catBack;
    public GameObject dogBack;

    void Awake()
    {
        P1 = GetComponent<Player>().P1;
        P2 = GetComponent<Player>().P2;
    }

    private FixedJoint fixedJoint;
    public GameObject frontHalf;
    public GameObject backHalf;

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
                    catFront.SetActive(true);
                    catBack.SetActive(false);
                    dogFront.SetActive(false);
                    dogBack.SetActive(true);

                    catFront.transform.SetParent(transform.GetChild(0));
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(1));

                    catFront.transform.position = dogFront.transform.position;
                    dogBack.transform.position = catBack.transform.position;

                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P1
                    // back half = cat = P2
                    catFront.SetActive(false);
                    catBack.SetActive(true);
                    dogFront.SetActive(true);
                    dogBack.SetActive(false);

                    catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(1));
                    dogFront.transform.SetParent(transform.GetChild(0));
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = dogBack.transform.position;
                    dogFront.transform.position = catFront.transform.position;

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
                    catFront.SetActive(true);
                    catBack.SetActive(false);
                    dogFront.SetActive(false);
                    dogBack.SetActive(true);

                    catFront.transform.SetParent(transform.GetChild(1));
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(0));

                    catFront.transform.position = dogFront.transform.position;
                    dogBack.transform.position = catBack.transform.position;

                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P2
                    // back half = cat = P1
                    catFront.SetActive(false);
                    catBack.SetActive(true);
                    dogFront.SetActive(true);
                    dogBack.SetActive(false);

                    catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(0));
                    dogFront.transform.SetParent(transform.GetChild(1));
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = dogBack.transform.position;
                    dogFront.transform.position = catFront.transform.position;

                    frontHalf = dogFront;
                    backHalf = catBack;
                }
            }
           
            // Restablish the fixed joint
            fixedJoint = frontHalf.GetComponent<FixedJoint>();
            if (fixedJoint == null) fixedJoint = frontHalf.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

            UnityEngine.Debug.Log("Switched!");
        }
    }
}
