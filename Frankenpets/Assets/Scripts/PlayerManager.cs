using UnityEngine;
using System.Diagnostics;
using Unity.Cinemachine;
using UnityEditor.VersionControl;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Collections;

public class PlayerManager : MonoBehaviour
{

    [Header("Player Input References")]
    public InputHandler player1Input;
    public InputHandler player2Input;

    [Header("Pet Info")]
    public GameObject P1Half; // Only use for initializing
    public GameObject P2Half; // Only use for initializing
    public GameObject P1Magnet; // Only use for initializing
    public GameObject P2Magnet; // Only use for initializing
    private FixedJoint fixedJoint;
    public Player P1 = new Player();
    public Player P2 = new Player();

    [Header("Movement Variables")]
    public float walkSpeed = 0.6f;
    public float turnSpeed = 1.0f;
    // private bool isFrozen = false; // whether the half's RigidBody's position is frozen in place 

    [Header("Splitting Variables")]
    public float reconnectionDistance = 0.3f;
    public float splitTime = 1.0f;
    public KeyCode reconnectToggleKey = KeyCode.Space;
    public AudioClip splitSound;
    public AudioClip reconnectSound;
    private Stopwatch splitStopwatch = new Stopwatch();
    private Quaternion initialRelativeRotation;
    private GameObject frontHalf;
    private GameObject backHalf;
    private GameObject frontMagnet;
    private GameObject backMagnet;
    private bool splitCondition = false; // stretching rig listens for this


    [Header("Switching Variables")]
    public float switchTime = 1.5f;
    public GameObject catFront;
    public GameObject dogFront;
    public GameObject catBack;
    public GameObject dogBack;
    private bool player1SwitchPressed = false;
    private bool player2SwitchPressed = false;

    public PetStationManager stationManager;

    [Header("Cameras")]
    public CinemachineCamera player1Camera;
    public CinemachineCamera player2Camera;
    public CameraMovement cameraMovement;
    public CinemachineCamera mainCamera;

    private Stopwatch switchStopwatch = new Stopwatch();

    [Header("Emoticons")]
    public GameObject sadEmote;    
    public GameObject happyEmote;

    [Header("Icons")]
    public GameObject P1CatIcon;    
    public GameObject P1DogIcon;
    public GameObject P2CatIcon;    
    public GameObject P2DogIcon;
    public GameObject P1CatSpeechIcon;
    public GameObject P1DogSpeechIcon;
    public GameObject P2CatSpeechIcon;
    public GameObject P2DogSpeechIcon;

    // Others
    private MessageManager messageManager;

    // Mainly used for the tutorial
    public bool canReconnect = true;
    public bool canSwitch = true;
    public bool canSplit = true;

    void Awake()
    {
        // Initialize the players
        P1.PlayerNumber = 1;
        P1.IsFront = true;
        P1.Half = P1Half;
        P1.Magnet = P1Magnet;

        if (P1.Half == catFront)
        {
            P1.Species = "cat";
        } else if (P1.Half == dogFront) 
        {
            P1.Species = "dog";
        } else 
        {
            UnityEngine.Debug.LogError("Player 1 set to invalid half.");
        }

        P2.PlayerNumber = 2;   
        P2.IsFront = false;
        P2.Species = "dog";
        P2.Half = P2Half;
        P2.Magnet = P2Magnet;

        if (P2.Half == catBack)
        {
            P2.Species = "cat";
        } else if (P2.Half == dogBack) 
        {
            P2.Species = "dog";
        } else 
        {
            UnityEngine.Debug.LogError("Player 2 set to invalid half.");
        }

        // Initialize splitting variables 
        frontMagnet = getFrontMagnet();
        backMagnet = getBackMagnet();
        frontHalf = getFrontHalf();
        backHalf = getBackHalf();
        initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;

        alignHalves();
        setJoint();
        updatePlayerIcons();

        GameObject messageObject = GameObject.Find("Messages");
        if (messageObject != null)
        {
            messageManager = messageObject.GetComponent<MessageManager>();
            if (messageManager != null)
            {
                UnityEngine.Debug.Log("MessageManager component found!");
            }
        } else
        {
            UnityEngine.Debug.LogError("GameObject 'Messages' not found in the scene.");
        }
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

        CheckSwitchInput();
        CheckReconnectInput();
        
        runSplitLogic();
        runSwitchLogic();
        EnsureUpright();

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
        return getFrontHalf().GetComponent<FixedJoint>();
    }
    // ADVANCED GETTERS/SETTERS ////////////////////////////////////////////


    // GETTERS/SETTERS FOR TUTORIAL PURPOSES /////////////////////////////
    // Used to determine when a pet can split/switch/reconnect in specific points of the tutorial
    public void setCanReconnect(bool canReconnect)
    {
        this.canReconnect = canReconnect;
    }
    public void setCanSplit(bool canSplit)
    {
        this.canSplit = canSplit;
    }
    public void setCanSwitch(bool canSwitch)
    {
        this.canSwitch = canSwitch;
    }
    public bool getCanReconnect()
    {
        return this.canReconnect;
    }
    public bool getCanSplit()
    {
        return this.canSplit;
    }
    public bool getCanSwitch()
    {
        return this.canSwitch;
    }
    // GETTERS/SETTERS FOR TUTORIAL PURPOSES /////////////////////////////


    // PLAYER INPUT CHECKERS ////////////////////////////////////////////
    public bool CheckSwitchInput() {
        player1SwitchPressed = player1Input.GetSwitchPressed();
        player2SwitchPressed = player2Input.GetSwitchPressed();

        return player1SwitchPressed && player2SwitchPressed;
    }

    private void CheckReconnectInput() {
        if ((player1Input.GetReconnectPressed() || player2Input.GetReconnectPressed()) && fixedJoint == null && canReconnect)
        {
            tryReconnect();
        }
    }

    private void CheckSplitConditions(float player1YInput, float player2YInput)
    {
        // Determine if players are pulling in opposite directions based on their positions
        bool frontPullingForward = false;
        bool backPullingBackward = false;
        
        if (P1.IsFront)
        {
            frontPullingForward = player1YInput > 0;
            backPullingBackward = player2YInput < 0;
        }
        else // P2 is front
        {
            frontPullingForward = player2YInput > 0;
            backPullingBackward = player1YInput < 0;
        }

        splitCondition = frontPullingForward && backPullingBackward;
        
        // Start or reset split timer based on pull direction
        if (splitCondition)
        {
            if (!splitStopwatch.IsRunning)
            {
                splitStopwatch.Start();
                // TODO: stretch sound
            }
        }
        else
        {
            if (splitStopwatch.IsRunning)
            {
                // TODO: stop stretch sound
                splitStopwatch.Reset();
            }
        }
    }
    // PLAYER INPUT CHECKERS ////////////////////////////////////////////


    // MOVEMENT METHODS ////////////////////////////////////////////
    private void runMovementLogic()
    {
        // Get player 1's input
        Vector2 player1MoveInput = player1Input.GetMoveInput();
        float turnInputP1 = player1MoveInput.x * turnSpeed;
        float moveInputP1 = player1MoveInput.y * walkSpeed;
        
        // Get player 2's input
        Vector2 player2MoveInput = player2Input.GetMoveInput();
        float turnInputP2 = player2MoveInput.x * turnSpeed;
        float moveInputP2 = player2MoveInput.y * walkSpeed;
        
        // Apply the combined rotation to both halves:
        float combinedTurn = turnInputP1 + turnInputP2;
        frontHalf.transform.Rotate(0.0f, combinedTurn, 0.0f, Space.Self);
        backHalf.transform.Rotate(0.0f, combinedTurn, 0.0f, Space.Self);
        
        // Apply the combined translation (forward/back) to both halves:
        float combinedMove = moveInputP1 + moveInputP2;
        frontHalf.transform.Translate(Vector3.forward * combinedMove * Time.deltaTime, Space.Self);
        backHalf.transform.Translate(Vector3.forward * combinedMove * Time.deltaTime, Space.Self);

        // Update split condition checking for pulling opposite directions
        CheckSplitConditions(player1MoveInput.y, player2MoveInput.y);

        // Update rig movement directionality
        RiggingMovement[] frontRigs = frontHalf.GetComponentsInChildren<RiggingMovement>();
        RiggingMovement[] backRigs = backHalf.GetComponentsInChildren<RiggingMovement>();
        RiggingMovement[] allRigs = new RiggingMovement[frontRigs.Length + backRigs.Length];
        frontRigs.CopyTo(allRigs, 0);
        backRigs.CopyTo(allRigs, frontRigs.Length);

        foreach (RiggingMovement rigging in allRigs)
        {
            rigging.changeTargetDirection(combinedMove);
        }
    }

    private void runSeparatedMovementLogic()
    {
        // Get player inputs
        Vector2 player1MoveInput = player1Input.GetMoveInput();
        Vector2 player2MoveInput = player2Input.GetMoveInput();
        
        // Movement for Player 1's half
        float turnSpeedP1 = turnSpeed;
        P1.Half.transform.Rotate(0.0f, player1MoveInput.x * turnSpeedP1, 0.0f, Space.Self);
        P1.Half.transform.Translate(Vector3.forward * player1MoveInput.y * walkSpeed * Time.deltaTime, Space.Self);
        
        // Movement for Player 2's half
        float turnSpeedP2 = turnSpeed;
        P2.Half.transform.Rotate(0.0f, player2MoveInput.x * turnSpeedP2, 0.0f, Space.Self);
        P2.Half.transform.Translate(Vector3.forward * player2MoveInput.y * walkSpeed * Time.deltaTime, Space.Self);
    }
    // MOVEMENT METHODS ////////////////////////////////////////////
    

    // SPLITTING METHODS ////////////////////////////////////////////
    private void runSplitLogic() 
    {
        // If the split timer exceeds the threshold and the halves are still connected, split them.
        if (splitStopwatch.IsRunning && splitStopwatch.Elapsed.TotalSeconds > splitTime && fixedJoint != null && canSplit)
        {
            splitStopwatch.Reset();
            Destroy(fixedJoint); // Split the halves
            fixedJoint = null;
            // TODO: stop stretch sound

            // Add rotation constraints when split
            frontHalf.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            backHalf.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            messageManager.cameraIndicatorMessage(); // Will have split camera, so temporarily display the "P1" / "P2" labels
            
            // Play audio
            if (splitSound != null)
            {
                AudioSource.PlayClipAtPoint(splitSound, transform.position);
            }
            

            UnityEngine.Debug.Log("Halves disconnected due to opposing pull.");
        }
    }

    private void tryReconnect()
    {
        float distance = Vector3.Distance(frontMagnet.transform.position, backMagnet.transform.position);
        if (distance < reconnectionDistance)
        {
            UnityEngine.Debug.Log("Trying to reconnect.");
            alignHalves();
            setJoint();

            // TODO: Apply animation

            // Play audio
            if (reconnectSound != null)
            {
                AudioSource.PlayClipAtPoint(reconnectSound, transform.position); 
            }

            UnityEngine.Debug.Log("Halves reconnected.");
        
            // Reset the relative rotation
            initialRelativeRotation = Quaternion.Inverse(frontHalf.transform.rotation) * backHalf.transform.rotation;
        }
        else
        {
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

    public bool shouldStretch()
    {
        return splitCondition;
    }
    // SPLITTING METHODS ////////////////////////////////////////////


    // SWITCHING METHODS ////////////////////////////////////////////
    // runSwitchLogic(), tryStartSwitch(), cancelSwitch(), tryFinishSwitch() correspond to front <-> back switching (NOT SWITCHING SPECIES!)
    private void runSwitchLogic()
    {
        if (CheckSwitchInput() && canSwitch) 
            tryStartSwitch();
        else
            cancelSwitch();

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

    private void tryFinishSwitch()
    {
        if ((switchStopwatch.Elapsed.TotalSeconds > switchTime) && (fixedJoint != null))
        {
            switchStopwatch.Reset();

            // Switch which half the players are controlling
            P1.IsFront = !P1.IsFront;
            P2.IsFront = !P2.IsFront;

            Destroy(frontHalf.GetComponent<FixedJoint>());

            catFront.SetActive(false);
            catBack.SetActive(false);
            dogFront.SetActive(false);
            dogBack.SetActive(false);

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

                    // update players and variables
                    P1.Half = catFront;
                    P2.Half = dogBack;
                    P1.Magnet = catFront.transform.GetChild(2).gameObject;
                    P2.Magnet = dogBack.transform.GetChild(2).gameObject;
                }
                else
                {
                    // front half = dog = P1
                    // back half = cat = P2
                    catFront.transform.SetParent(null);
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = backHalf.transform.position + transform.TransformDirection(Vector3.up * 0.215f);
                    catBack.transform.rotation = backHalf.transform.rotation;
                    dogFront.transform.position = frontHalf.transform.position + transform.TransformDirection(Vector3.up * 0.215f);
                    dogFront.transform.rotation = frontHalf.transform.rotation;

                    // catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(1));
                    dogFront.transform.SetParent(transform.GetChild(0));
                    // dogBack.transform.SetParent(null);
                    
                    P1.Half = dogFront;
                    P2.Half = catBack;
                    P1.Magnet = dogFront.transform.GetChild(2).gameObject;
                    P2.Magnet = catBack.transform.GetChild(2).gameObject;
                }
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

                    P2.Half = catFront;
                    P1.Half = dogBack;
                    P2.Magnet = catFront.transform.GetChild(2).gameObject;
                    P1.Magnet = dogBack.transform.GetChild(2).gameObject;
                }
                else
                {
                    // front half = dog = P2
                    // back half = cat = P1
                    catFront.transform.SetParent(null);
                    dogBack.transform.SetParent(null);

                    catBack.transform.position = backHalf.transform.position + transform.TransformDirection(Vector3.up * 0.215f);
                    catBack.transform.rotation = backHalf.transform.rotation;
                    dogFront.transform.position = frontHalf.transform.position + transform.TransformDirection(Vector3.up * 0.215f);
                    dogFront.transform.rotation = frontHalf.transform.rotation;
                    
                    // catFront.transform.SetParent(null);
                    catBack.transform.SetParent(transform.GetChild(0));
                    dogFront.transform.SetParent(transform.GetChild(1));
                    // dogBack.transform.SetParent(null);

                    P2.Half = dogFront;
                    P1.Half = catBack;
                    P2.Magnet = dogFront.transform.GetChild(2).gameObject;
                    P1.Magnet = catBack.transform.GetChild(2).gameObject;
                }
            }
           
            player1Camera.Follow = P1.Half.transform;
            player1Camera.LookAt = P1.Half.transform;
            player2Camera.Follow = P2.Half.transform;
            player2Camera.LookAt = P2.Half.transform;

            refreshHalves();
            
            P1.Half.SetActive(true);
            P2.Half.SetActive(true);

            alignHalves();

            if (getJoint() == null)
            {
                setJoint();
            }
            
            updatePlayerIcons();

            // Play audio (TODO: NEW AUDIO)
            if (splitSound != null)
            {
                AudioSource.PlayClipAtPoint(splitSound, transform.position);
            }

            UnityEngine.Debug.Log("Switched!");
        }
    }

    private void tryFinishSwitchV2() 
    {
        if ((switchStopwatch.Elapsed.TotalSeconds > switchTime) && (fixedJoint != null))
        {
            switchStopwatch.Reset();

            // Switch which half the players are controlling
            P1.IsFront = !P1.IsFront;
            P2.IsFront = !P2.IsFront;

            string P1PrevSpecies = P1.Species;
            P1.Species = P2.Species;
            P2.Species = P1PrevSpecies;

            if (P1.IsFront)
            {
                P1.Half = frontHalf;
                P2.Half = backHalf;
                P1.Magnet = frontHalf.transform.GetChild(2).gameObject;
                P2.Magnet = backHalf.transform.GetChild(2).gameObject;
            }
            else
            {
                P1.Half = backHalf;
                P2.Half = frontHalf;
                P1.Magnet = backHalf.transform.GetChild(2).gameObject;
                P2.Magnet = frontHalf.transform.GetChild(2).gameObject;
            }

            player1Camera.Follow = P1.Half.transform;
            player1Camera.LookAt = P1.Half.transform;
            player2Camera.Follow = P2.Half.transform;
            player2Camera.LookAt = P2.Half.transform;

            refreshHalves();
            updatePlayerIcons();

            if (getJoint() == null)
            {
                setJoint();
            }

            messageManager.switchSuccessMessage();
            UnityEngine.Debug.Log("Switched!");
        }
    }

    public void TransferControl(GameObject dockedHalf, GameObject counterpart)
    {
        // First determine which player owns this half
        bool isPlayer1Docked = false;
        
        // Check under Player1's children
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.gameObject == dockedHalf)
            {
                isPlayer1Docked = true;
                break;
            }
        }

        Player controllingPlayer = isPlayer1Docked ? P1 : P2;
        Player otherPlayer = isPlayer1Docked ? P2 : P1;

        // Update the controlling player's half reference
        controllingPlayer.Half = counterpart;
        
        // Update magnet reference - make sure magnet exists
        GameObject petMagnet = null;

        foreach (Transform child in counterpart.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("PetMagnet"))
            {
                petMagnet = child.gameObject;
                break;
            }
        }

        if (petMagnet != null)
        {
            controllingPlayer.Magnet = petMagnet;
        }
        else
        {
            UnityEngine.Debug.LogError($"No magnet found on {counterpart.name}");
        }

        // Update front/back status if needed
        bool wasControllingFront = dockedHalf.name.Contains("Front");
        bool willControlFront = counterpart.name.Contains("Front");
        
        if (wasControllingFront != willControlFront)
        {
            controllingPlayer.IsFront = willControlFront;
            otherPlayer.IsFront = !willControlFront;
        }

        // Make sure the Rigidbody is not kinematic after transfer
        Rigidbody rb = counterpart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        bool isDocked = dockedHalf.GetComponent<Rigidbody>().isKinematic;
        if (isDocked && counterpart != null) {
            if (controllingPlayer.PlayerNumber == 1) {
                player1Camera.Follow = counterpart.transform;
                player1Camera.LookAt = counterpart.transform;
            } else {
                player2Camera.Follow = counterpart.transform;
                player2Camera.LookAt = counterpart.transform;
            }
        }

        refreshHalves();
        updatePlayerIcons();
    }

    public void refreshHalves()
    {
        frontHalf = getFrontHalf();
        backHalf = getBackHalf();
        frontMagnet = getFrontMagnet();
        backMagnet = getBackMagnet();

        cameraMovement.frontHalf = frontHalf.transform;
        mainCamera.Follow = frontHalf.transform;
        mainCamera.LookAt = frontHalf.transform;
    }
    // SWITCHING METHODS ////////////////////////////////////////////


    // COLLISION METHODS ////////////////////////////////////////////
    void EnsureUpright() 
    {
        if (fixedJoint != null) {  // Only enforce when connected
            // Lock rotation around X and Z axes while preserving Y rotation
            Quaternion frontRotation = frontHalf.transform.rotation;
            Quaternion backRotation = backHalf.transform.rotation;
            
            // Keep only the Y rotation
            Vector3 frontEuler = frontRotation.eulerAngles;
            Vector3 backEuler = backRotation.eulerAngles;
            
            frontHalf.transform.rotation = Quaternion.Euler(0, frontEuler.y, 0);
            backHalf.transform.rotation = Quaternion.Euler(0, backEuler.y, 0);
        }
    }
    // COLLISION METHODS ////////////////////////////////////////////


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


    // ICONS ////////////////////////////////////////////
    public void updatePlayerIcons()
    {
        bool isP1Cat = P1.Species == "cat";
        bool isP1Front = P1.IsFront;

        P1CatIcon.SetActive(isP1Cat);
        P1DogIcon.SetActive(!isP1Cat);
        P2CatIcon.SetActive(!isP1Cat);
        P2DogIcon.SetActive(isP1Cat);

        P1CatSpeechIcon.SetActive(isP1Cat);
        P1DogSpeechIcon.SetActive(!isP1Cat);
        P2CatSpeechIcon.SetActive(!isP1Cat);
        P2DogSpeechIcon.SetActive(isP1Cat);
    }
    // ICONS ////////////////////////////////////////////


    // COROUTINES //////////////////////////////////////
    private IEnumerator waitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
