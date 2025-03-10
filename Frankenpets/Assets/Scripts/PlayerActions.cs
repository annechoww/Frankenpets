using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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
    // private GameObject climbText;

    [Header("Dash Variables")]
    public float dashSpeedMultiplier = 10f;  // Multiplier for player's movement speed during dash
    public float dashDuration = 0.4f;        // How long the dash lasts
    public float dashCooldown = 1.2f;        // Cooldown before next dash
    private bool isDashing = false;          // If player is currently dashing
    private bool canDash = true;             // If dash ability is available
    private float lastDashTime = -10f;       // Time since last dash
    private float dashTimeRemaining;         // Remaining time in current dash
    private float originalWalkSpeed;         // Store original walk speed to restore after dash 

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
    // private GameObject grabText;
    private Vector3 mouthPosition;
    private Vector3 mouthDirection;
    private Vector3 grabPoint; // Position where the joint connects

    private GrabPoint currentGrabPoint;
    private GrabbableObject currentGrabbableObject;
    private bool isDraggableObject = false;

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

    [Header("Glowing Objects Variables")]
    public Color dogGlowColor = Color.yellow;
    public Color catGlowColor = Color.red;
    private float glowIntensity = 2.0f;
    private float glowDuration = 1.5f;
    private Renderer[] dogObjects;
    private Renderer[] catObjects;

    [Header("UI Variables")]
    public GameObject grabText;
    public GameObject climbText;
    public GameObject controlsMenu;
    private bool isViewingControlsMenu = false;

    private void OnValidate() {
    // If dashSpeedMultiplier is still the old default, update it to the new default
    if (Mathf.Approximately(dashSpeedMultiplier, 3.5f)) {
        dashSpeedMultiplier = 10f;
    }
}

    private void Start()
    {   
        // climbText = GameObject.FindGameObjectWithTag("ClimbText");
        // grabText = GameObject.FindGameObjectWithTag("GrabText");
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
        getPlayerManager();

        // Find all task objects
        GameObject[] grabbableObjects = GameObject.FindGameObjectsWithTag("Grabbable");
        GameObject[] draggableObjects = GameObject.FindGameObjectsWithTag("Draggable");
        dogObjects = new Renderer[grabbableObjects.Length + draggableObjects.Length];

        for (int i = 0; i < grabbableObjects.Length; i++)
        {
            dogObjects[i] = grabbableObjects[i].GetComponent<Renderer>();
        }

        for (int i = 0; i < draggableObjects.Length; i++)
        {
            dogObjects[grabbableObjects.Length + i - 1] = draggableObjects[i].GetComponent<Renderer>();
        }

        // Find all climbable objects
        GameObject[] climbableObjects = GameObject.FindGameObjectsWithTag("Climbable");
        catObjects = new Renderer[climbableObjects.Length];

        for (int i = 0; i < climbableObjects.Length; i++)
        {
            catObjects[i] = climbableObjects[i].GetComponent<Renderer>();
        }
        //////////////////

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

    private void Update()
    {
        runJumpLogic();
        runNoiseLogic();
        runClimbLogic();
        runGrabLogic();
        runTailLogic();
        runPawLogic();
        // runHindLegsLogic();
        runDashLogic();
        runGlowLogic();
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
        print("P1 Species: " + P1.Species);
        print("P2 Species: " + P2.Species);
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
        else if ((player1Special && !P1.IsFront && P1.Species == "cat") || 
                (player2Special && !P2.IsFront && P2.Species == "cat"))
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
        
        // Configure the parameters based on object properties
        float weight = 5f; // Default medium weight
        float resistance = 0.5f; // Default medium resistance
        
        if (currentGrabPoint != null)
        {
            weight = currentGrabPoint.grabWeight;
            resistance = currentGrabPoint.dragResistance;
        }
        else if (currentGrabbableObject != null)
        {
            weight = currentGrabbableObject.grabWeight;
            resistance = currentGrabbableObject.dragResistance;
        }
        
        dragController.ConfigureFromProperties(weight, resistance);
        
        // Set visual state for dragging
        grabRiggingScript.drag();
        
        UnityEngine.Debug.Log($"Set up drag controller for {targetRigidbody.gameObject.name} with weight: {weight}, resistance: {resistance}");
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
    
    // Store the original walk speed to restore later
    originalWalkSpeed = playerManager.walkSpeed;
    
    // Temporarily boost the walk speed with our 
    UnityEngine.Debug.Log("Dashing by " + dashSpeedMultiplier + "x");
    playerManager.walkSpeed *= dashSpeedMultiplier;
    
    
    // Optional: Add visual or audio effects
    PlayDashEffect();
}

private void EndDash()
{
    isDashing = false;
    
    // Restore the original walk speed
    playerManager.walkSpeed = originalWalkSpeed;
    
    // Optional: Clean up any dash effects
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

////////////////////////////////////////// Glow Action ///////////////////////////////////////////////
    private void runGlowLogic()
    {
        if (player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
        {
            UnityEngine.Debug.Log("Glow button pressed");
            StartCoroutine(GlowEffect());
        }
    }

    private IEnumerator GlowEffect()
    {
        foreach (Renderer objRender in dogObjects)
        {
            if (objRender != null)
            {
                objRender.material.EnableKeyword("_EMISSION");
                objRender.material.SetColor("_EmissionColor", dogGlowColor * glowIntensity);
            }
        }

        foreach (Renderer objRender in catObjects)
        {
            if (objRender != null)
            {
                objRender.material.EnableKeyword("_EMISSION");
                objRender.material.SetColor("_EmissionColor", catGlowColor * glowIntensity);
            }
        }

        yield return new WaitForSeconds(glowDuration);

        foreach (Renderer objRender in dogObjects)
        {
            if (objRender != null)
            {
                objRender.material.SetColor("_EmissionColor", Color.black);
            }
        }

        foreach (Renderer objRender in catObjects)
        {
            if (objRender != null)
            {
                objRender.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

////////////////////////////////////////// Actions UI ///////////////////////////////////////////////
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
        else if ((player1Input.GetControlsMenuPressed() || player2Input.GetControlsMenuPressed() || Input.GetKeyDown(KeyCode.I)) && isViewingControlsMenu)
        {
            controlsMenu.SetActive(false);
            isViewingControlsMenu = false;
        }
    }
}
