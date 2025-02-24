using UnityEngine;
using System.Diagnostics;
using UnityEditor.VersionControl;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    [Header("Pet Info")]
    public GameObject P1Half; // Only use for initializing
    public GameObject P2Half; // Only use for initializing
    public GameObject P1Magnet; // Only use for initializing
    public GameObject P2Magnet; // Only use for initializing
    private FixedJoint fixedJoint;
    public Player P1 = new Player();
    public Player P2 = new Player();

    [Header("Movement Variables")]
    public float walkSpeed = 0.8f;
    public float frontTurnSpeed = 1.5f;
    public float backTurnSpeed = 1.5f;
    // private bool isFrozen = false; // whether the half's RigidBody's position is frozen in place 

    [Header("Splitting Variables")]
    public float reconnectionDistance = 0.3f;
    public float splitTime = 1.0f;
    public KeyCode reconnectToggleKey = KeyCode.Space;
    private Stopwatch splitStopwatch = new Stopwatch();
    private Quaternion initialRelativeRotation;
    private GameObject frontHalf;
    private GameObject backHalf;
    private GameObject frontMagnet;
    private GameObject backMagnet;


    [Header("Switching Variables")]
    public float switchTime = 1.5f;
    public GameObject catFront;
    public GameObject dogFront;
    public GameObject catBack;
    public GameObject dogBack;
    private Stopwatch switchStopwatch = new Stopwatch();

    [Header("Emoticons")]
    public GameObject sadEmote;    
    public GameObject happyEmote;

    // Others
    private MessageManager messageManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the players
        P1.PlayerNumber = 1;
        P1.IsFront = true;
        P1.Species = "cat";
        P1.Half = P1Half;
        P1.Magnet = P1Magnet;

        P2.PlayerNumber = 2;   
        P2.IsFront = false;
        P2.Species = "dog";
        P2.Half = P2Half;
        P2.Magnet = P2Magnet;

        // Initialize splitting variables 
        frontMagnet = getFrontMagnet();
        backMagnet = getBackMagnet();
        frontHalf = getFrontHalf();
        backHalf = getBackHalf();
        initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;

        alignHalves();
        setJoint();

        // Other variables
        // sadEmote = GameObject.FindGameObjectWithTag("SadEmote");
        // happyEmote = GameObject.FindGameObjectWithTag("HappyEmote");
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (fixedJoint != null)
        {
            runMovementLogic();
        }
        else {
            runSeparatedMovementLogic();
        }
        
        runSplitLogic();
        runSwitchLogic();

        // debugging
        if (Input.GetKeyDown(KeyCode.I))
        {
            UnityEngine.Debug.Log(fixedJoint);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UnityEngine.Debug.Log("p1 front " + P1.IsFront);
            UnityEngine.Debug.Log("p1 " + P1.Half);
            UnityEngine.Debug.Log("p2 front " + P2.IsFront);
            UnityEngine.Debug.Log("p2 " + P2.Half);
            UnityEngine.Debug.Log("front half " + frontHalf);
            UnityEngine.Debug.Log("back half " + backHalf);

        }
    }

    // ADVANCED GETTERS/SETTERS ////////////////////////////////////////////
    public GameObject getFrontHalf()
    {
        if (P1.IsFront) return P1.Half;
        else return P2.Half;
    }

    public GameObject getBackHalf()
    {
        if (!P1.IsFront) return P1.Half;
        else return P2.Half;
    }

    public GameObject getFrontMagnet()
    {
        if (P1.IsFront) return P1.Magnet;
        else return P2.Magnet;
    }

    public GameObject getBackMagnet()
    {
        if (!P1.IsFront) return P1.Magnet;
        else return P2.Magnet;
    }

    public void setJoint(){

            frontHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
            frontHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            backHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
            backHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;

            // Create a new FixedJoint
            fixedJoint = frontHalf.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

            // Set anchor points
            fixedJoint.anchor = frontMagnet.transform.localPosition;
            fixedJoint.connectedAnchor = backMagnet.transform.localPosition;

            // Apply Rigidbody constraints to prevent tilting
            frontHalf.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
      
    }

    public FixedJoint getJoint()
    {
        return getFrontHalf().GetComponent<FixedJoint>(); // frontPlayer.Half.transform.GetChild(0).gameObject.GetComponent<FixedJoint>();
    }
    // ADVANCED GETTERS/SETTERS ////////////////////////////////////////////


    // MOVEMENT METHODS ////////////////////////////////////////////
    private void runMovementLogic()
    {
        // OLD CODE USING setPlayer1Movement() and setPlayer2Movement() methods ////////////////
        // if (fixedJoint != null && bothHalvesTurningOpposite())
        // {
        //     P1.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        //     P2.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        //     isFrozen = true;
        // }

        // if (isFrozen) 
        // {
        //     P1.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //     P1.Half.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //     isFrozen = false;
        // }

        // setPlayer1Movement();
        // setPlayer2Movement();
        // OLD CODE USING setPlayer1Movement() and setPlayer2Movement() methods ////////////////

        // Get turning input from player1 (WASD keys) for turning:
        float turnInputP1 = 0.0f;
        if (Input.GetKey(KeyCode.A)) turnInputP1 -= frontTurnSpeed;
        if (Input.GetKey(KeyCode.D)) turnInputP1 += frontTurnSpeed;
        
        // Get turning input from player2 (arrow keys) for turning:
        float turnInputP2 = 0.0f;
        if (Input.GetKey(KeyCode.LeftArrow)) turnInputP2 -= backTurnSpeed;
        if (Input.GetKey(KeyCode.RightArrow)) turnInputP2 += backTurnSpeed;
        
        // Average the turn inputs
        float combinedTurn = (turnInputP1 + turnInputP2) / 2.0f;
        
        // Get movement (forward/back) input from player1:
        float moveInputP1 = 0.0f;
        if (Input.GetKey(KeyCode.W)) moveInputP1 += walkSpeed;
        if (Input.GetKey(KeyCode.S)) moveInputP1 -= walkSpeed;
        
        // Get movement input from player2:
        float moveInputP2 = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) moveInputP2 += walkSpeed;
        if (Input.GetKey(KeyCode.DownArrow)) moveInputP2 -= walkSpeed;
        
        // Average the movement inputs
        float combinedMove = (moveInputP1 + moveInputP2) / 2.0f;
        
        // Apply the combined rotation to both halves:
        frontHalf.transform.Rotate(0.0f, combinedTurn, 0.0f, Space.Self);
        backHalf.transform.Rotate(0.0f, combinedTurn, 0.0f, Space.Self);
        
        // Apply the combined translation (forward/back) to both halves:
        frontHalf.transform.Translate(Vector3.forward * combinedMove * Time.deltaTime, Space.Self);
        backHalf.transform.Translate(Vector3.forward * combinedMove * Time.deltaTime, Space.Self);
    }

    private void setPlayer1Movement()
    {
        float turnSpeed = P1.IsFront ? frontTurnSpeed : backTurnSpeed;
        
        if (Input.GetKey(KeyCode.A)) P1.Half.transform.Rotate(0.0f, -turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.D)) P1.Half.transform.Rotate(0.0f, turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.W)) P1.Half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.S)) P1.Half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    private void setPlayer2Movement()
    {
        float turnSpeed = P2.IsFront ? frontTurnSpeed : backTurnSpeed;
        
        if (Input.GetKey(KeyCode.LeftArrow)) P2.Half.transform.Rotate(0.0f, -turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.RightArrow)) P2.Half.transform.Rotate(0.0f, turnSpeed, 0.0f, Space.Self);
        if (Input.GetKey(KeyCode.UpArrow)) P2.Half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.DownArrow)) P2.Half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
    }

    private bool bothHalvesTurningOpposite()
    {
        if (fixedJoint != null && Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.D)) return true;

        if (fixedJoint != null && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.A)) return true;

        return false;
    }

    private void runSeparatedMovementLogic()
    {
        // Movement for Player 1’s half (using WASD)
        float turnSpeedP1 = P1.IsFront ? frontTurnSpeed : backTurnSpeed;
        if (Input.GetKey(KeyCode.A))
        {
            P1.Half.transform.Rotate(0.0f, -turnSpeedP1, 0.0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            P1.Half.transform.Rotate(0.0f, turnSpeedP1, 0.0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.W))
        {
            P1.Half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.S))
        {
            P1.Half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
        }
        
        // Movement for Player 2’s half (using Arrow keys)
        float turnSpeedP2 = P2.IsFront ? frontTurnSpeed : backTurnSpeed;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            P2.Half.transform.Rotate(0.0f, -turnSpeedP2, 0.0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            P2.Half.transform.Rotate(0.0f, turnSpeedP2, 0.0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            P2.Half.transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            P2.Half.transform.Translate(Vector3.back * walkSpeed * Time.deltaTime, Space.Self);
        }
    }
    // MOVEMENT METHODS ////////////////////////////////////////////
    

    // SPLITTING METHODS ////////////////////////////////////////////
    private void runSplitLogic() 
    {
        // Determine the input for splitting based on which player is controlling the front half.
        bool frontForward = false;
        bool backBackward = false;
        if (P1.IsFront) // P1 controls the front half, P2 the back half
        {
            frontForward = Input.GetKey(KeyCode.W);
            backBackward = Input.GetKey(KeyCode.DownArrow);
        }
        else // P2 controls the front half, P1 the back half
        {
            frontForward = Input.GetKey(KeyCode.UpArrow);
            backBackward = Input.GetKey(KeyCode.S);
        }
        bool splitCondition = frontForward && backBackward;

        // Start or reset the split timer based on the condition.
        if (splitCondition)
        {
            if (!splitStopwatch.IsRunning)
                splitStopwatch.Start();
        }
        else
        {
            if (splitStopwatch.IsRunning)
                splitStopwatch.Reset();
        }

        // If the split timer exceeds the threshold and the halves are still connected, split them.
        if (splitStopwatch.IsRunning && splitStopwatch.Elapsed.TotalSeconds > splitTime && fixedJoint != null)
        {
            splitStopwatch.Reset();
            Destroy(fixedJoint); // Split the halves
            fixedJoint = null;

            // Add rotation constraints when split
            frontHalf.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            backHalf.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            messageManager.cameraIndicatorMessage(); // Will have split camera, so temporarily display the "P1" / "P2" labels
            UnityEngine.Debug.Log("Halves disconnected due to opposing pull.");
        }

        // Reconnection: if there's no fixed joint and the reconnect key is pressed, try to reconnect.
        if (fixedJoint == null && Input.GetKeyDown(reconnectToggleKey)) 
        {
            tryReconnect();
        }
    }

    private void tryStartSplit()
    {
        splitStopwatch.Start();
    }

    private void cancelSplit()
    {
        splitStopwatch.Reset();
    }

    private void tryFinishSplit()
    {
        if ((splitStopwatch.Elapsed.TotalSeconds > splitTime) && (fixedJoint != null)) 
        {
            splitStopwatch.Reset();
            Destroy(fixedJoint); // Split the halves
            fixedJoint = null;

            UnityEngine.Debug.Log("Halves disconnected.");
        }      
    }

    private bool canReconnect()
    {
        return (fixedJoint == null) && Input.GetKeyDown(reconnectToggleKey);
    }

    private void tryReconnect()
    {
        float distance = Vector3.Distance(frontMagnet.transform.position, backMagnet.transform.position);
        if (distance < reconnectionDistance)
        {
            UnityEngine.Debug.Log("Trying to reconnect.");
            // Temporarily disable bottomHalf physics
            alignHalves();
            // Rigidbody bottomRb = backHalf.GetComponent<Rigidbody>();
            // bool originalKinematic = bottomRb.isKinematic;
            // bottomRb.isKinematic = true;

            // // Align orientation and position
            // backHalf.transform.rotation = frontHalf.transform.rotation * initialRelativeRotation;
            

            // Vector3 positionOffset = frontMagnet.transform.position - backMagnet.transform.position;
            // backHalf.transform.position += positionOffset;

            // // Re-enable physics
            // bottomRb.isKinematic = originalKinematic;

            setJoint();

            // Create a new FixedJoint
            // fixedJoint = frontHalf.AddComponent<ConfigurableJoint>();
            // fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

            // // Configure joint constraints
            // fixedJoint.angularXMotion = ConfigurableJointMotion.Locked;
            // fixedJoint.angularZMotion = ConfigurableJointMotion.Locked;
            // fixedJoint.angularYMotion = ConfigurableJointMotion.Free;

            // // Set anchor points
            // fixedJoint.anchor = frontMagnet.transform.localPosition;
            // fixedJoint.connectedAnchor = backMagnet.transform.localPosition;

            // frontHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
            // frontHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            // backHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationX;
            // backHalf.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationZ;

            // initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;

            // TODO: Apply animation

            UnityEngine.Debug.Log("Halves reconnected.");
        

            // Reset the relative rotation
            initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;

        }
        else
        {
            // error message
            messageManager.reconnectFailMessage();
        }
    }

    private void alignHalves()
    {
        Rigidbody bottomRb = backHalf.GetComponent<Rigidbody>();
        bool originalKinematic = bottomRb.isKinematic;
        bottomRb.isKinematic = true;

        // Align orientation and position
        backHalf.transform.rotation = frontHalf.transform.rotation * initialRelativeRotation;
        

        Vector3 positionOffset = frontMagnet.transform.position - backMagnet.transform.position;
        backHalf.transform.position += positionOffset;

        // Re-enable physics
        bottomRb.isKinematic = originalKinematic;
    }
    // SPLITTING METHODS ////////////////////////////////////////////


    // SWITCHING METHODS ////////////////////////////////////////////
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
            messageManager.switchFailMessage();
        }
        else
        {
            switchStopwatch.Start();
        }
    }

    private void cancelSwitch()
    {
        switchStopwatch.Reset();
    }

    private bool tryFinishSwitch()
    {
        if ((switchStopwatch.Elapsed.TotalSeconds > switchTime) && (fixedJoint != null))
        {
            switchStopwatch.Reset();

            // Switch which half the players are controlling
            P1.IsFront = !P1.IsFront;
            P2.IsFront = !P2.IsFront;

            catFront.SetActive(false);
            catBack.SetActive(false);
            dogFront.SetActive(false);
            dogBack.SetActive(false);

            catFront.GetComponent<Rigidbody>().isKinematic = true;
            catBack.GetComponent<Rigidbody>().isKinematic = true;
            dogFront.GetComponent<Rigidbody>().isKinematic = true;
            dogBack.GetComponent<Rigidbody>().isKinematic = true;

            // Switch the half to the player's species
            if (P1.IsFront)
            {
                UnityEngine.Debug.Log("P1 is now front");
                if (P1.Species == "cat")
                {
                    // make sure all halves have no parents before setting their position
                    // o/w it will use world space instead of local (relative to parent) space!!!
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);

                    catFront.transform.position = frontHalf.transform.position; 
                    catFront.transform.rotation = frontHalf.transform.rotation;
                    dogBack.transform.position = backHalf.transform.position;
                    dogBack.transform.rotation = backHalf.transform.rotation;

                    // set the correct halves as children under P1 and P2
                    catFront.transform.SetParent(transform.GetChild(0));
                    // catBack.transform.SetParent(null);
                    // dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(1));

                    // hide/unhide the halves
                    // catFront.SetActive(true);
                    // catBack.SetActive(false);
                    // dogFront.SetActive(false);
                    // dogBack.SetActive(true);

                    // update players and variables
                    P1.Half = catFront;
                    P2.Half = dogBack;
                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P1
                    // back half = cat = P2
                    catFront.transform.SetParent(null);
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = backHalf.transform.position + transform.TransformDirection(Vector3.up * 0.15f);
                    catBack.transform.rotation = backHalf.transform.rotation;
                    dogFront.transform.position = frontHalf.transform.position + transform.TransformDirection(Vector3.up * 0.15f);
                    dogFront.transform.rotation = frontHalf.transform.rotation;

                    // catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(1));
                    dogFront.transform.SetParent(transform.GetChild(0));
                    // dogBack.transform.SetParent(null);

                    // catFront.SetActive(false);
                    // catBack.SetActive(true);
                    // dogFront.SetActive(true);
                    // dogBack.SetActive(false);
                    
                    P1.Half = dogFront;
                    P2.Half = catBack;
                    frontHalf = dogFront;
                    backHalf = catBack;
                }

                P1.Magnet = frontHalf.transform.GetChild(2).gameObject;
                P2.Magnet = backHalf.transform.GetChild(2).gameObject;
                frontMagnet = P1.Magnet;
                backMagnet = P2.Magnet;

            }
            else // if P2.IsFront
            {
                UnityEngine.Debug.Log("P2 is now front");
                if (P2.Species == "cat")
                {
                    // front half = cat = P2
                    // back half = dog = P1
                    catBack.transform.SetParent(null);
                    dogFront.transform.SetParent(null);

                    catFront.transform.position = frontHalf.transform.position;
                    catFront.transform.rotation = frontHalf.transform.rotation;
                    dogBack.transform.position = backHalf.transform.position;
                    dogBack.transform.rotation = backHalf.transform.rotation;

                    catFront.transform.SetParent(transform.GetChild(1));
                    // catBack.transform.SetParent(null);
                    // dogFront.transform.SetParent(null);
                    dogBack.transform.SetParent(transform.GetChild(0));

                    // catFront.SetActive(true);
                    // catBack.SetActive(false);
                    // dogFront.SetActive(false);
                    // dogBack.SetActive(true);

                    P2.Half = catFront;
                    P1.Half = dogBack;
                    frontHalf = catFront;
                    backHalf = dogBack;
                }
                else
                {
                    // front half = dog = P2
                    // back half = cat = P1
                    catFront.transform.SetParent(null);
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = backHalf.transform.position + transform.TransformDirection(Vector3.up * 0.15f);
                    catBack.transform.rotation = backHalf.transform.rotation;
                    dogFront.transform.position = frontHalf.transform.position + transform.TransformDirection(Vector3.up * 0.15f);
                    dogFront.transform.rotation = frontHalf.transform.rotation;

                    
                    // catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(0));
                    dogFront.transform.SetParent(transform.GetChild(1));
                    // dogBack.transform.SetParent(null);

                    // catFront.SetActive(false);
                    // catBack.SetActive(true);
                    // dogFront.SetActive(true);
                    // dogBack.SetActive(false);


                    P2.Half = dogFront;
                    P1.Half = catBack;
                    frontHalf = dogFront;
                    backHalf = catBack;
                }
                    P2.Magnet = frontHalf.transform.GetChild(2).gameObject;
                    P1.Magnet = backHalf.transform.GetChild(2).gameObject;
                    frontMagnet = P2.Magnet;
                    backMagnet = P1.Magnet;
            }
           
            alignHalves();
            setJoint(); // Restablish fixed joint 
            
            catFront.GetComponent<Rigidbody>().isKinematic = false;
            catBack.GetComponent<Rigidbody>().isKinematic = false;
            dogFront.GetComponent<Rigidbody>().isKinematic = false;
            dogBack.GetComponent<Rigidbody>().isKinematic = false;

            // Reset the rotation according to the new halves (maybe don't need this though because new halves should be in same rotation as old ones?)
            // initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;

            P1.Half.SetActive(true);
            P2.Half.SetActive(true);

            UnityEngine.Debug.Log("Switched!");
        }

        return false;
    }
    // SWITCHING METHODS ////////////////////////////////////////////

    // EMOTES ////////////////////////////////////////////
    public GameObject startEmote(GameObject half, string emotion)
    {
        GameObject emoticon;

        switch (emotion)
        {
            case "sad": emoticon = sadEmote; break;
            default: emoticon = happyEmote; break;
        }

        emoticon.SetActive(true);
        emoticon.transform.SetParent(half.transform);
        emoticon.transform.position = half.transform.position + (Vector3.up * 0.35f);
        return emoticon;
    }

    public void cancelEmote(GameObject emoticon)
    {
        emoticon.transform.SetParent(null);
        emoticon.SetActive(false);
    }
    // EMOTES ////////////////////////////////////////////

}
