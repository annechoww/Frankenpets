using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    private ControllerAssignment controllerAssignment;

    [Header("Jumping Variables")]
    private float jumpForce = 20f;
    public float jumpCooldown = 0.5f;
    public float chargedJumpForce = 35f;
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
    private ClimbMovement climbMovementScript;

    [Header("Grab System Settings")]
    [Tooltip("Whether to use the hybrid grab system with virtual tether for draggable objects")]
    public bool useHybridGrabSystem = true;
    
    [Tooltip("Transform that represents the dog's mouth grab point")]
    public Transform mouthGrabPoint;

    // Grabbing Variables
    private RaycastHit hit;
    private Collider grabbableObject;
    private bool canGrab = false;
    private bool isGrabbing = false;
    private Vector3 mouthPosition;
    private Vector3 mouthDirection;
    private Vector3 grabPoint; // Position where the joint connects

    private GrabPoint currentGrabPoint;
    private GrabbableObject currentGrabbableObject;
    private bool isDraggableObject = false;

    [Header("Angle Restriction")]
    [Tooltip("Maximum angle the pet can turn away from the grab point (in degrees)")]
    public float maxTurnAngle = 45f;
    private Vector3 initialGrabDirection;

    [Header("Grab Joint Settings")]
    public float defaultSpringForce = 1000f;
    public float defaultDamperForce = 20f;
    private ConfigurableJoint grabJoint;

    private DragController dragController;
    private float originalPlayerSpeed;

    private float originalTurnSpeed;
    private bool turnRestricted = false;

    // Respawn Variables
    private PlayerRespawn playerRespawn;

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
    private HindLegsRigging2 hindRiggingScript;
    private PawRigging pawRiggingScript;
    private ClimbRigging climbRiggingScript;
    private GrabRigging grabRiggingScript;
    
    public Transform objectGrabPoint;
    public bool isPaw; // MudTracks.cs listens to this bool, so leave it as public!
    public Transform objectDragPoint;
    private bool isStanding = false;

    [Header("UI Variables")]
    public GameObject grabText;
    public GameObject climbText;
    public GameObject pawText;
    public GameObject controlsMenuParent;
    private GameObject controlsMenu;
    private bool isViewingControlsMenu = false;

    [Header("Movement Scheme UI")]
    public Slider movementSchemeSlider;
    public TextMeshProUGUI standardSchemeText;
    public TextMeshProUGUI altSchemeText;
    private Color activeColor = new Color(1f, 1f, 0.5f); // Bright yellow-orange
    private Color inactiveColor = new Color(0.49f, 0.4f, 0.31f); // Dimmed version

    [Header("Rumble Control UI")]
    public Slider player1RumbleSlider;
    public Slider player2RumbleSlider;
    public UnityEngine.UI.Image player1RumbleBackground;
    public UnityEngine.UI.Image player2RumbleBackground;
    private Color rumbleEnabledColor = new Color(1f, 1f, 0.5f); 
    private Color rumbleDisabledColor = new Color(0.49f, 0.4f, 0.31f);

    [Header("Restart Game UI")]
    public UnityEngine.UI.Image radialFillImage;             // Reference to the radial fill image
    public GameObject confirmationPopup;      // Reference to the confirmation popup
    public TextMeshProUGUI player1ConfirmText;   // Reference to P1 "Confirm?" text
    public TextMeshProUGUI player1ConfirmedText; // Reference to P1 "Confirmed!" text
    public TextMeshProUGUI player2ConfirmText;   // Reference to P2 "Confirm?" text
    public TextMeshProUGUI player2ConfirmedText; // Reference to P2 "Confirmed!" text
    public TextMeshProUGUI confirmPromptText; // Reference to the confirmation prompt text

    // Restart system variables
    private float radialFillAmount = 0f;      // Current fill amount (0-1)
    private float fillSpeed = 0.5f;           // Speed of fill per second
    private float decaySpeed = 0.4f;          // Speed of decay per second when not holding
    private float confirmThreshold = 0.95f;   // Threshold to trigger confirmation popup
    private bool player1HoldingRestart = false;
    private bool player2HoldingRestart = false;
    private bool player1Confirmed = false;   // For confirmation popup
    private bool player2Confirmed = false;   // For confirmation popup
    private bool inConfirmationPhase = false;

    // Respawn variables
    private float player1RespawnHoldTime = 0f;
    private float player2RespawnHoldTime = 0f;
    private const float REQUIRED_RESPAWN_HOLD_TIME = 1.5f; // Time in seconds players need to hold the button
    private bool player1Respawning = false;
    private bool player2Respawning = false;

    // Initial scene to load on restart
    public string initialSceneName = "Splash Screen";


    [Header("Tutorial Variables")]
    public bool isTutorial = false; // ENABLE THIS IN THE ATTIC
    
    // Tutorial overlay
    [Header("Tutorial Overlay")]
    public TutorialText tutorialTextScript;

    // Dash force settings:
    [Header("Dash Variables")]
    public float dashForce = 50f;
    public float dashUpwardForce = 2f;

    // Timing settings:
    public float dashDuration = 0.4f;
    public float dashCooldown = 1.2f;
    private float lastDashTime = -10f;
    private float dashTimeRemaining;

    // State flags:
    private bool isDashing = false;
    private bool hasLiftedPaw = false; 

    public bool IsDashing
    {
        get { return isDashing; }
    }
    private bool canDash = true;
    private float originalWalkSpeed;

    private string currentSceneName;


    private void Start()
    {   
        getPlayerManager();
        controllerAssignment = FindFirstObjectByType<ControllerAssignment>();
        Scene currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        playerRespawn = FindFirstObjectByType<PlayerRespawn>();

        // Set the Controls Menu to keycaps or gamepad
        if (controllerAssignment.IsKeyboard())
        {
            controlsMenu = controlsMenuParent.transform.GetChild(0).gameObject;
            controlsMenu.SetActive(true);
            controlsMenuParent.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            controlsMenuParent.transform.GetChild(0).gameObject.SetActive(false);
            controlsMenu = controlsMenuParent.transform.GetChild(1).gameObject;
            controlsMenu.SetActive(true);
        }
        UpdateControlsMenu();

        movementSchemeSlider.value = playerManager.altMovement ? 1 : 0;
        UpdateMovementSchemeUI();

        player1RumbleSlider.value = player1Input.rumbleEnabled ? 1 : 0;
        player2RumbleSlider.value = player2Input.rumbleEnabled ? 1 : 0;
        UpdateRumbleUI();

        InitializeRestartUI();
    }

    bool tutOverlayDone()
    {
        return tutorialTextScript.overlayDone();
    }

    private void Update()
    {
        // if ((currentSceneName == "AtticLevel") && !tutOverlayDone() ){
        //     return;
        // }

        //print("P1 GlowJustPressed: " + player1Input.GetGlowJustPressed());

        if (!isViewingControlsMenu) {
            runGrabLogic();
            runJumpLogic();
            runNoiseLogic();
            runTailLogic(); 
            runRespawnLogic();
            if (!isTutorial)
            {
                runClimbLogic();
                runPawLogic();
                runDashLogic();
            }
        }

        //if (isGrabbing && isDraggableObject) enforceAngleRestriction();

        if (inConfirmationPhase) {
            HandleConfirmationInput();
        }
        else {
            runControlsMenuLogic();

            if (isViewingControlsMenu) {
                HandleRestartRadialFill();
            }
        }

        // This makes the grabText and climbText float :3
        grabText.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.0005f, 0);
        climbText.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.0005f, 0);
    }

    private void FixedUpdate()
    {
        // if (isClimbing)
        // {
        //     // Apply climbing movement in FixedUpdate for smooth physics
        //     frontRb.linearVelocity = climbDirection;
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        string frontSpecies = P1.IsFront ? P1.Species : P2.Species;

        if (other.CompareTag("Climbable") && frontSpecies == "cat")
        {
            // Show UI Popover
            showClimbText(other.gameObject);

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
        } else if ((other.CompareTag("Grabbable") || other.CompareTag("Draggable")) && !isGrabbing && frontSpecies == "dog")
        {
            grabbableObject = other;
            
            // Show UI Popover
            showGrabText(other.gameObject);
            
            canGrab = true;
        } else if (other.CompareTag("FrontPaw") && !isPaw)
        {
            showPawText(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            // Hide UI popover
            hideClimbText();

            isNearClimbable = false;

            if (isClimbing)
            {
                StopClimbing();
            }
        } else if (other.CompareTag("Grabbable") || other.CompareTag("Draggable"))
        {
            // Hide UI Popover
            hideGrabText();

            canGrab = false;

            if (!isGrabbing)
            {
                grabbableObject = null;
            }
        } else if (other.CompareTag("FrontPaw"))
        {
            hidePawText();
        }
    }

    private void getPlayerManager()
    {
        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();

        // Now you can access Player details like P1.Species
        if (playerManager != null)
        {
            UnityEngine.Debug.Log("P1 Species: " + playerManager.P1.Species);
            UnityEngine.Debug.Log("P2 Species: " + playerManager.P2.Species);

            P1 = playerManager.P1;
            P2 = playerManager.P2;

            frontHalf = playerManager.getFrontHalf();
            backHalf = playerManager.getBackHalf();

            frontRb = frontHalf.GetComponent<Rigidbody>();
            backRb = backHalf.GetComponent<Rigidbody>();

            climbMovementScript = frontHalf.GetComponentInChildren<ClimbMovement>();
            mouthRiggingScript = frontHalf.GetComponentInChildren<MouthRigging>();
            tailRiggingScript = backHalf.GetComponentInChildren<TailRigging>();
            pawRiggingScript = frontHalf.GetComponentInChildren<PawRigging>();
            hindRiggingScript = backHalf.GetComponentInChildren<HindLegsRigging2>();
            climbRiggingScript = frontHalf.GetComponentInChildren<ClimbRigging>();
            grabRiggingScript = frontHalf.GetComponentInChildren<GrabRigging>();
        }
        else
        {
            UnityEngine.Debug.Log("Error in fetching playerManager");
        }
    }

    public void RefreshAfterSwitch(PlayerManager pm)
    {
        // 1) Update references to P1 and P2 based on the PlayerManager
        P1 = pm.P1;
        P2 = pm.P2;

        // 2) Update references to the new front/back halves
        frontHalf = pm.getFrontHalf();
        backHalf = pm.getBackHalf();
        frontRb = frontHalf.GetComponent<Rigidbody>();
        backRb = backHalf.GetComponent<Rigidbody>();

        UpdateControlsMenu();
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

        // Charged Jump
        else if (((player1Special && !P1.IsFront && P1.Species == "cat") || 
                (player2Special && !P2.IsFront && P2.Species == "cat")) && !isTutorial)
        {
            tryChargedJump(chargedJumpForce, chargedJumpCooldown);
        }
    }

    private void tryStartJump(float jumpForce, float jumpCooldown)
    {
        if (Time.time - lastJumpTime > jumpCooldown)
        {
            if (playerManager.getJoint() != null)
            {
                frontRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            backRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
        }
    }

    public void tryChargedJump(float jumpForce, float jumpCooldown)
    {
        if (Time.time - lastJumpTime > jumpCooldown)
        {
            if (playerManager.getJoint() != null)
            {
                frontRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            backRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;

            StartCoroutine(FloatyJumpEffect());
        }
    }

    private IEnumerator FloatyJumpEffect()
    {
        float floatyDuration = 1f; // Duration of floaty effect
        float elapsed = 0f;
        
        // Apply small upward forces while in early part of jump
        while (elapsed < floatyDuration && backRb.linearVelocity.y > 0)
        {
            // Apply a small upward force
            float floatyForce = 2.0f; // Adjust this value
            backRb.AddForce(Vector3.up * floatyForce, ForceMode.Force);
            if (playerManager.getJoint() != null)
            {
                frontRb.AddForce(Vector3.up * floatyForce, ForceMode.Force);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

////////////////////////////////////// Noise Logic /////////////////////////////////////
    private void runNoiseLogic()
    {
        // if ((Input.GetKey(KeyCode.X) && P1.IsFront) || ((Input.GetKey(KeyCode.Period)) && P2.IsFront))
        if ((player1Input.GetSoundTailJustPressed() && P1.IsFront) || (player2Input.GetSoundTailJustPressed() && P2.IsFront))
        {
            string frontSpecies = P1.IsFront ? P1.Species : P2.Species;

            if (frontSpecies == "cat") {
                AudioManager.Instance.PlayPlayerMeowSFX();
            }
            else {
                AudioManager.Instance.PlayPlayerBarkSFX();
            }

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

        //Start climbing when front cat player presses special near climbable
        if (isNearClimbable && !isClimbing)
        {
            if ((player1Special && P1.IsFront) || (player2Special && P2.IsFront))
            {
                
                if (climbMovementScript.checkClimb()){
                    StartClimbing();
                }
                
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
        UnityEngine.Debug.Log("in start climbing function");
        climbRiggingScript.climb();
        isClimbing = true;
        playerManager.setIsClimb(true);

        frontRb.useGravity = false;
        // Zero out current velocities
        frontRb.linearVelocity = Vector3.zero;
        frontRb.angularVelocity = Vector3.zero;
        // Freeze rotation while climbing
        frontRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void StopClimbing()
    {
        UnityEngine.Debug.Log("in end climbing function");
        climbRiggingScript.release();
        isClimbing = false;
        playerManager.setIsClimb(false);
        frontRb.useGravity = true;
        frontRb.constraints = RigidbodyConstraints.None;
    }

////////////////////////////////////////// Grab Action ///////////////////////////////////////////////
    
    private void determineGrabType() 
    {
        if (grabbableObject == null)
        {
            UnityEngine.Debug.LogWarning("Attempted to determine grab type with no grabbable object");
            return;
        }
        
        // Calculate the dog's mouth position (for grab point calculation)
        ///mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
        mouthPosition = objectGrabPoint.position;

        // Reset all references to ensure clean state
        currentGrabPoint = null;
        currentGrabbableObject = null;
        grabPoint = Vector3.zero;
        isDraggableObject = false;
        
        // CASE 1: Check if this is a specific grab point (like a rug corner)
        currentGrabPoint = grabbableObject.GetComponent<GrabPoint>();
        
        if (currentGrabPoint != null)
        { 
            // This is a grab point (like a rug corner)
            
            // Make sure it has a valid parent to grab
            if (currentGrabPoint.parentRigidbody == null)
            {
                UnityEngine.Debug.LogWarning($"Grab point {grabbableObject.name} has no parent rigidbody assigned");
                return;
            }
            
            // Use the grab point's position as the connection point
            grabPoint = currentGrabPoint.transform.position;

            isDraggableObject = currentGrabPoint.grabBehavior == GrabPoint.GrabBehavior.Draggable;
            
            // Log for debugging
            UnityEngine.Debug.Log($"Grabbing {grabbableObject.name} as a grab point. Parent: {currentGrabPoint.parentRigidbody.name}");
            
            return;
        }
        
        // CASE 2: Check if it's a directly grabbable object with custom properties
        currentGrabbableObject = grabbableObject.GetComponent<GrabbableObject>();
        
        if (currentGrabbableObject != null)
        {
            // This is a grabbable object with custom properties

            isDraggableObject = currentGrabbableObject.grabBehavior == GrabbableObject.GrabBehavior.Draggable;
            
            if (isDraggableObject){
                mouthPosition = objectDragPoint.position;
            } else{
                mouthPosition = objectGrabPoint.position;
            }
            // Use closest point on the collider as the grab point
            grabPoint = grabbableObject.ClosestPoint(mouthPosition);
            
            //If closest point is at the center (inside the collider), use a point on the surface
            if (Vector3.Distance(grabPoint, grabbableObject.transform.position) < 0.01f)
            {
                // Cast a ray from mouth to object to find surface point
                Vector3 direction = (grabbableObject.transform.position - mouthPosition).normalized;
                Ray ray = new Ray(mouthPosition, direction);
                UnityEngine.Debug.DrawRay(ray.origin, direction, Color.red, 10.0f);
                
                if (grabbableObject.Raycast(ray, out RaycastHit hitInfo, 10f))
                {
                    grabPoint = hitInfo.point;
                }
                else
                {
                    // Fallback: use a point slightly offset from center toward mouth
                    grabPoint = grabbableObject.transform.position - direction * 0.1f;
                }
            }
            
            UnityEngine.Debug.Log($"Grabbing {grabbableObject.name} as a grabbable object with custom properties at point {grabPoint}");
            
            return;
        }
        
        // CASE 3: Basic grabbable object with no special components
        // Use closest point on the collider as the grab point
        grabPoint = grabbableObject.ClosestPoint(mouthPosition);
        
        // Same surface point detection as above
        if (Vector3.Distance(grabPoint, grabbableObject.transform.position) < 0.01f)
        {
            Vector3 direction = (grabbableObject.transform.position - mouthPosition).normalized;
            Ray ray = new Ray(mouthPosition, direction);
            
            if (grabbableObject.Raycast(ray, out RaycastHit hitInfo, 10f))
            {
                grabPoint = hitInfo.point;
            }
            else
            {
                grabPoint = grabbableObject.transform.position - direction * 0.1f;
            }
        }
        
        UnityEngine.Debug.Log($"Grabbing {grabbableObject.name} as a basic grabbable object at point {grabPoint}");
    }

    private void runGrabLogic() 
    {
        bool player1Special = player1Input.GetSpecialActionPressed();
        bool player2Special = player2Input.GetSpecialActionPressed();

        string frontSpecies = P1.IsFront? P1.Species: P2.Species;
        bool isSpecialReleased = (P1.IsFront && !player1Special) || (P2.IsFront && !player2Special);

        // Only dogs can grab
        if (frontSpecies != "dog") return;

        if (!isGrabbing) {
            if ((player1Special && P1.IsFront) || (player2Special && P2.IsFront))
            {
                // Only try to grab if there's a valid grabbable object
                if (canGrab && grabbableObject != null) {
                    UnityEngine.Debug.Log("Grabbed item");
                    isGrabbing = true;
                    grabObject();
                }
            }
        } 
        else if (isSpecialReleased && isGrabbing) {
            grabRiggingScript.release();
            releaseGrabbedObject();
            isGrabbing = false;
        }
        else if (isGrabbing) {
            // Only update joint for non-draggable objects or if the joint exists
            if ((!useHybridGrabSystem || !isDraggableObject) && grabJoint != null) {
                updateGrabJoint();
            }
            // DragController handles its own updates in FixedUpdate
        }
    }


    private void grabObject() 
    {
        determineGrabType();
        if (grabPoint == Vector3.zero) {
            UnityEngine.Debug.LogWarning("No grab point found");
            return;
        }

        Rigidbody targetRigidbody = getTargetRigidBody();

        if (targetRigidbody == null) {
            UnityEngine.Debug.LogWarning("No target rigidbody found");
            return;
        }

        // Store the initial direction from pet to grab point
        // We only care about horizontal direction, so zero out the y component
        Vector3 petToGrab = grabPoint - frontHalf.transform.position;
        petToGrab.y = 0;
        initialGrabDirection = petToGrab.normalized;

        // Apply movement restrictions regardless of grab method
        originalPlayerSpeed = playerManager.walkSpeed;
        applyMovementPenalty();
        applyTurnRestriction();

        // Set up the appropriate grab system based on object type
        if (useHybridGrabSystem && isDraggableObject) {
            // Use drag controller for draggable objects
            setupDragController(targetRigidbody);
        }
        else {
            // Use joints for portable objects
            setupGrabJoint(targetRigidbody);
        }

        grabText.SetActive(false);
    }

    private void enforceAngleRestriction()
    {
        if (!isGrabbing || initialGrabDirection == Vector3.zero) return;
        
        // Get the current facing direction of the pet (in world space)
        Vector3 currentFacing = frontHalf.transform.forward;
        currentFacing.y = 0;
        currentFacing.Normalize();
        
        // Get the current direction to the grab point (in world space)
        Vector3 currentGrabDirection;
        
        if (useHybridGrabSystem && isDraggableObject && dragController != null)
        {
            // For drag controller, use the actual object position
            Vector3 worldGrabPoint = dragController.gameObject.transform.TransformPoint(dragController.forceApplicationPoint);
            currentGrabDirection = worldGrabPoint - frontHalf.transform.position;
        }
        else if (grabJoint != null)
        {
            // For joint system, use the connected body position
            currentGrabDirection = grabJoint.connectedBody.transform.position - frontHalf.transform.position;
        }
        else
        {
            return; // No valid reference point
        }
        
        // Zero out vertical component and normalize
        currentGrabDirection.y = 0;
        currentGrabDirection.Normalize();

        float turnRestriction = 0;
        if (currentGrabPoint != null) {
            turnRestriction = currentGrabPoint.turnRestriction;
        }
        else if (currentGrabbableObject != null) {
            turnRestriction = currentGrabbableObject.turnRestriction;
        }
        
        // Calculate the angle between the pet's forward and the direction to the grab point
        float currentAngle = Vector3.SignedAngle(currentFacing, currentGrabDirection, Vector3.up);

        // Check if we're beyond the maximum angle
        if (Mathf.Abs(currentAngle) > maxTurnAngle)
        {
            // Get turning input from player controls
            bool player1IsFront = P1.IsFront;
            Vector2 frontPlayerInput = player1IsFront ? player1Input.GetMoveInput() : player2Input.GetMoveInput();
            float turnInputRaw = frontPlayerInput.x;

            // Calculate the direction we should face to turn toward the object
            Vector3 targetDirection = currentGrabDirection;
            
            // Calculate what the new forward direction would be after turning
            // For this, we need to create a rotation based on player input
            Quaternion turnRotation = Quaternion.Euler(0, turnInputRaw * playerManager.turnSpeed * Time.deltaTime, 0);
            Vector3 newForward = turnRotation * currentFacing;
            
            // Compare current dot product (how much we're facing toward the object)
            // with new dot product (how much we would face toward the object after turning)
            float currentDot = Vector3.Dot(currentFacing, targetDirection);
            float newDot = Vector3.Dot(newForward, targetDirection);
            
            // If the new direction faces more toward the object (higher dot product)
            if (newDot > currentDot)
            {
                // Allow turning toward the object
                playerManager.turnSpeed = originalTurnSpeed;
            }
            else
            {
                // Block turning away from the object
                playerManager.turnSpeed = 0;
            }
        }
        else
        {
            // Within acceptable angle range, use normal turn restriction
            playerManager.turnSpeed = originalTurnSpeed * (1 - turnRestriction);
        }
    }
     private void setupDragController(Rigidbody targetRigidbody)
    {
        // Get or add a DragController to the target
        dragController = targetRigidbody.gameObject.GetComponent<DragController>();
        if (dragController == null)
        {
            dragController = targetRigidbody.gameObject.AddComponent<DragController>();
        }
        
        // Calculate local grab point relative to object
        Vector3 localGrabPoint = targetRigidbody.transform.InverseTransformPoint(grabPoint);
        
        // Initialize the drag controller
        dragController.Initialize(objectGrabPoint, localGrabPoint);
        
        // Get tether settings from the appropriate component
        float springConstant = 1000f; // Default values
        float dampingConstant = 50f;
        float maxTetherForce = 2000f;
        float weight = 5f;
        float resistance = 0.5f;
        
        if (currentGrabPoint != null)
        {
            weight = currentGrabPoint.grabWeight;
            resistance = currentGrabPoint.dragResistance;
            springConstant = currentGrabPoint.springConstant;
            dampingConstant = currentGrabPoint.dampingConstant;
            maxTetherForce = currentGrabPoint.maxTetherForce;
        }
        else if (currentGrabbableObject != null)
        {
            weight = currentGrabbableObject.grabWeight;
            resistance = currentGrabbableObject.dragResistance;
            springConstant = currentGrabbableObject.springConstant;
            dampingConstant = currentGrabbableObject.dampingConstant;
            maxTetherForce = currentGrabbableObject.maxTetherForce;
        }
        
        // Configure the drag controller with the correct settings
        dragController.ConfigureFromProperties(springConstant, dampingConstant, maxTetherForce);
        
        // Set visual state for dragging
        grabRiggingScript.drag();
        
        UnityEngine.Debug.Log($"Set up drag controller for {targetRigidbody.gameObject.name} with weight: {weight}, resistance: {resistance}, spring: {springConstant}, damping: {dampingConstant}, maxForce: {maxTetherForce}");
    }

    private void setupGrabJoint(Rigidbody targetRigidbody)
    {
        // Create the joint
        grabJoint = gameObject.AddComponent<ConfigurableJoint>();
        grabJoint.connectedBody = targetRigidbody;

        // Configure joint physics
        configureJoint(grabJoint);

        // Get mouth position
        
        
        // Determine if this is a portable object
        bool isPortable = false;
        
        if (currentGrabPoint != null) {
            isPortable = currentGrabPoint.grabBehavior == GrabPoint.GrabBehavior.Portable;
        } else if (currentGrabbableObject != null) {
            isPortable = currentGrabbableObject.grabBehavior == GrabbableObject.GrabBehavior.Portable;
        }
        
        if (isPortable) {
            mouthPosition = objectGrabPoint.position;
            // Set the anchor point at the dog's mouth
            grabJoint.anchor = transform.InverseTransformPoint(mouthPosition);
            
            // For portable objects, make them move to the dog's mouth
            grabJoint.autoConfigureConnectedAnchor = false;
            grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(grabPoint);

            // grabJoint.xMotion = ConfigurableJointMotion.Limited;
            // grabJoint.yMotion = ConfigurableJointMotion.Limited;
            // grabJoint.zMotion = ConfigurableJointMotion.Limited;

        } else {
            mouthPosition = objectDragPoint.position;
            // For draggable objects using the joint system
            grabRiggingScript.drag();
            
            // Set the anchor point at the dog's mouth
            grabJoint.anchor = transform.InverseTransformPoint(mouthPosition);
            
            // Keep the connection at the grab point
            grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(grabPoint);
        }
        
        UnityEngine.Debug.Log($"Set up grab joint for {targetRigidbody.gameObject.name} with isPortable: {isPortable}");
    }



    private Rigidbody getTargetRigidBody() {
        if (currentGrabPoint != null && currentGrabPoint.parentRigidbody != null) {
            return currentGrabPoint.parentRigidbody;
        }

        Rigidbody directRigidbody = grabbableObject.GetComponent<Rigidbody>();
        if (directRigidbody != null) {
            return directRigidbody;
        }  

        if (grabbableObject.transform.parent != null) {
            return grabbableObject.transform.parent.GetComponent<Rigidbody>();
        }

        return null;
    }

    private void configureJoint(ConfigurableJoint joint) {
        if (currentGrabPoint != null) {
            currentGrabPoint.ConfigureJoint(joint);
            return;
        }

        if (currentGrabbableObject != null) {
            currentGrabbableObject.ConfigureJoint(joint);
            return;
        }

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;   
        joint.zMotion = ConfigurableJointMotion.Limited;

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        JointDrive posDrive = new JointDrive
        {
            positionSpring = defaultSpringForce,
            positionDamper = defaultDamperForce,
            maximumForce = float.MaxValue
        };

        joint.xDrive = posDrive;   
        joint.yDrive = posDrive;
        joint.zDrive = posDrive;

    }

    private void applyMovementPenalty() {

        float penalty = 0f;

        if (currentGrabPoint != null) {
            penalty = currentGrabPoint.movementPenalty;
        }
        else if (currentGrabbableObject != null) {
            penalty = currentGrabbableObject.movementPenalty;
        }
        else {
            penalty = 0.5f;
        }

        playerManager.walkSpeed *= (1f -penalty);
    }

    private void applyTurnRestriction() {

        if (!turnRestricted) {
            originalTurnSpeed = playerManager.turnSpeed;
        }

        bool preventTurning = false;
        float turnRestriction = 0f;

        if (currentGrabPoint != null) {
            preventTurning = currentGrabPoint.preventTurning;
            turnRestriction = currentGrabPoint.turnRestriction;
        }
        else if (currentGrabbableObject != null) {
            preventTurning = currentGrabbableObject.preventTurning;
            turnRestriction = currentGrabbableObject.turnRestriction;
        }

        // Apply turn restriction - we've simplified this to avoid rotation issues
        if (preventTurning) {
            playerManager.turnSpeed = 0f;
        } else {
            playerManager.turnSpeed = originalTurnSpeed * (1f - turnRestriction);
        }

        turnRestricted = true;
    }

    private void updateGrabJoint() {

        if (useHybridGrabSystem && isDraggableObject) {
            // For drag controller, nothing to update here
            // The drag controller handles the physics in its own FixedUpdate
            return;
        }

        if (grabJoint == null && isGrabbing) {
            releaseGrabbedObject();
            return;
        }
    }
    private void releaseGrabbedObject() 
    {
        UnityEngine.Debug.Log("Released item");

        initialGrabDirection = Vector3.zero;

        if (useHybridGrabSystem && isDraggableObject) {
            if (dragController != null) {
                // First stop the dragging behavior
                dragController.StopDragging();
                
                // Important: Destroy the component completely when done
                Destroy(dragController);
                dragController = null;
            }
        }
        else {
            if (grabJoint != null) {
                Destroy(grabJoint);
                grabJoint = null;
            }
        }

        // Restore original movement values
        playerManager.walkSpeed = originalPlayerSpeed;
        if (turnRestricted) {
            playerManager.turnSpeed = originalTurnSpeed;
            turnRestricted = false;
        }
        
        // Reset state variables
        isGrabbing = false;
        isDraggableObject = false;
        currentGrabPoint = null;
        currentGrabbableObject = null;
        
        // Reset visual state
        grabRiggingScript.release();
    }

    // Handle joint breaking automatically
    private void OnJointBreak(float breakForce)
    {
        if (isGrabbing)
        {
            releaseGrabbedObject();
        }
    }

    ////////////////////////////////////////// Hind Legs Action ///////////////////////////////////////////////
    private void runHindLegsLogic()
    {
        // Get special action input for hind legs (dog back special)
        bool player1Special = player1Input.GetSpecialActionPressed();
        bool player2Special = player2Input.GetSpecialActionPressed();
        // Check for dog species in back position
        string backSpecies = P1.IsFront ? P2.Species : P1.Species;
        bool isSpecialReleased = (!P1.IsFront && !player1Special) || (!P2.IsFront && !player2Special);
        
        // Only cats can climb
        if (backSpecies != "dog") return;
        
        // if (!isStanding){
        //     UnityEngine.Debug.Log("not standing");
        // } else{
        //     UnityEngine.Debug.Log("is standing");
        // }
        // Start climbing when front cat player presses special near climbable
        if (!isStanding)
        {
            if ((player1Special && !P1.IsFront) || (player2Special && !P2.IsFront))
            {
                isStanding = true;
                hindRiggingScript.stand(); 
            }
        
        } else if (isSpecialReleased && isStanding)
        {
            isStanding = false;
           hindRiggingScript.release();
        }
        
    }

    ////////////////////////////////////////////// Dash Action //////////////////////////////////////////////
    // Replace the existing runHindLegsLogic() with this implementation
    // New separate function for dash logic
    private void runDashLogic()
    {
        // Get special action input for dash (dog back special)
        bool player1Special = player1Input.GetSpecialActionPressed();
        bool player2Special = player2Input.GetSpecialActionPressed();
        
        // Check for dog species in back position
        string backSpecies = P1.IsFront ? P2.Species : P1.Species;
        
        // Only dogs can dash
        if (backSpecies != "dog") return;
        
        // Just track dash time remaining if dashing
        if (isDashing && dashTimeRemaining > 0)
        {
            dashTimeRemaining -= Time.deltaTime;
            
            // End dash when time expires
            if (dashTimeRemaining <= 0)
            {
                EndDash();
            }
        }
        
        // Check cooldown
        if (Time.time - lastDashTime < dashCooldown)
        {
            canDash = false;
        }
        else
        {
            canDash = true;
        }
        
        // Start dash when back dog player presses special
        if (canDash && !isDashing && ((player1Special && !P1.IsFront) || (player2Special && !P2.IsFront)))
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        lastDashTime = Time.time;
        dashTimeRemaining = dashDuration;
        
        // Get the rigidbody
        Rigidbody backRb = playerManager.getBackHalf().GetComponent<Rigidbody>();

        // Ensure continuous collision detection is enabled
        // frontRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // backRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Determine the dash direction.
        // For example, if the back half is the dashing side:
        Vector3 dashDirection = backHalf.transform.forward;  // You can modify this if you want directional input
        dashDirection.Normalize();
        
        // Calculate the dash impulse.
        // You can expose dashForce and dashUpwardForce as public variables in your class.
        Vector3 dashImpulse = dashDirection * dashForce;
        if (playerManager.getJoint() == null) {
            dashImpulse *= 0.5f; // Reduce the force if not attached to front half
        }  
        dashImpulse += Vector3.up * dashUpwardForce; // Optional upward component
        
        // Optionally reset velocity so the dash is consistent.
        // frontRb.linearVelocity = Vector3.zero;
        backRb.linearVelocity = Vector3.zero;
        
        // Apply the impulse force
        backRb.AddForce(dashImpulse, ForceMode.Impulse);
        
        // Optional: Play dash effects (sound, particles, etc.)
        PlayDashEffect();

        // Schedule the dash end after the dash duration.
        Invoke(nameof(EndDash), dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        
        // Optional: if you disabled gravity during dash, re-enable it
        Rigidbody frontRb = playerManager.getFrontHalf().GetComponent<Rigidbody>();
        Rigidbody backRb = playerManager.getBackHalf().GetComponent<Rigidbody>();
        frontRb.useGravity = true;
        backRb.useGravity = true;
        
        // Optional: stop dash effects, etc.
        StopDashEffect();
    }

    // Add visual/audio feedback for the dash
    private void PlayDashEffect()
    {
        // Play a sound effect
        AudioSource audioSource = backHalf.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = backHalf.AddComponent<AudioSource>();
        }
        
        // TODO: Add a sound effect for dash
        // audioSource.PlayOneShot(dashSound);
        
        // can add particle effects here
    }

    private void StopDashEffect()
    {
        // Stop any ongoing effects
        // Example:
        // if (dashEffectInstance != null)
        // {
        //     Destroy(dashEffectInstance);
        // }
    }

    ////////////////////////////////////////// Front Paws Action //////////////////////////////////////////////
    private void runPawLogic()
    {
        // if (((Input.GetKeyDown(KeyCode.Z) && P1.IsFront) || (Input.GetKeyDown(KeyCode.Comma) && P2.IsFront)))
        if ((player1Input.GetJumpPressed() && P1.IsFront) || (player2Input.GetJumpPressed() && P2.IsFront))
        {
            // If paw hasn't been lifted yet, call the liftPaw() method
            if (!hasLiftedPaw)
            {
                pawRiggingScript.liftPaw();
                hasLiftedPaw = true;  // Mark that paw has been lifted
                isPaw = true;
            }
        // } else if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Comma))
        } else if (!player1Input.GetJumpPressed() && !player2Input.GetJumpPressed())
        {
            hasLiftedPaw = false;
            isPaw = false;
        }
        
    }

////////////////////////////////////////// Tail Action ////////////////////////////////////////////////////
    private void runTailLogic()
    {
        // if (((Input.GetKeyDown(KeyCode.Z) && !P1.IsFront) || (Input.GetKeyDown(KeyCode.Period) && !P2.IsFront)))
        if ((player1Input.GetSoundTailPressed() && !P1.IsFront) || (player2Input.GetSoundTailPressed() && !P2.IsFront))
        {
            tailRiggingScript.useTail();
        }
        else{
            tailRiggingScript.naturalTailMovement();
        }

    }
     
////////////////////////////////////////// Respawn Action ////////////////////////////////////////////
    public void runRespawnLogic()
    {
        // Player 1 respawn logic
        if (player1Input.GetRespawnPressed())
        {
            // Increment hold time while button is pressed
            player1RespawnHoldTime += Time.deltaTime;
            
            // Check if hold time requirement met and not already respawning
            if (player1RespawnHoldTime >= REQUIRED_RESPAWN_HOLD_TIME && !player1Respawning)
            {
                player1Respawning = true;
                playerRespawn.Respawn(P1);
            }
        }
        else
        {
            // Reset hold time when button is released
            player1RespawnHoldTime = 0f;
            
            // Reset respawning flag
            player1Respawning = false;
        }

        // Player 2 respawn logic (identical to Player 1)
        if (player2Input.GetRespawnPressed())
        {
            player2RespawnHoldTime += Time.deltaTime;
            
            if (player2RespawnHoldTime >= REQUIRED_RESPAWN_HOLD_TIME && !player2Respawning)
            {
                player2Respawning = true;
                playerRespawn.Respawn(P2);
            }
        }
        else
        {
            player2RespawnHoldTime = 0f;
            
            player2Respawning = false;
        }
    }

////////////////////////////////////////// Actions UI ///////////////////////////////////////////////

    public void UpdateControlsMenu()
    {   
        // Update the UI elements based on current controller assignment
        if (controllerAssignment != null)
        {
            if (controllerAssignment.IsKeyboard()) {
                print("Is keyboard");
                return;
            }
            
            Transform P1Controls = controlsMenu.transform.GetChild(0).gameObject.transform;
            Transform P2Controls = controlsMenu.transform.GetChild(1).gameObject.transform;

            bool P1isFront = P1.IsFront;
            UnityEngine.Debug.Log("p1isfront is "+ P1isFront);
          
            P1Controls.GetChild(0).gameObject.SetActive(P1isFront);
            P1Controls.GetChild(1).gameObject.SetActive(!P1isFront);

            P2Controls.GetChild(0).gameObject.SetActive(!P1isFront);
            P2Controls.GetChild(1).gameObject.SetActive(P1isFront);
        }
    }

    private void showClimbText(GameObject other)
    {
        climbText.SetActive(true);
        climbText.transform.position = other.transform.position + (Vector3.forward * 0.06f);// - (Vector3.up * 0.10f);
        Vector3 pos = climbText.transform.position;
        climbText.transform.position = new Vector3(pos.x, frontHalf.transform.position.y + 0.5f, pos.z);

        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) climbText.transform.GetChild(0).gameObject.SetActive(true);
            else climbText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else climbText.transform.GetChild(2).gameObject.SetActive(true);

        // Make text always face frontHalf
        climbText.transform.LookAt(frontHalf.transform); 
        Vector3 rot = climbText.transform.rotation.eulerAngles;
        climbText.transform.rotation = Quaternion.Euler(0, rot.y, rot.z);
        
        // Flip the text to unmirror it
        // climbText.transform.rotation = Quaternion.Euler(0, climbText.transform.rotation.eulerAngles.y + 180, 0);
    }

    private void hideClimbText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) climbText.transform.GetChild(0).gameObject.SetActive(false);
            else climbText.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            climbText.transform.GetChild(2).gameObject.SetActive(false); 
        }

        climbText.SetActive(false);
    }

    private void showGrabText(GameObject other)
    {
        grabText.SetActive(true);
        grabText.transform.position = other.transform.position;// + (Vector3.forward * 0.05f) - (Vector3.up * 0.10f);

        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) grabText.transform.GetChild(0).gameObject.SetActive(true);
            else grabText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else grabText.transform.GetChild(2).gameObject.SetActive(true);

        // Make text always face frontHalf
        grabText.transform.LookAt(frontHalf.transform); 
        Vector3 rot = grabText.transform.rotation.eulerAngles;
        grabText.transform.rotation = Quaternion.Euler(0, rot.y, rot.z);
        
        // Flip the text to unmirror it
        // grabText.transform.rotation = Quaternion.Euler(0, grabText.transform.rotation.eulerAngles.y + 180, 0);
    }

    private void hideGrabText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) grabText.transform.GetChild(0).gameObject.SetActive(false);
            else grabText.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            grabText.transform.GetChild(2).gameObject.SetActive(false);
        }

        grabText.SetActive(false);
    }

    private void showPawText(GameObject other)
    {
        pawText.SetActive(true);
        pawText.transform.position = other.transform.position;// + (Vector3.forward * 0.05f) - (Vector3.up * 0.10f);

        Vector3 pos = pawText.transform.position;
        
        pawText.transform.position = new Vector3(pos.x, pos.y, frontHalf.transform.position.z) + (Vector3.forward * 0.3f);

        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) pawText.transform.GetChild(0).gameObject.SetActive(true);
            else pawText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else pawText.transform.GetChild(2).gameObject.SetActive(true);

        // Make text always face frontHalf
        pawText.transform.LookAt(frontHalf.transform); 
        Vector3 rot = pawText.transform.rotation.eulerAngles;
        pawText.transform.rotation = Quaternion.Euler(0, rot.y, rot.z);
        
    }

    private void hidePawText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) pawText.transform.GetChild(0).gameObject.SetActive(false);
            else pawText.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            pawText.transform.GetChild(2).gameObject.SetActive(false);
        }

        pawText.SetActive(false);
    }

    private void runControlsMenuLogic()
    {
        float rumbleConfirmationLow = 0.2f;
        float rumbleConfirmationHigh = 0.2f;
        float rumbleConfirmationDuration = 0.25f;

        if (player1Input.GetControlsMenuJustPressed() || player2Input.GetControlsMenuJustPressed())
        {
            isViewingControlsMenu = !isViewingControlsMenu;
            controlsMenuParent.SetActive(isViewingControlsMenu);

            if (isViewingControlsMenu) {
                movementSchemeSlider.value = playerManager.altMovement ? 1 : 0;
                UpdateMovementSchemeUI();

                UpdateRumbleUI();
            }
        }

        if (isViewingControlsMenu)
        {
            if (player1Input.GetGlowJustPressed() || player2Input.GetGlowJustPressed()) {
                playerManager.altMovement = !playerManager.altMovement;

                movementSchemeSlider.value = playerManager.altMovement ? 1 : 0;

                UpdateMovementSchemeUI();

                AudioManager.Instance.playUIClickSFX();
            }

            if (player1Input.GetSpecialActionJustPressed()) {
                player1Input.rumbleEnabled = !player1Input.rumbleEnabled;
                if (player1Input.rumbleEnabled) {
                    player1Input.TriggerRumble(rumbleConfirmationLow, rumbleConfirmationHigh, rumbleConfirmationDuration);
                }
                player1RumbleSlider.value = player1Input.rumbleEnabled ? 1 : 0;
                UpdateRumbleUI();
                
                AudioManager.Instance.playUIClickSFX();
            }

            if (player2Input.GetSpecialActionJustPressed()) {
                player2Input.rumbleEnabled = !player2Input.rumbleEnabled;
                if (player2Input.rumbleEnabled) {
                    player2Input.TriggerRumble(rumbleConfirmationLow, rumbleConfirmationHigh, rumbleConfirmationDuration);
                }
                player2RumbleSlider.value = player2Input.rumbleEnabled ? 1 : 0;
                UpdateRumbleUI();
                
                AudioManager.Instance.playUIClickSFX();
                
            }
            
        }
    }

    private void UpdateMovementSchemeUI()
    {
        bool isAltMovement = playerManager.altMovement;
        
        // Update text colors if they're assigned
        if (standardSchemeText != null && altSchemeText != null)
        {
            standardSchemeText.color = isAltMovement ? inactiveColor : activeColor;
            altSchemeText.color = isAltMovement ? activeColor : inactiveColor;
        }
    }

    private void UpdateRumbleUI()
    {
        // Update slider values to match current settings
        player1RumbleSlider.value = player1Input.rumbleEnabled ? 1 : 0;
        player2RumbleSlider.value = player2Input.rumbleEnabled ? 1 : 0;
        
        // Update background colors
        player1RumbleBackground.color = player1Input.rumbleEnabled ? rumbleEnabledColor : rumbleDisabledColor;
        player2RumbleBackground.color = player2Input.rumbleEnabled ? rumbleEnabledColor : rumbleDisabledColor;
    }

    private void InitializeRestartUI()
    {
        // Ensure all UI components are properly set
        if (radialFillImage != null)
        {
            radialFillImage.fillAmount = 0f;
        }
        
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        
        // Reset all state variables
        radialFillAmount = 0f;
        player1HoldingRestart = false;
        player2HoldingRestart = false;
        player1Confirmed = false;
        player2Confirmed = false;
        inConfirmationPhase = false;
        
        // Reset UI states
        UpdateRestartUI();
    }

    private void HandleRestartRadialFill()
    {
        if (!isViewingControlsMenu) return;

        print("Handling restart radial fill");
        
        // Check if players are holding the makeSoundTail button
        player1HoldingRestart = player1Input.GetSoundTailPressed();
        player2HoldingRestart = player2Input.GetSoundTailPressed();

        if (player1HoldingRestart || player2HoldingRestart)
        {
            print("Holding restart");

        }
        
        bool anyPlayerHolding = player1HoldingRestart || player2HoldingRestart;
        bool bothPlayersHolding = player1HoldingRestart && player2HoldingRestart;
        
        // Update fill amount based on player input
        if (anyPlayerHolding)
        {
            // Determine target fill amount
            float targetFill = bothPlayersHolding ? 1.0f : 0.5f;
            
            // If current fill is higher than target and we're not at both players holding,
            // decay to target fill instead of increasing
            if (radialFillAmount > targetFill && !bothPlayersHolding)
            {
                radialFillAmount -= decaySpeed * Time.deltaTime;
                radialFillAmount = Mathf.Max(radialFillAmount, targetFill);
            }
            else
            {
                // Fill up to target
                radialFillAmount += fillSpeed * Time.deltaTime;
                radialFillAmount = Mathf.Min(radialFillAmount, targetFill);
            }
        }
        else
        {
            // Both players released, decay to zero
            radialFillAmount -= decaySpeed * Time.deltaTime;
            radialFillAmount = Mathf.Max(radialFillAmount, 0f);
        }
        
        // Update UI
        if (radialFillImage != null)
        {
            radialFillImage.fillAmount = radialFillAmount;
        }
        
        // Check if we've hit the confirmation threshold
        if (radialFillAmount >= confirmThreshold && !inConfirmationPhase)
        {
            ShowRestartConfirmation();
        }
    }

    private void ShowRestartConfirmation()
    {
        inConfirmationPhase = true;
        
        // Reset confirmation states
        player1Confirmed = false;
        player2Confirmed = false;
        
        // Activate confirmation popup
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(true);
        }
        
        // Update the UI to show initial confirmation state
        UpdateConfirmationUI();
    }

    private void HandleConfirmationInput()
    {
        // Check for confirmation input (Jump) and cancel input (Glow)
        bool player1Confirm = player1Input.GetJumpJustPressed();
        bool player2Confirm = player2Input.GetJumpJustPressed();
        bool player1Cancel = player1Input.GetGlowJustPressed();
        bool player2Cancel = player2Input.GetGlowJustPressed();
        
        // Handle Player 1 confirmation toggle
        if (player1Confirm)
        {
            // Toggle confirmation status
            player1Confirmed = !player1Confirmed;
            AudioManager.Instance.playUIClickSFX();
            UpdateConfirmationUI();
        }
        
        // Handle Player 2 confirmation toggle
        if (player2Confirm)
        {
            // Toggle confirmation status
            player2Confirmed = !player2Confirmed;
            AudioManager.Instance.playUIClickSFX();
            UpdateConfirmationUI();
        }
        
        // Handle cancellation
        if (player1Cancel || player2Cancel)
        {
            CancelRestart();
            return;
        }
        
        // Check if both players have confirmed
        if (player1Confirmed && player2Confirmed)
        {
            // Both players confirmed, perform restart
            StartCoroutine(PerformRestart());
        }
    }

    private void UpdateConfirmationUI()
    {
        // Update Player 1 confirmation status
        if (player1ConfirmText != null && player1ConfirmedText != null)
        {
            player1ConfirmText.gameObject.SetActive(!player1Confirmed);
            player1ConfirmedText.gameObject.SetActive(player1Confirmed);
        }
        
        // Update Player 2 confirmation status
        if (player2ConfirmText != null && player2ConfirmedText != null)
        {
            player2ConfirmText.gameObject.SetActive(!player2Confirmed);
            player2ConfirmedText.gameObject.SetActive(player2Confirmed);
        }
    }

    private void UpdateRestartUI()
    {
        if (radialFillImage != null)
        {
            radialFillImage.fillAmount = radialFillAmount;
        }
    }

    private void CancelRestart()
    {
        inConfirmationPhase = false;
        
        // Hide the confirmation popup
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        
        // Reset fill amount
        radialFillAmount = 0f;
        UpdateRestartUI();
        
    }

    private IEnumerator PerformRestart()
    {
        
        // Show a loading message or transition effect
        if (confirmPromptText != null)
        {
            confirmPromptText.text = "Restarting game...";
        }
        
        // Wait a short time for feedback
        yield return new WaitForSeconds(1.0f);
        
        // Load the initial scene
        Destroy(AudioManager.Instance.gameObject);
        Destroy(TaskManager.Instance.gameObject);

        SceneManager.LoadScene(initialSceneName);
    }

}
