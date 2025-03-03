using System.Collections;
using System.Diagnostics;
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
    private ControllerAssignment controllerAssignment;

    [Header("Jumping Variables")]
    private float jumpForce = 15f;
    public float jumpCooldown = 0.5f;
    private float chargedJumpForce = 25f;
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
    // private GameObject climbText;

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

    [Header("Grab Joint Settings")]
    public float defaultSpringForce = 1000f;
    public float defaultDamperForce = 20f;
    private ConfigurableJoint grabJoint;
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
    private HindLegsRigging hindLegsRiggingScript;
    private PawRigging pawRiggingScript;

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

    private void Start()
    {   
        // climbText = GameObject.FindGameObjectWithTag("ClimbText");
        // grabText = GameObject.FindGameObjectWithTag("GrabText");
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
        Invoke("getPlayerManager", 0f);

        // Find all grabbable objects
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
    }

    private void Update()
    {
        runJumpLogic();
        runNoiseLogic();
        runClimbLogic();
        runGrablogic();
        runTailLogic();
        runPawLogic();
        runGlowLogic();
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
            climbText.transform.position = other.transform.position + (Vector3.forward * 0.05f);
            showClimbText();

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
            grabText.transform.position = grabbableObject.transform.position + (Vector3.up * 0.30f);
            showGrabText();
            
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

            mouthRiggingScript = frontHalf.GetComponentInChildren<MouthRigging>();
            tailRiggingScript = backHalf.GetComponentInChildren<TailRigging>();
            pawRiggingScript = frontHalf.GetComponentInChildren<PawRigging>();
            //hindLegsRiggingScript = backHalf.GetComponentInChildren<HindLegsRigging>();
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
            if (playerManager.getJoint() != null)
            {
                frontRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
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
    
    private void determineGrabType() 
    {
        if (grabbableObject == null)
        {
            UnityEngine.Debug.LogWarning("Attempted to determine grab type with no grabbable object");
            return;
        }
        
        // Calculate the dog's mouth position (for grab point calculation)
        mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);
        
        // Reset all references to ensure clean state
        currentGrabPoint = null;
        currentGrabbableObject = null;
        grabPoint = Vector3.zero;
        
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
            
            // Log for debugging
            UnityEngine.Debug.Log($"Grabbing {grabbableObject.name} as a grab point. Parent: {currentGrabPoint.parentRigidbody.name}");
            
            return;
        }
        
        // CASE 2: Check if it's a directly grabbable object with custom properties
        currentGrabbableObject = grabbableObject.GetComponent<GrabbableObject>();
        
        if (currentGrabbableObject != null)
        {
            // This is a grabbable object with custom properties
            
            // Use closest point on the collider as the grab point
            grabPoint = grabbableObject.ClosestPoint(mouthPosition);
            
            // If closest point is at the center (inside the collider), use a point on the surface
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

            releaseGrabbedObject();

        }
        else if (dogFrontSpecial && canGrab)
        {
            UnityEngine.Debug.Log("Grabbed item");
            
            grabObject();
        }
        else if (dogFrontSpecial){
            mouthRiggingScript.openMouth();
        }

        if (isGrabbing & grabJoint != null) {
            updateGrabJoint();
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

        mouthPosition = transform.position + transform.TransformDirection(Vector3.forward * 0.23f + Vector3.up * 0.202f);

        grabJoint = gameObject.AddComponent<ConfigurableJoint>();
        grabJoint.connectedBody = targetRigidbody;

        configureJoint(grabJoint);

        // Set the anchor point at the dog's mouth
        grabJoint.anchor = transform.InverseTransformPoint(mouthPosition);
        
        // Determine if this is a portable or draggable object
        bool isPortable = false;
        
        if (currentGrabPoint != null) {
            isPortable = currentGrabPoint.grabBehavior == GrabPoint.GrabBehavior.Portable;
        } else if (currentGrabbableObject != null) {
            isPortable = currentGrabbableObject.grabBehavior == GrabbableObject.GrabBehavior.Portable;
        }
        
        if (isPortable) {
            // For portable objects, make them move to the dog's mouth
            grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(targetRigidbody.transform.position);
            
            // Optional: adjust rotation to orient properly in mouth
            Quaternion targetRotation = Quaternion.LookRotation(-transform.forward, transform.up);
            targetRigidbody.transform.rotation = targetRotation;
        } else {
            // For draggable objects, keep the connection at the grab point
            grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(grabPoint);
        }

        originalPlayerSpeed = playerManager.walkSpeed;

        applyMovementPenalty();
        applyTurnRestriction();

        isGrabbing = true;
        grabText.SetActive(false);

        mouthRiggingScript.openMouth();
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
        if (grabJoint == null && isGrabbing) {
            releaseGrabbedObject();
            return;
        }
    }

    private void releaseGrabbedObject() {
        UnityEngine.Debug.Log("Released item");

        if (grabJoint != null) {
            Destroy(grabJoint);
            grabJoint = null;
        }

        playerManager.walkSpeed = originalPlayerSpeed;
        if (turnRestricted) {
            playerManager.turnSpeed = originalTurnSpeed;
            turnRestricted = false;
        }
        isGrabbing = false;
        currentGrabPoint = null;
        currentGrabbableObject = null;

        // mouthRiggingScript.closeMouth();
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

////////////////////////////////////////// Glow Action ///////////////////////////////////////////////
    private void runGlowLogic()
    {
        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.M))
        {
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
    private void showClimbText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            climbText.SetActive(true);

            if (P1.IsFront) climbText.transform.GetChild(0).gameObject.SetActive(true);
            else climbText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            climbText.SetActive(true);

            climbText.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void hideClimbText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            if (P1.IsFront) climbText.transform.GetChild(0).gameObject.SetActive(false);
            else climbText.transform.GetChild(1).gameObject.SetActive(false);

            climbText.SetActive(false);
        }
        else
        {
            climbText.transform.GetChild(2).gameObject.SetActive(false);

            climbText.SetActive(false);
        }
    }

    private void showGrabText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            grabText.SetActive(true);

            // if (P1.IsFront) grabText.transform.GetChild(0).gameObject.SetActive(true);
            // else grabText.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            grabText.SetActive(true);
            
            // grabText.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void hideGrabText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            // if (P1.IsFront) grabText.transform.GetChild(0).gameObject.SetActive(false);
            // else grabText.transform.GetChild(1).gameObject.SetActive(false);

            grabText.SetActive(false);
        }
        else
        {
            // grabText.transform.GetChild(2).gameObject.SetActive(false);

            grabText.SetActive(false);
        }
    }   

}