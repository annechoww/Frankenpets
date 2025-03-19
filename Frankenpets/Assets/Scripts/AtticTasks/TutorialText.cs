using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [Header("Tutorial Overlay Variables")]
    public GameObject overlay;
    public GameObject overlayBG;

    [Header("Bottom Text/Instruction Variables")]
    public RectTransform bottomUIParent;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialSmallText;
    public GameObject speechBubbleTwoTails;
    public GameObject speechBubbleLeft;
    public GameObject speechBubbleRight;
    public GameObject movementUI;
    public GameObject jumpUI;
    public GameObject splitUI;
    public GameObject reconnectUI;
    public GameObject switchUI;
    public GameObject grabUI;
    public GameObject pressEnterToContinueUI;

    [Header("Black Overlays")]
    public GameObject bottomUIParentHighlight;
    public GameObject todoListHighlight;


    [Header("Controls Menu Pop-up")]
    public GameObject controlsMenu;

    [Header("Controls Corner Variables")]
    public GameObject controlsCornerUIParent;
    private GameObject controlsCornerUIChild;
    public GameObject P2ControlsGrab;
    private GameObject P1ControlsCF;
    // private GameObject P1ControlsDF; 
    private GameObject P1ControlsCB;
    // private GameObject P1ControlsDB; 
    // private GameObject P2ControlsCF; 
    private GameObject P2ControlsDF;
    // private GameObject P2ControlsCB; 
    private GameObject P2ControlsDB;

    [Header("Icons and Sounds")]
    public GameObject P1IconLarge;
    public GameObject P2IconLarge;
    // public GameObject playerIcons;
    public AudioClip whineSound;
    public AudioClip mewSound;
    public AudioClip barkSound;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    public Transform frontHalf;

    [Header("Attic Lighting")]
    public Light directionalLight;
    public Light pointLight;
    public float minDirLightIntensity = 0.0f;
    public float maxDirLightIntensity = 2.0f;
    public float minPointLightIntensity = 0.5f;
    public float maxPointLightIntensity = 1.0f;

    [Header("Vase Task")]
    public GameObject vaseArrow;
    public GameObject vasePawPath;
    public GameObject vaseLightsParent;
    public GameObject vaseParticle;
    private Light[] vaseLights;
    private Task2Tutorial enterVaseAreaTrigger;


    [Header("Box Task")]
    public GameObject frontBoxArrow;
    public GameObject backBoxArrow;
    public GameObject boxesPawPath;
    public GameObject boxParticle;
    public GameObject boxesLightsParent;
    private Light[] boxesLights;
    
    [Header("Rug Task")]    
    public GameObject rugMainLight;
    public GameObject rugLightsParent;
    private Light[] rugLights;
    public GameObject rugArrow;
    public GameObject rugPawPath;
    private GameObject task3;
    private Task3Tutorial enterRugAreaTrigger;

    [Header("Todo List")]
    public GameObject vaseTaskTodo;
    public GameObject boxTaskTodo;
    public GameObject rugTaskTodo;
    public RectTransform todoList;

    
    // State tracking variables
    private int currTutorialStage = -1;
    private bool hasSplit = false;
    private bool hasReconnected = false;
    private bool hasSwitched = false;
    private bool canLeaveAttic = false;
    private FixedJoint fixedJoint;

    // Other script references    
    private PlayerManager playerManager;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment; 
    private bool isKeyboard;   


    void Awake()
    {
        enterVaseAreaTrigger = GameObject.Find("Task 2").GetComponent<Task2Tutorial>();
        enterRugAreaTrigger = GameObject.Find("Task 3").GetComponent<Task3Tutorial>();

        GameObject pet = GameObject.Find("Pet");
        playerManager = pet.GetComponent<PlayerManager>();
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = pet.GetComponent<ControllerAssignment>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(updateTutorialText());

        fixedJoint = playerManager.getJoint();
        vaseLights = vaseLightsParent.GetComponentsInChildren<Light>();
        boxesLights = boxesLightsParent.GetComponentsInChildren<Light>();
        rugLights = rugLightsParent.GetComponentsInChildren<Light>();

        // Set the instructions UI according to keycaps or gamepad
        isKeyboard = controllerAssignment.IsKeyboard();
        
        movementUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        jumpUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        splitUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        reconnectUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        switchUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        grabUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);

        movementUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        jumpUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        splitUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        reconnectUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        switchUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        grabUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);

        controlsCornerUIParent.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        controlsCornerUIParent.transform.GetChild(1).gameObject.SetActive(!isKeyboard);

        if (isKeyboard) controlsCornerUIChild = controlsCornerUIParent.transform.GetChild(0).gameObject; // update variable to be either its keyboard or gamepad child
        else controlsCornerUIChild = controlsCornerUIParent.transform.GetChild(1).gameObject;

        P1ControlsCF = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(0).gameObject;
        // P1ControlsDF = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(1).gameObject;
        P1ControlsCB = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(1).gameObject; // old index was 2
        // P1ControlsDB = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(3).gameObject;
        // P2ControlsCF = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(0).gameObject;
        P2ControlsDF = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(0).gameObject; // old index was 1
        // P2ControlsCB = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(2).gameObject;
        P2ControlsDB = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(1).gameObject; // old index was 3

        P2ControlsGrab.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        P2ControlsGrab.transform.GetChild(1).gameObject.SetActive(!isKeyboard);

        if (isKeyboard){
            overlay.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        } else if (!isKeyboard){
            overlay.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }

        playerManager.setCanSwitch(false);
        playerManager.setCanReconnect(false);
        playerManager.setCanSplit(false);
    }

    void Update()
    {
        updateControlsCornerUI();

        if (getCurrTutorialStage() == tutStartOverlay)
        {
            handleOverlay();
            if (isKeyboard && Input.GetKeyDown(KeyCode.Space))
            {
                tutOverlayOrder++;
            } else if (!isKeyboard && (player1Input.GetGlowPressed() || player2Input.GetGlowPressed()))
            {
                tutOverlayOrder++;
            }
        }

        // BLOCK SPLITTING UNTIL tutSplit
        if (getCurrTutorialStage() == tutSplit)
        {
            playerManager.setCanSplit(true);
        }

        // BLOCK SWITCHING UNTIL tutSwitch
        if (getCurrTutorialStage() >= tutSwitch)
        {
            playerManager.setCanSwitch(true);
        }

        // BLOCK RECONNECTING UNTIL tutReconnect
        if (getCurrTutorialStage() >= tutReconnect)
        {
            playerManager.setCanReconnect(true);
        }

        // check for first split
        if (getCurrTutorialStage() == tutSplit && !hasSplit)
        {
            fixedJoint = playerManager.getJoint();

            if (fixedJoint == null)
            {
                hasSplit = true;
                advanceTutorialStage();
            }
        }

        // check for first reconnection
        if (getCurrTutorialStage() == tutReconnect && !hasReconnected)
        {
            fixedJoint = playerManager.getJoint();

            if (fixedJoint)
            {
                hasReconnected = true;
                advanceTutorialStage();
            }
        }

        // check for first switch
        if (getCurrTutorialStage() == tutSwitch && !hasSwitched)
        {
            if (!playerManager.P1.IsFront && playerManager.P2.IsFront)
            {
                hasSwitched = true;
                advanceTutorialStage();
            }
        }

        // check for keypress / gamepad press to close the tutorial
        // if (getCurrTutorialStage() == tutComplete && checkForSpace())
        // {
        //     // leaveTutorial();
        //     StartCoroutine(HideEffect(pressEnterToContinueUI, speechBubbleTwoTails));
        // }

        // check for return key press to switch before exit attic
        // if (getCurrTutorialStage() == scaredDog && checkForSpace())
        // {
        //     advanceTutorialStage();
            
        // }

        // check for last switch to exit attic
        if (getCurrTutorialStage() == scaredDog && playerManager.P1.IsFront && !playerManager.P2.IsFront)
        {
            advanceTutorialStage();
        }
    }

    public const int tutStartOverlay = -1;
    public const int tutMoveToVase = 0;
    public const int tutBreakVase = 1;
    public const int tutSplit = 2;
    public const int tutScatterBoxes = 3;
    public const int tutReconnect = 4;
    public const int tutMoveToRug = 5;
    public const int tutSwitch = 6;
    public const int tutDragRug = 7;
    public const int tutComplete = 8;
    public const int scaredDog = 9;
    // public const int annoyedCat = 10;
    public const int leaveAttic = 10;

    // tutorial overlay variables
    private int tutOverlayOrder = 1;

    public bool overlayDone()
    {
        return currTutorialStage!=-1;
    }

    private void handleOverlay()
    {
        switch(tutOverlayOrder)
        {
            
            case 2:
                overlay.transform.GetChild(1).gameObject.SetActive(false);
                overlay.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 3:
                overlay.transform.GetChild(2).gameObject.SetActive(false);
                overlay.transform.GetChild(3).gameObject.SetActive(true);
                break;
            case 4:
                overlay.transform.GetChild(3).gameObject.SetActive(false);
                overlay.transform.GetChild(4).gameObject.SetActive(true);

                Transform menuUI = overlay.transform.GetChild(4).gameObject.transform.Find("MenuUI");
                if (isKeyboard)
                {
                    menuUI.transform.GetChild(0).gameObject.SetActive(true);
                } else{
                    menuUI.transform.GetChild(1).gameObject.SetActive(true);
                }
                break;
            case 5:
                overlay.transform.GetChild(4).gameObject.SetActive(false);
                overlayBG.SetActive(false);
                overlay.SetActive(false);
                P1IconLarge.SetActive(true);
                P2IconLarge.SetActive(true);
                advanceTutorialStage();
                break;
        }
    }


    // Update is called once per frame
    private IEnumerator updateTutorialText()
    {
        switch (currTutorialStage)
        {
            case tutMoveToVase:
                // Bottom UI
                P1IconLarge.SetActive(true);
                P2IconLarge.SetActive(true);
                StartCoroutine(Highlight(bottomUIParentHighlight));
                yield return StartCoroutine(ShowBottomUI(movementUI, speechBubbleTwoTails, "Move to the vase.", ""));
                
                // Arrow (already active)

                // Paw path (already active)

                // Light path (already active)
                break;
            case tutBreakVase:
                // Deactivate prev case
                yield return StartCoroutine(HideEffect(movementUI, speechBubbleTwoTails));

                // Tell player to break the vase
                StartCoroutine(Highlight(todoListHighlight));
                revealTaskTodo(vaseTaskTodo);
                // Turn on controls UI (reveals jump control since it's already set as active)
                controlsCornerUIParent.SetActive(true);

                yield return new WaitForSeconds(2.0f);
                yield return StartCoroutine(Bounce(controlsCornerUIChild.transform));

                break;

            case tutSplit:
                // Completed vase task
                completeTaskTodo(vaseTaskTodo);
                enterVaseAreaTrigger.enabled = false;

                // Move the controls UI to the corner (OLD)
                // StartCoroutine(MoveControlsCornerUI());
                

                // Bottom UI
                yield return StartCoroutine(ShowBottomUI(splitUI, speechBubbleTwoTails, "Chaos! Let's <u>split apart</u>.", ""));

                // Deactivate prev paw path
                vaseArrow.SetActive(false);
                vasePawPath.SetActive(false);
                vaseParticle.SetActive(false);

                // Dim the previous light path
                foreach (Light light in vaseLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.0f, 2.0f));
                }
                StartCoroutine(DelaySetActive(vaseLightsParent, false, 2.5f));

                // Turn on attic light
                brightenWorldLights();
            
                // gif ???

                break;
           
            case tutScatterBoxes:
                // Deactivate prev case
                yield return StartCoroutine(HideEffect(splitUI, speechBubbleTwoTails));

                // Tell player to push the boxes
                StartCoroutine(Highlight(todoListHighlight));
                revealTaskTodo(boxTaskTodo);

                // Arrow 
                backBoxArrow.SetActive(true);
                frontBoxArrow.SetActive(true);

                // Particles 
                boxParticle.SetActive(true);
                
                // Paw path
                boxesPawPath.SetActive(true); 

                // Dim attic lights
                dimLights();

                // Light path
                boxesLightsParent.SetActive(true);
                foreach (Light light in boxesLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.1f, 1.5f));
                }

                break;

            case tutReconnect:
                // Box task is completed
                completeTaskTodo(boxTaskTodo);

                // Bottom UI
                yield return StartCoroutine(ShowBottomUI(reconnectUI, speechBubbleTwoTails, "Let's <u>reconnect</u>.", ""));

                // Disable guiding path 
                backBoxArrow.SetActive(false);
                frontBoxArrow.SetActive(false);
                boxesPawPath.SetActive(false);
                boxParticle.SetActive(false);
                
                // Dim the previous light path 
                foreach (Light light in boxesLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.0f, 2.0f));
                }
                StartCoroutine(DelaySetActive(boxesLightsParent, false, 2.5f));

                // Brighten the attic lights
                brightenWorldLights();

                break;

            case tutMoveToRug:
                // Hide previous message
                yield return StartCoroutine(HideEffect(reconnectUI, speechBubbleTwoTails));
                
                // Reveal reconnect control in controls corner
                yield return StartCoroutine(RevealControlsCornerUI(1));
                                   
                // Light
                dimLights();
                rugMainLight.SetActive(true);
                StartCoroutine(lerpLightIntensity(rugMainLight.GetComponent<Light>(), 10.0f, 1.5f));
                rugLightsParent.SetActive(true);
                foreach (Light light in rugLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.1f, 1.5f));
                }
                
                // Arrow
                rugArrow.SetActive(true);

                // Paw path
                rugPawPath.SetActive(true);

                // Show new message
                yield return StartCoroutine(ShowBottomUI(null, speechBubbleLeft, "What's under that <u>purple rug</u>?", ""));

                break;
            case tutSwitch:
                enterRugAreaTrigger.enabled = false;

                // Hide previous message
                yield return StartCoroutine(HideEffect(null, null));
                
                // Disable previous guiding path + light 
                foreach (Light light in rugLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.0f, 1.5f));
                }
                StartCoroutine(DelaySetActive(rugLightsParent, false, 2.5f));

                StartCoroutine(lerpLightIntensity(rugMainLight.GetComponent<Light>(), 0.0f, 2.0f));
                StartCoroutine(DelaySetActive(rugMainLight, false, 2.5f));
                
                rugArrow.SetActive(false);
                rugPawPath.SetActive(false);

                // Add back world light  
                brightenWorldLights();

                // Show new message
                yield return StartCoroutine(ShowBottomUI(switchUI, speechBubbleLeft, "I can't grab this. Let's <u>switch</u>.", ""));
                
                break;
            case tutDragRug:
                // Hide previous message
                yield return StartCoroutine(HideEffect(switchUI, speechBubbleLeft));
                
                // Reveal reconnect control in controls corner
                yield return StartCoroutine(RevealControlsCornerUI(0));

                // StartCoroutine(Highlight(todoListHighlight));
                revealTaskTodo(rugTaskTodo);

                // Show new msg
                // tutorialText.transform.SetParent(speechBubbleRight.transform, true);
                yield return StartCoroutine(ShowBottomUI(grabUI, speechBubbleRight, "Woah, I'm at the front now!", ""));

                break;
            case tutComplete:
                // Hide previous message
                yield return StartCoroutine(HideEffect(grabUI, speechBubbleRight));
                completeTaskTodo(rugTaskTodo);
               
                // Reveal "grab" control for dog
                P2ControlsGrab.SetActive(true);


                // messageManager.startPressEnterToHideTutorial();

                // Show new message
                // tutorialText.transform.SetParent(speechBubbleTwoTails.transform, true);
                yield return StartCoroutine(ShowBottomUI(null, speechBubbleTwoTails, "Let's wreck this house!", "Leave the attic, or look around"));

                // Reveal remaining basic controls
                yield return new WaitForSeconds(1.0f);
                yield return StartCoroutine(RevealControlsCornerUI(3));
                // yield return StartCoroutine(Bounce(controlsCornerUIChild.transform));

                // next case activated in AtticPrevention.cs
                break;
            
            case scaredDog:
                // Hide things from previous case and annoyedCat case 
                // messageManager.cancelPressEnterToHideTutorial();
                // speechBubbleLeft.SetActive(false);
                speechBubbleTwoTails.SetActive(false);
                // switchUI.SetActive(false);

                // Show scaredDog dialogue
                // tutorialText.transform.SetParent(speechBubbleRight.transform, true);
                yield return StartCoroutine(HideEffect(switchUI, null));
                yield return StartCoroutine(ShowBottomUI(switchUI, speechBubbleRight, "The drop's too high for me... Let's <u>switch</u>!", "", false));
        
                // pressEnterToContinueUI.SetActive(true);

                if (whineSound != null)
                {
                    AudioManager.Instance.Play3DSFX(whineSound, playerManager.getFrontHalf().transform.position);
                }

                break;

            // case annoyedCat:
        
            //     // tutorialText.transform.SetParent(speechBubbleLeft.transform, true);


            //     yield return StartCoroutine(HideEffect(pressEnterToContinueUI, speechBubbleRight));
            //     yield return StartCoroutine(ShowBottomUI(switchUI, speechBubbleLeft, "Fine, I'll jump. Switch with me.", "", false));
          

            //     if (mewSound != null)
            //     {
            //         AudioManager.Instance.Play3DSFX(mewSound, playerManager.getBackHalf().transform.position);
            //     }
            //     break;

            case leaveAttic:
                yield return StartCoroutine(HideEffect(switchUI, speechBubbleLeft));
                break;
        }
    }

    // TUTORIAL-SPECIFIC METHODS //////////////////////////////////////
    public void advanceTutorialStage()
    {
        currTutorialStage++;
        UnityEngine.Debug.Log("Tutorial stage: " + currTutorialStage);
        StartCoroutine(updateTutorialText());
    }

    public void leaveAtticSpeech()
    {
        currTutorialStage = scaredDog;
        StartCoroutine(updateTutorialText());
    }

    public void hideLeaveAtticSpeech()
    {
        currTutorialStage = leaveAttic;
        StartCoroutine(updateTutorialText());
    }

    private void leaveTutorial()
    {
        // hide speech bubble stuff 
        speechBubbleTwoTails.SetActive(false);
        tutorialText.text = "";
        tutorialSmallText.text = "";
        messageManager.cancelPressEnterToHideTutorial();
        P2IconLarge.SetActive(false);
        P1IconLarge.SetActive(false);

        // show the player icons
        // playerIcons.SetActive(true); 
    }

    public int getCurrTutorialStage()
    {
        return currTutorialStage;
    }
    // TUTORIAL-SPECIFIC METHODS //////////////////////////////////////


    // IDK WHAT TO CALL THIS //////////////////////////////////////////
    public bool checkForSpace()
    {
        return Input.GetKeyDown(KeyCode.Space) || 
               player1Input.GetGlowPressed() || player2Input.GetGlowPressed();
    }

    private IEnumerator DelaySetActive(GameObject obj, bool isActive, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(isActive);
    }
    // IDK WHAT TO CALL THIS //////////////////////////////////////////


    // LIGHTING ///////////////////////////////////////////////////////
    private void dimLights()
    {
        // directional light
        StartCoroutine(lerpLightIntensity(directionalLight, minDirLightIntensity, 2.0f));

        // point light 
        StartCoroutine(lerpLightIntensity(pointLight, minPointLightIntensity, 2.0f));
    }

    private void brightenWorldLights()
    {
        // directional light
        StartCoroutine(lerpLightIntensity(directionalLight, maxDirLightIntensity, 2.0f));

        // point light 
        StartCoroutine(lerpLightIntensity(pointLight, maxPointLightIntensity, 2.0f));
    }

    private IEnumerator lerpLightIntensity(Light light, float targetIntensity, float duration)
    {
        float startIntensity = light.intensity;
        float time = 0.0f;

        while (time < duration)
        {
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        light.intensity = targetIntensity;
    }
    // LIGHTING ///////////////////////////////////////////////////////


    // CONTROLS CORNER UI /////////////////////////////////////////////
    public void updateControlsCornerUI()
    {
        bool isP1Cat = playerManager.P1.Species == "cat";
        bool isP1Front = playerManager.P1.IsFront;

        P1ControlsCF.SetActive(isP1Cat && isP1Front);
        // P1ControlsDF.SetActive(!isP1Cat && isP1Front);
        P1ControlsCB.SetActive(isP1Cat && !isP1Front);
        // P1ControlsDB.SetActive(!isP1Cat && !isP1Front);

        // P2ControlsCF.SetActive(!isP1Cat && !isP1Front);
        P2ControlsDF.SetActive(isP1Cat && !isP1Front);
        // P2ControlsCB.SetActive(!isP1Cat && isP1Front);
        P2ControlsDB.SetActive(isP1Cat && isP1Front);

        if (getCurrTutorialStage() >= tutComplete) P2ControlsGrab.SetActive(isP1Cat && !isP1Front);
    }

    private IEnumerator RevealControlsCornerUI(int idx)
    {
        P1ControlsCF.transform.GetChild(idx).gameObject.SetActive(true);
        P1ControlsCB.transform.GetChild(idx).gameObject.SetActive(true);
       
        P2ControlsDF.transform.GetChild(idx).gameObject.SetActive(true);
        P2ControlsDB.transform.GetChild(idx).gameObject.SetActive(true);

        yield return null;
    }

    private IEnumerator Bounce(Transform objectTransform, float bounceHeight = 20.0f, float bounceSpeed = 8.0f, float duration = 3.0f)
    {
        float elapsedTime = 0f;
        Vector3 startPos = objectTransform.position;

        while (elapsedTime < duration)
        {
            float bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            objectTransform.position = startPos + new Vector3(0, bounceOffset, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset position after bouncing
        objectTransform.position = startPos;
    }

    private IEnumerator MoveControlsCornerUI()
    {
        float moveSpeed = 3.0f;

        RectTransform P1RectTransform = controlsCornerUIChild.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        RectTransform P2RectTransform = controlsCornerUIChild.transform.GetChild(1).gameObject.GetComponent<RectTransform>();

        Vector2 P1targetPosition1 = new Vector2(-1260, 36);
        Vector2 P2targetPosition1 = new Vector2(231, 36);

        while (Vector2.Distance(P1RectTransform.anchoredPosition, P1targetPosition1) > 1f)
        {
            P1RectTransform.anchoredPosition = Vector2.Lerp(P1RectTransform.anchoredPosition, P1targetPosition1, Time.deltaTime * moveSpeed);
            P2RectTransform.anchoredPosition = Vector2.Lerp(P2RectTransform.anchoredPosition, P2targetPosition1, Time.deltaTime * moveSpeed);
            yield return null;
        }
        
        P1RectTransform.anchoredPosition = P1targetPosition1; // Ensure final position is exact
        P2RectTransform.anchoredPosition = P2targetPosition1;
        
        Vector2 P1targetPosition2 = new Vector2(-1260, -252);
        Vector2 P2targetPosition2 = new Vector2(231, -252);

        while (Vector2.Distance(P1RectTransform.anchoredPosition, P1targetPosition2) > 1f)
        {
            P1RectTransform.anchoredPosition = Vector2.Lerp(P1RectTransform.anchoredPosition, P1targetPosition2, Time.deltaTime * moveSpeed);
            P2RectTransform.anchoredPosition = Vector2.Lerp(P2RectTransform.anchoredPosition, P2targetPosition2, Time.deltaTime * moveSpeed);
            yield return null;
        }
        
        P1RectTransform.anchoredPosition = P1targetPosition2; // Ensure final position is exact
        P2RectTransform.anchoredPosition = P2targetPosition2;
    }
    // CONTROLS CORNER UI /////////////////////////////////////////////

    // BOTTOM SPEECH UI ///////////////////////////////////////////////
    private IEnumerator SlideUpEffect(RectTransform rectTransform)
    {
        while (isCoroutineRunning)
            yield return null;

        isCoroutineRunning = true;
        
        float moveSpeed = 5.0f;
        // Vector2 targetPosition = new Vector2(-10, -710);
        Vector2 targetPosition = new Vector2(-10, -465);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    private IEnumerator SlideDownEffect(RectTransform rectTransform)
    {
        while (isCoroutineRunning)
            yield return null;

        isCoroutineRunning = true;

        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -710);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    private bool isCoroutineRunning = false;
    private IEnumerator HideEffect(GameObject uiComponent = null, GameObject bubble = null)
    {
        while (isCoroutineRunning)
            yield return null;

        isCoroutineRunning = true;

        RectTransform rectTransform = bottomUIParent;

        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -1086);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            
        }

        tutorialSmallText.text = "";
        tutorialText.text = "";

        if (uiComponent != null) uiComponent.SetActive(false);
        if (bubble != null) bubble.SetActive(false);
        
        isCoroutineRunning = false;
        yield return null;
        
    }

    // private Coroutine bounceCoroutine;
    private IEnumerator ShowBottomUI(GameObject uiComponent = null, GameObject bubble = null, string largeText = "", string smallText = "", bool playSound = true)
    {
        if (uiComponent != null) uiComponent.SetActive(true);
        if (bubble != null) bubble.SetActive(true);
        

        if (playSound)
        {
            if (bubble == speechBubbleLeft) AudioManager.Instance.PlaySFX(mewSound);
            else if (bubble == speechBubbleRight) AudioManager.Instance.PlaySFX(barkSound);
            else
            {
                AudioManager.Instance.PlaySFX(mewSound);
                yield return new WaitForSeconds(0.5f);
                AudioManager.Instance.PlaySFX(barkSound);
            }
        }

        bottomUIParent.anchoredPosition = new Vector2(-10, -1086);

        tutorialSmallText.text = smallText;
        tutorialText.text = largeText;

        yield return StartCoroutine(SlideUpEffect(bottomUIParent));
        yield return StartCoroutine(SlideDownEffect(bottomUIParent));

        // Stop any ongoing bounce before starting a new one
        // if (bounceCoroutine != null)
        // {
        //     StopCoroutine(bounceCoroutine);
        //     bounceCoroutine = null;
        // }

        // bounceCoroutine = StartCoroutine(BounceAfterDelay(bottomUIParent, 0.7f));
    }

    // Delay a coroutine 
    private IEnumerator DelayedCall(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }


    // Bounce effect (not currently used; looks a bit off)
    // private float initialBounceHeight = 200.0f;
    // private float gravity = 500f;
    // private int bounceCount = 2;
    // private float heightDamping = 0.8f; // Higher = bounces higher 
    // private float speedDamping = 0.6f; // Higher = slower
    // private IEnumerator BallBounceEffect(RectTransform obj)
    // {
    //     Vector2 startPosition = new Vector2(-10, -710);
    //     float height = initialBounceHeight;
    //     float velocity = Mathf.Sqrt(2 * gravity * height); // Convert height to velocity

    //     // First fall (natural drop)    
    //     yield return MoveYWithGravity(obj, startPosition.y, velocity);

    //     for (int i = 0; i < bounceCount; i++)
    //     {
    //         velocity *= Mathf.Sqrt(heightDamping); // Reduce velocity smoothly
    //         height *= heightDamping; // Reduce bounce height

    //         // Move up and down naturally
    //         yield return MoveYWithGravity(obj, startPosition.y + height, velocity);
    //         velocity *= speedDamping; // Speed up next bounce
    //         yield return MoveYWithGravity(obj, startPosition.y, velocity);

    //         velocity *= speedDamping; // Slow down next bounce
    //     }
    // }

    // Moves the object smoothly using physics-based motion
    // private IEnumerator MoveYWithGravity(RectTransform rectTransform, float targetY, float velocity)
    // {
    //     float startY = rectTransform.anchoredPosition.y;
    //     float timeToPeak = velocity / gravity; // Time to reach the top

    //     float elapsedTime = 0f;
    //     while (elapsedTime < timeToPeak)
    //     {
    //         float t = elapsedTime / timeToPeak;
    //         float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth ease-out for up movement
    //         rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(startY, targetY, easedT));

    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
    // }

    // private IEnumerator BounceAfterDelay(RectTransform rectTransform, float delay)
    // {
    //     yield return new WaitForSeconds(delay); 

    //     Vector3 initialPosition = rectTransform.anchoredPosition;

    //     while (true)
    //     {
    //         float newY = initialPosition.y + Mathf.Sin(Time.time * 4.0f) * 20.0f; // speed, amplitude
    //         rectTransform.anchoredPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    //         yield return null; // Wait for next frame
    //     }
    // }
    // BOTTOM SPEECH UI ///////////////////////////////////////////////


    // TO-DO LIST /////////////////////////////////////////////////////
    private IEnumerator Highlight(GameObject highlight)
    {
        highlight.SetActive(true);
        // highlight.color = new Color(0, 0, 0, 177);

        yield return StartCoroutine(FadeIn(highlight, 1.0f));

        yield return new WaitForSeconds(1.2f);

        yield return StartCoroutine(FadeOut(highlight, 1.0f));

        highlight.SetActive(false);
    }

    private IEnumerator FadeIn(GameObject highlight, float duration)
    {
        RectTransform rectTransform = highlight.GetComponent<RectTransform>();
        Image image = highlight.GetComponent<Image>();

        float elapsedTime = 0f;
        Color startColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.0f, 0.85f, elapsedTime / duration);
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null; // causing problem?
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, 0.85f);
    }

    private IEnumerator FadeOut(GameObject highlight, float duration)
    {
        RectTransform rectTransform = highlight.GetComponent<RectTransform>();
        Image image = highlight.GetComponent<Image>();

        float elapsedTime = 0f;
        Color startColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.85f, 0.0f, elapsedTime / duration);
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null; 
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, 0.0f); // Ensure it's fully invisible
    }

    private float pulseSpeed = 1.5f;
    private float scaleAmount = 0.2f;
    private Vector3 initialScale; // tbh should not be storing it as global var. should make a new instance for each rectTransform
    private Coroutine StartPulseCoroutine;
    private IEnumerator StartPulse(RectTransform rectTransform)
    {
        initialScale = rectTransform.localScale;
        float time = 0f;

        while (true) 
        {
            float scaleFactor = 1 + Mathf.Abs(Mathf.Sin(time * pulseSpeed)) * scaleAmount;
            rectTransform.localScale = initialScale * scaleFactor;

            time += Time.deltaTime;

            yield return null; 
        }
    }

    private IEnumerator StopPulse(RectTransform rectTransform)
    {
        if (StartPulseCoroutine != null) StopCoroutine(StartPulseCoroutine);

        rectTransform.localScale = initialScale;

        yield return null;
    }

    private void revealTaskTodo(GameObject taskTodo)
    {
        // Question mark --> task 
        taskTodo.transform.GetChild(0).gameObject.SetActive(false);
        taskTodo.transform.GetChild(1).gameObject.SetActive(true);

        // Pulsing effect
        // StartPulseCoroutine = StartCoroutine(StartPulse(taskTodo.GetComponent<RectTransform>()));
        StartPulseCoroutine = StartCoroutine(StartPulse(todoList));
                
    }

    private void completeTaskTodo(GameObject taskTodo)
    {
        // Stop pulsing effect
        // StartCoroutine(StopPulse(taskTodo.GetComponent<RectTransform>()));
        StartCoroutine(StopPulse(todoList));

        // Task --> strikethrough
        taskTodo.transform.GetChild(1).gameObject.SetActive(false);
        taskTodo.transform.GetChild(2).gameObject.SetActive(true);
        
    }
    // TO-DO LIST /////////////////////////////////////////////////////
}
