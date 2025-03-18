using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

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
    public ControllerAssignment controllerAssignment;

    [Header("Jumping Variables")]
    private float jumpForce = 20f;
    public float jumpCooldown = 0.5f;
    private float chargedJumpForce = 35f;
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
    //public Transform objectDragPoint;
    private bool isStanding = false;

    [Header("UI Variables")]
    public GameObject grabText;
    public GameObject climbText;
    public GameObject controlsMenu;
    private bool isViewingControlsMenu = false;

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
    private bool canDash = true;
    private float originalWalkSpeed;

    private string currentSceneName;

    private void Start()
    {   
        getPlayerManager();
        Scene currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        // Set the Controls Menu to keycaps or gamepad
        if (controllerAssignment.IsKeyboard())
        {
            controlsMenu.transform.GetChild(0).gameObject.SetActive(true);
            controlsMenu.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            controlsMenu.transform.GetChild(0).gameObject.SetActive(false);
            controlsMenu.transform.GetChild(1).gameObject.SetActive(true);
        }
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

        runJumpLogic();
        runNoiseLogic();
        runTailLogic();

        runGrabLogic();

        if (!isTutorial)
        {
            runClimbLogic();
            runPawLogic();
            // runHindLegsLogic();
            runDashLogic();
        }

        

        //if (isGrabbing && isDraggableObject) enforceAngleRestriction();

        runControlsMenuLogic();

        // This makes the grabText and climbText float :3
        grabText.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.0005f, 0);
        climbText.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.0005f, 0);
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

    private void tryChargedJump(float jumpForce, float jumpCooldown)
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
            grabPoint = grabbableObject.transform.position;

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
            
            // Use closest point on the collider as the grab point
            grabPoint = grabbableObject.ClosestPoint(mouthPosition);
            
            //If closest point is at the center (inside the collider), use a point on the surface
            if (Vector3.Distance(grabPoint, grabbableObject.transform.position) < 0.01f)
            {
                // Cast a ray from mouth to object to find surface point
                Vector3 direction = (grabbableObject.transform.position - mouthPosition).normalized;
                Ray ray = new Ray(mouthPosition, direction);
                
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
        mouthPosition = objectGrabPoint.position;
        
        // Determine if this is a portable object
        bool isPortable = false;
        
        if (currentGrabPoint != null) {
            isPortable = currentGrabPoint.grabBehavior == GrabPoint.GrabBehavior.Portable;
        } else if (currentGrabbableObject != null) {
            isPortable = currentGrabbableObject.grabBehavior == GrabbableObject.GrabBehavior.Portable;
        }
        
        if (isPortable) {
            // Set the anchor point at the dog's mouth
            grabJoint.anchor = transform.InverseTransformPoint(mouthPosition);
            
            // For portable objects, make them move to the dog's mouth
            grabJoint.autoConfigureConnectedAnchor = false;
            grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(targetRigidbody.transform.position);

            // grabJoint.xMotion = ConfigurableJointMotion.Limited;
            // grabJoint.yMotion = ConfigurableJointMotion.Limited;
            // grabJoint.zMotion = ConfigurableJointMotion.Limited;

        } else {
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
     

////////////////////////////////////////// Actions UI ///////////////////////////////////////////////

    public void UpdateControlsMenu()
    {
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
        
        // Update the UI elements based on current controller assignment
        if (controllerAssignment != null)
        {
            Transform P1Controls = controlsMenu.transform.GetChild(0).gameObject.transform;
            Transform P2Controls = controlsMenu.transform.GetChild(1).gameObject.transform;

            bool P1isFront = P1.IsFront;
          
            P1Controls.GetChild(0).gameObject.SetActive(P1isFront);
            P1Controls.GetChild(1).gameObject.SetActive(!P1isFront);

            P2Controls.GetChild(0).gameObject.SetActive(!P1isFront);
            P2Controls.GetChild(1).gameObject.SetActive(P1isFront);
        
            // controlsMenu.transform.GetChild(0).gameObject.SetActive(false);
            // controlsMenu.transform.GetChild(1).gameObject.SetActive(true);
            

            
        }
    }

    private void showClimbText(GameObject other)
    {
        climbText.SetActive(true);
        climbText.transform.position = other.transform.position + (Vector3.forward * 0.05f);// - (Vector3.up * 0.10f);

        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) climbText.transform.GetChild(0).gameObject.SetActive(true);
            else climbText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else climbText.transform.GetChild(2).gameObject.SetActive(true);

        // Make text always face frontHalf
        climbText.transform.LookAt(frontHalf.transform); 
        
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

    private void runControlsMenuLogic()
    {
        if ((player1Input.GetControlsMenuPressed() || player2Input.GetControlsMenuPressed() || Input.GetKeyDown(KeyCode.I)) && !isViewingControlsMenu)
        {
            controlsMenu.SetActive(true);
            isViewingControlsMenu = true;
        }
        else if ((player1Input.GetGlowPressed() || player2Input.GetGlowPressed() || Input.GetKeyDown(KeyCode.I)) && isViewingControlsMenu)
        {
            controlsMenu.SetActive(false);
            isViewingControlsMenu = false;
        }
    }
}

