using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [Header("Text/Instruction Variables")]
    public GameObject overlay;
    public GameObject overlayBG;
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
    public GameObject controlsMenu;
    public GameObject controlsCornerUIParent;
    private GameObject controlsCornerUIChild;
    private GameObject P1ControlsCF;
    private GameObject P1ControlsDF;
    private GameObject P1ControlsCB;
    private GameObject P1ControlsDB;
    private GameObject P2ControlsCF;
    private GameObject P2ControlsDF;
    private GameObject P2ControlsCB;
    private GameObject P2ControlsDB;

    [Header("Icons and Sounds")]
    public GameObject P1IconLarge;
    public GameObject P2IconLarge;
    public GameObject playerIcons;
    public AudioClip whineSound;
    public AudioClip mewSound;

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
    public GameObject rugLight;
    public GameObject rugArrow;

    private GameObject task3;
    private Task3Tutorial enterRugAreaTrigger;
    
    // State tracking variables
    private int currTutorialStage = -1;
    private bool hasSplit = false;
    private bool hasReconnected = false;
    private bool hasSwitched = false;
    private bool canLeaveAttic = false;
    private FixedJoint fixedJoint;

    // Other variables 
    private GameObject emote;

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
        updateTutorialText();

        fixedJoint = playerManager.getJoint();
        vaseLights = vaseLightsParent.GetComponentsInChildren<Light>();
        boxesLights = boxesLightsParent.GetComponentsInChildren<Light>();

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

        UnityEngine.Debug.Log(controlsCornerUIChild);

        P1ControlsCF = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(0).gameObject;
        P1ControlsDF = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(1).gameObject;
        P1ControlsCB = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(2).gameObject;
        P1ControlsDB = controlsCornerUIChild.transform.GetChild(0).transform.GetChild(3).gameObject;
        P2ControlsCF = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(0).gameObject;
        P2ControlsDF = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(1).gameObject;
        P2ControlsCB = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(2).gameObject;
        P2ControlsDB = controlsCornerUIChild.transform.GetChild(1).transform.GetChild(3).gameObject;

        if (isKeyboard){
            overlay.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        } else if (!isKeyboard){
            overlay.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
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
            } else if (!isKeyboard && (player1Input.GetJumpPressed() || player2Input.GetJumpPressed()))
            {
                tutOverlayOrder++;
            }
        }

        // BLOCK SPLITTING UNTIL tutSplit
        if (getCurrTutorialStage() < tutSplit)
        {
            playerManager.setCanSplit(false);
        }
        else
        {
            playerManager.setCanSplit(true);
        }

        // BLOCK SWITCHING UNTIL tutSwitch
        if (getCurrTutorialStage() < tutSwitch)
        {
            playerManager.setCanSwitch(false);
        }
        else
        {
            playerManager.setCanSwitch(true);
        }

        // BLOCK RECONNECTING UNTIL tutReconnect
        if (getCurrTutorialStage() < tutReconnect)
        {
            playerManager.setCanReconnect(false);
        }
        else
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

        // check for keypress / gamepad press on "creepy..."
        if (getCurrTutorialStage() == 3 && checkForSpace())
        {
            advanceTutorialStage();
        }

        // check for keypress / gamepad press to close the tutorial
        if (getCurrTutorialStage() == tutComplete && checkForSpace())
        {
            leaveTutorial();
        }

        // check for return key press to switch before exit attic
        if (getCurrTutorialStage() == scaredDog && checkForSpace())
        {
            advanceTutorialStage();
        }

        // check for last switch to exit attic
        if (getCurrTutorialStage() == annoyedCat && playerManager.P1.IsFront && !playerManager.P2.IsFront)
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
    public const int annoyedCat = 10;
    public const int leaveAttic = 11;

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
    private void updateTutorialText()
    {
        switch (currTutorialStage)
        {
            case tutMoveToVase:
                // Speech UI
                P1IconLarge.SetActive(true);
                P2IconLarge.SetActive(true);
                speechBubbleTwoTails.SetActive(true);
                tutorialText.transform.SetParent(speechBubbleTwoTails.transform, true);
                tutorialText.text = "Let's move to the vase.";
                movementUI.SetActive(true);

                // Arrow (already active)

                // Paw path (already active)

                // Light path (already active)
                break;
            case tutBreakVase:
                // Deactivate stuff from prev case 
                vaseArrow.SetActive(false);
                vasePawPath.SetActive(false);
                vaseParticle.SetActive(false);
                movementUI.SetActive(false);
                // Turn on attic light
                brightenWorldLights();

                // Dim the previous light path
                foreach (Light light in vaseLights)
                {
                    StartCoroutine(lerpLightIntensity(light, 0.0f, 2.0f));
                }
                StartCoroutine(DelaySetActive(vaseLightsParent, false, 2.5f));

                // Speech UI
                tutorialText.text = "Hmm... can we break the vase?";
                // jumpUI.SetActive(true);

                // Turn on controls UI
                controlsCornerUIParent.SetActive(true);
                break;

            case tutSplit:
                // jumpUI.SetActive(false);
                enterVaseAreaTrigger.enabled = false;

                // Move the controls UI to the corner
                StartCoroutine(MoveControlsCornerUI());

                tutorialText.text = "Chaos! Now, let's split apart.";
                splitUI.SetActive(true);

                // gif

                break;
            // case 3:
            //     splitUI.SetActive(false);
            //     speechBubbleTwoTails.SetActive(false);

            //     speechBubbleLeft.SetActive(true);
            //     tutorialText.transform.SetParent(speechBubbleLeft.transform, true);
            //     tutorialText.text = "Creepy...";
            //     pressEnterToContinueUI.SetActive(true);

            //     GameObject backHalf = playerManager.getBackHalf();
            //     // emote = playerManager.startEmote(backHalf, "sad");
            //     if (whineSound != null)
            //     {
            //         AudioSource.PlayClipAtPoint(whineSound, backHalf.transform.position);
            //     }
            //     break;
            // case 4:
            //     UnityEngine.Debug.Log("case 4");
            //     playerManager.cancelEmote(emote);
            //     tutorialText.text = "sorry, i take that back.";
                
            //     emote = playerManager.startEmote(playerManager.getBackHalf(), "happy");
            //     // play happy dog sound
            //     break;              
            case tutScatterBoxes:
                // Cancel prev case
                splitUI.SetActive(false);
                speechBubbleTwoTails.SetActive(false);
                // playerManager.cancelEmote(emote);
                // pressEnterToContinueUI.SetActive(false);
                // speechBubbleLeft.SetActive(false);

                // Speech UI
                speechBubbleRight.SetActive(true);
                tutorialText.transform.SetParent(speechBubbleRight.transform, true);
                tutorialText.text = "Push the boxes while split apart."; 

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
                
                tutorialText.text = "Let's reconnect."; // speech bubble text
                reconnectUI.SetActive(true); // small text

                break;
            case tutMoveToRug:
                reconnectUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleLeft.SetActive(true);
                tutorialText.transform.SetParent(speechBubbleLeft.transform, true);
                tutorialText.text = "Hey, what's under that purple rug?"; // speech bubble text

                // Light
                dimLights();
                rugLight.SetActive(true);
                StartCoroutine(lerpLightIntensity(rugLight.GetComponent<Light>(), 10.0f, 1.5f));

                // Arrow
                rugArrow.SetActive(true);

                break;
            case tutSwitch:
                enterRugAreaTrigger.enabled = false;
                StartCoroutine(lerpLightIntensity(rugLight.GetComponent<Light>(), 0.0f, 2.0f));
                StartCoroutine(DelaySetActive(rugLight, false, 2.5f));
                rugArrow.SetActive(false);

                // light 
                brightenWorldLights();
                
                tutorialText.text = "I can't grab this, can you help?";
                switchUI.SetActive(true);
                
                break;
            case tutDragRug:
                switchUI.SetActive(false);
                speechBubbleLeft.SetActive(false);

                speechBubbleRight.SetActive(true);

                tutorialText.transform.SetParent(speechBubbleRight.transform, true);

                tutorialText.text = "Woah, I'm at the front now!";
                grabUI.SetActive(true);

                break;
            case tutComplete:
                grabUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleTwoTails.SetActive(true);

                tutorialText.transform.SetParent(speechBubbleTwoTails.transform, true);

                tutorialText.text = "Let's wreck this house!";
                tutorialSmallText.text = "Leave the attic, or take a look around";
                messageManager.startPressEnterToHideTutorial();

                // next case activated in AtticPrevention.cs
                break;
            
            case scaredDog:
                messageManager.cancelPressEnterToHideTutorial();
                speechBubbleLeft.SetActive(false);
                speechBubbleTwoTails.SetActive(false);
                switchUI.SetActive(false);

                
                tutorialSmallText.text = "";

                speechBubbleRight.SetActive(true);
                tutorialText.transform.SetParent(speechBubbleRight.transform, true);
                tutorialText.text = "The drop's too high... I'm scared!";
                pressEnterToContinueUI.SetActive(true);

                if (whineSound != null)
                {
                    AudioSource.PlayClipAtPoint(whineSound, playerManager.getFrontHalf().transform.position);
                }

                break;
            case annoyedCat:
                pressEnterToContinueUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleLeft.SetActive(true);
                switchUI.SetActive(true);
                tutorialText.transform.SetParent(speechBubbleLeft.transform, true);
                tutorialText.text = "Fine, I'll jump. Switch with me.";

                if (mewSound != null)
                {
                    AudioSource.PlayClipAtPoint(mewSound, playerManager.getBackHalf().transform.position);
                }
                break;

            case leaveAttic:
                speechBubbleLeft.SetActive(false);
                switchUI.SetActive(false);
                tutorialText.text = "";
                break;
        }
    }

    public void advanceTutorialStage()
    {
        // Debug.Log("advanced to next tutorial stage");
        currTutorialStage++;
        updateTutorialText();
    }

    public void leaveAtticSpeech()
    {
        currTutorialStage = scaredDog;
        updateTutorialText();
    }

    public int getCurrTutorialStage()
    {
        return currTutorialStage;
    }

    public bool checkForSpace()
    {
        return Input.GetKeyDown(KeyCode.Space) || 
               player1Input.GetSwitchPressed() || player1Input.GetReconnectPressed() ||
               player2Input.GetSwitchPressed() || player2Input.GetReconnectPressed();
    }

    // Coroutines
    private IEnumerator waitForSeconds(float seconds)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < seconds) 
        {
            yield return null;
        }

        stopwatch.Reset();
        advanceTutorialStage();
    }

    private IEnumerator DelaySetActive(GameObject obj, bool isActive, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(isActive);
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

    public void updateControlsCornerUI()
    {
        bool isP1Cat = playerManager.P1.Species == "cat";
        bool isP1Front = playerManager.P1.IsFront;

        P1ControlsCF.SetActive(isP1Cat && isP1Front);
        P1ControlsDF.SetActive(!isP1Cat && isP1Front);
        P1ControlsCB.SetActive(isP1Cat && !isP1Front);
        P1ControlsDB.SetActive(!isP1Cat && !isP1Front);

        P2ControlsCF.SetActive(!isP1Cat && !isP1Front);
        P2ControlsDF.SetActive(isP1Cat && !isP1Front);
        P2ControlsCB.SetActive(!isP1Cat && isP1Front);
        P2ControlsDB.SetActive(isP1Cat && isP1Front);
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
}
