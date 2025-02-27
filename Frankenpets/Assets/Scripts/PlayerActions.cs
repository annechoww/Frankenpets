using UnityEngine;


/**
This Class handles all player actions. The actions are mapped as follows:
    - (C) and (/) handles the special actions of the corresponding half and its species (Climb, Grab, Charged Jump, Hind Legs[TBF])
    ----------------------------------------------
    |         Front         |        Back        |
    |--------------------------------------------|
    | Make Noise (X or .)   | Jump (Z or ,)      |
    | Front Paws (Z or ,)   | Use Tail (X or .)  |
    ----------------------------------------------
*/

public class PlayerActions : MonoBehaviour
{
    [Header("Input References")]
    public InputHandler player1Input;
    public InputHandler player2Input;

    [Header("Jumping Variables")]
    public float jumpForce = 15f;
    public float jumpCooldown = 0.5f;
    public float chargedJumpForce = 25f;
    public float chargedJumpCooldown = 0.8f;
    private float lastJumpTime = -10f;

    [Header("Noise Variables")]
    public AudioClip dogClip;
    public AudioClip catClip;

    [Header("Climbing Settings")]
    public float climbSpeed = 2f;
    public float climbCheckDistance = 0.5f;
    private bool isNearClimbable = false;
    private bool isClimbing = false;
    private Vector3 climbDirection;
    private GameObject climbText;

    // Grabbing Variables
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;
    private GameObject grabText;
    private Vector3 mouthPosition;
    private Vector3 mouthDirection;

    private PlayerManager playerManager; 
    private Player P1;
    private Player P2;
    private GameObject frontHalf;
    private GameObject backHalf;
    private Rigidbody frontRb;
    private Rigidbody backRb;

    [Header("Rigging Variables")]
    private MouthRigging mouthRiggingScript;
    private TailRigging tailRiggingScript;
    private HindLegsRigging hindLegsRiggingScript;
    private PawRigging pawRiggingScript;

    private void Start()
    {
        climbText = GameObject.FindGameObjectWithTag("ClimbText");
        grabText = GameObject.FindGameObjectWithTag("GrabText");
        
        Invoke("getPlayerManager", 0f);
    }

    private void Update()
    {
        runJumpLogic();
        runNoiseLogic();
        runClimbLogic();
        runGrablogic();
        runTailLogic();
        runPawLogic();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            // Apply climbing movement in FixedUpdate for smooth physics
            frontRb.linearVelocity = climbDirection * climbSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            // Show UI Popover
            climbText.SetActive(true);
            climbText.transform.position = other.transform.position + (Vector3.forward * 0.05f);

            isNearClimbable = true;
            // Get the surface normal to determine climb direction
            if (Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, climbCheckDistance))
            {
                // Project the up vector onto the surface plane to get the climb direction
                climbDirection = Vector3.ProjectOnPlane(Vector3.up, hit.normal).normalized;
            }
            else
            {
                climbDirection = Vector3.up;
            }
        } else if ((other.CompareTag("Grabbable") || other.CompareTag("Draggable")) && !isGrabbing)
        {
            grabbableObject = other;
            
            // Show UI Popover
            grabText.transform.position = grabbableObject.transform.position + (Vector3.up * 0.30f);
            grabText.SetActive(true);
            
            canGrab = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            // Hide UI popover
            climbText.SetActive(false);

            isNearClimbable = false;
            if (isClimbing)
            {
                StopClimbing();
            }
        } else if (other.CompareTag("Grabbable") || other.CompareTag("Draggable"))
        {
            // Hide UI Popover
            grabText.SetActive(false);
            canGrab = false;
        }
    }

    private void getPlayerManager()
    {
        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();

        // Now you can access Player details like P1.Species
        if (playerManager != null)
        {
            Debug.Log("P1 Species: " + playerManager.P1.Species);
            Debug.Log("P2 Species: " + playerManager.P2.Species);

            P1 = playerManager.P1;
            P2 = playerManager.P2;

            frontHalf = playerManager.getFrontHalf();
            backHalf = playerManager.getBackHalf();

            frontRb = frontHalf.GetComponent<Rigidbody>();
            backRb = backHalf.GetComponent<Rigidbody>();

            mouthRiggingScript = frontHalf.GetComponentInChildren<MouthRigging>();
            tailRiggingScript = backHalf.GetComponentInChildren<TailRigging>();
            pawRiggingScript = frontHalf.GetComponentInChildren<PawRigging>();
            //hindLegsRiggingScript = backHalf.GetComponentInChildren<HindLegsRigging>();
        }
        else
        {
            Debug.Log("Error in fetching playerManager");
        }
    }

////////////////////////////////////// Jump Logic /////////////////////////////////////
    private void runJumpLogic()
    {

        // Get input from handlers instead of direct Input.GetKey
        bool player1Jump = player1Input.GetJumpPressed();
        bool player2Jump = player2Input.GetJumpPressed();
        bool player1Special = player1Input.GetSpecialActionPressed();
        bool player2Special = player2Input.GetSpecialActionPressed(); 

        // Basic Jump
        if ((player1Jump && !P1.IsFront) || (player2Jump && !P2.IsFront))
        {
            tryStartJump(jumpForce, jumpCooldown);
        }

        // Charged Jump - TODO: Update with valid species 
        else if ((player1Special && !P1.IsFront && P1.Species == "cat") || 
                (player2Special && !P2.IsFront && P2.Species == "cat"))
        {
            tryStartJump(chargedJumpForce, chargedJumpCooldown);
        }
    }

    private void tryStartJump(float jumpForce, float jumpCooldown)
    {
        if (Time.time - lastJumpTime > jumpCooldown)
        {
            frontRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            backRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
        }
    }

////////////////////////////////////// Noise Logic /////////////////////////////////////
    private void runNoiseLogic()
    {
        if ((Input.GetKey(KeyCode.X) && P1.IsFront) || ((Input.GetKey(KeyCode.Period)) && P2.IsFront))
        {
            // TODO: Add animation trigger when this happens to frontHalf
            string frontSpecies = P1.IsFront ? P1.Species : P2.Species;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = (frontSpecies == "cat") ? catClip : dogClip;
            audioSource.Play();
            mouthRiggingScript.openMouth();
        }
    }

////////////////////////////////////////// Climb Action ///////////////////////////////////////////////
    private void runClimbLogic()
    {


        // Get special action input for climbing (cat front special)
        bool player1Special = player1Input.GetSpecialActionPressed();
        bool player2Special = player2Input.GetSpecialActionPressed();
        
        // Check for cat species in front position
        string frontSpecies = P1.IsFront ? P1.Species : P2.Species;
        bool isSpecialReleased = (P1.IsFront && !player1Special) || (P2.IsFront && !player2Special);
        
        // Only cats can climb
        if (frontSpecies != "cat") return;
        
        // Start climbing when front cat player presses special near climbable
        if (isNearClimbable && !isClimbing)
        {
            if ((player1Special && P1.IsFront) || (player2Special && P2.IsFront))
            {
                StartClimbing();
            }
        }
        // Stop climbing when button is released
        else if (isSpecialReleased && isClimbing)
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        
        isClimbing = true;
        frontRb.useGravity = false;
        // Zero out current velocities
        frontRb.linearVelocity = Vector3.zero;
        frontRb.angularVelocity = Vector3.zero;
        // Freeze rotation while climbing
        frontRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void StopClimbing()
    {
        isClimbing = false;
        frontRb.useGravity = true;
        frontRb.constraints = RigidbodyConstraints.None;
    }

////////////////////////////////////////// Grab Action ///////////////////////////////////////////////
    private void runGrablogic() // TODO: Add dog species check
    {
        // FOR DEBUGGING: Make sure mouthPosition and mouthDirection match the one at line 54, and comment out the variables on lines 54 & 55
        // Vector3 mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
        // Vector3 mouthDirection = mouthPosition + transform.TransformDirection(Vector3.forward); //Vector3.forward * 0.34f + 
        // Debug.DrawLine(mouthPosition, Vector3.forward + Vector3.up, Color.red, 2, false);
        // Debug.DrawLine(mouthPosition, mouthDirection, Color.red, 2, false);

        bool p1IsDogFront = P1.IsFront && P1.Species == "dog";
        bool p2IsDogFront = P2.IsFront && P2.Species == "dog";

        bool dogFrontSpecialP1 = p1IsDogFront && player1Input.GetSpecialActionPressed();
        bool dogFrontSpecialP2 = p2IsDogFront && player2Input.GetSpecialActionPressed();

        bool dogFrontSpecial = dogFrontSpecialP1 || dogFrontSpecialP2;

        if (dogFrontSpecial && isGrabbing) {
            Debug.Log("Released item");

            Physics.IgnoreLayerCollision(10, 9, false);

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

        }
        else if (dogFrontSpecial && canGrab)
        {
            Debug.Log("Grabbed item");
            
            mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
            Physics.IgnoreLayerCollision(10, 9, true);

            if (grabbableObject.CompareTag("Draggable"))
            {                                
                grabbableObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbableObject.transform.parent.SetParent(transform);
                grabbableObject.transform.parent.position += transform.TransformDirection(Vector3.up * 0.05f);

                // TODO: play bite animation

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
        else if (dogFrontSpecial){
            mouthRiggingScript.openMouth();
        }

        // if (((Input.GetKeyDown(KeyCode.C) && P1.IsFront) || (Input.GetKeyDown(KeyCode.Slash) && P2.IsFront)) && isGrabbing)
        // {
        //     Debug.Log("Released item");

        //     Physics.IgnoreLayerCollision(10, 9, false);

        //     if (grabbableObject.CompareTag("Draggable"))
        //     {
        //         grabbableObject.transform.parent.SetParent(null);
        //         grabbableObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //     }
        //     else
        //     {
        //         grabbableObject.transform.SetParent(null);
        //         grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //     }
            
        //     isGrabbing = false;

        // } else if (((Input.GetKeyDown(KeyCode.C) && P1.IsFront) || (Input.GetKeyDown(KeyCode.Slash) && P2.IsFront)) && canGrab)
        // {
        //     Debug.Log("Grabbed item");
            
        //     mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
        //     Physics.IgnoreLayerCollision(10, 9, true);

        //     if (grabbableObject.CompareTag("Draggable"))
        //     {                                
        //         grabbableObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //         grabbableObject.transform.parent.SetParent(transform);
        //         grabbableObject.transform.parent.position += transform.TransformDirection(Vector3.up * 0.05f);

        //         // TODO: play bite animation

        //     } 
        //     else
        //     {
        //         grabbableObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //         grabbableObject.transform.SetParent(transform);
        //         grabbableObject.transform.position = mouthPosition;
        //     }

        //     isGrabbing = true;
        //     grabText.SetActive(false);
        // } else if (((Input.GetKeyDown(KeyCode.C) && P1.IsFront) || (Input.GetKeyDown(KeyCode.Slash) && P2.IsFront))){
        //     mouthRiggingScript.openMouth();
        // }
    }

////////////////////////////////////////// Hind Legs Action ///////////////////////////////////////////////
// TODO: Fix hind legs prior to adding to script here.


////////////////////////////////////////// Front Paws Action //////////////////////////////////////////////
private void runPawLogic()
{
    if (((Input.GetKeyDown(KeyCode.Z) && P1.IsFront) || (Input.GetKeyDown(KeyCode.Comma) && P2.IsFront)))
    {
        pawRiggingScript.liftPaw();
    }
    
}

////////////////////////////////////////// Tail Action ////////////////////////////////////////////////////
private void runTailLogic()
{
    if (((Input.GetKeyDown(KeyCode.Z) && !P1.IsFront) || (Input.GetKeyDown(KeyCode.Period) && !P2.IsFront)))
    {
        tailRiggingScript.useTail();
    }
    else{
        tailRiggingScript.naturalTailMovement();
    }

}

}