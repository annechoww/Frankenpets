using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [Header("Text/Instruction Variables")]
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

    [Header("Icons")]
    public GameObject P1IconLarge;
    public GameObject P2IconLarge;
    public GameObject playerIcons;
    public GameObject arrow;
    public GameObject arrow2;
    public GameObject pathToVase;
    public GameObject pathToBoxes;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    public Transform frontHalf;

    [Header("Task Locations")]
    public Transform vaseTask;
    public Transform frontBoxesTask;
    public Transform backBoxesTask;
    
    // State tracking variables
    private int currTutorialStage = 0;
    private bool hasSplit = false;
    private bool hasReconnected = false;
    private bool hasSwitched = false;
    private bool canLeaveAttic = false;
    private FixedJoint fixedJoint;

    // Other variables 
    private Stopwatch stopwatch = new Stopwatch();
    private GameObject emote;

    // Script references
    private GameObject task1;
    private PawPath task1PawPath;
    private GameObject task2;
    private Task2Tutorial task2Tutorial;
    private PawPath task2PawPath;
    private GameObject task3;
    private Task3Tutorial task3Tutorial;
    private PlayerManager playerManager;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;    

    void Awake()
    {
        task1 = GameObject.Find("Task 1");
        task1PawPath = task1.GetComponent<PawPath>();

        task2 = GameObject.Find("Task 2");
        task2PawPath = task2.GetComponent<PawPath>();
        task2Tutorial = task2.GetComponent<Task2Tutorial>();

        task3 = GameObject.Find("Task 3");
        task3Tutorial = task3.GetComponent<Task3Tutorial>();

        playerManager = GameObject.Find("Pet").GetComponent<PlayerManager>();
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateTutorialText();
        fixedJoint = playerManager.getJoint();

        // Set the instructions UI according to keycaps or gamepad
        bool isKeyboard = controllerAssignment.IsKeyboard();
        
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
    }

    void Update()
    {
        if (getCurrTutorialStage() == tutMoveToVase)
        {
            arrow.SetActive(true);
            arrow.transform.position = vaseTask.position + (Vector3.up * 0.30f);
            arrow.transform.LookAt(frontHalf);
            arrow.transform.rotation = Quaternion.Euler(180, 0, 0);
            // make the arrow float
            arrow.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.05f, 0);
        }

        if (getCurrTutorialStage() == tutScatterBoxes)
        {
            arrow.SetActive(true);
            arrow2.SetActive(true);
            arrow.transform.position = frontBoxesTask.position + (Vector3.up * 0.30f);
            arrow2.transform.position = backBoxesTask.position + (Vector3.up * 0.30f);
            arrow.transform.LookAt(frontHalf);
            arrow2.transform.LookAt(frontHalf);
            arrow.transform.rotation = Quaternion.Euler(180, 0, 0);
            arrow2.transform.rotation = Quaternion.Euler(180, 0, 0);

            arrow.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.05f, 0);
            arrow2.transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.05f, 0);
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
        if (getCurrTutorialStage() == 3 && checkForReturn())
        {
            advanceTutorialStage();
        }

        // check for keypress / gamepad press to close the tutorial
        if (getCurrTutorialStage() == tutComplete && checkForReturn())
        {
            leaveTutorial();
        }

        // check for second switch to exit attic
        if (getCurrTutorialStage() == scaredDog && checkForReturn())
        {
            advanceTutorialStage();
        }
        // if (getCurrTutorialStage() == annoyedCat)
        // {
        //     yield null WaitFOr
        //     speechBubbleRight.SetActive(true);
        //     tutorialText.text == "";
        // }
    }

    public const int tutMoveToVase = 0;
    public const int tutBreakVase = 1;
    public const int tutSplit = 2;
    public const int tutScatterBoxes = 4;
    public const int tutReconnect = 5;
    public const int tutMoveToRug = 6;
    public const int tutSwitch = 7;
    public const int tutDragRug = 8;
    public const int tutComplete = 9;
    public const int scaredDog = 10;
    public const int annoyedCat = 11;

    // Update is called once per frame
    private void updateTutorialText()
    {
        switch (currTutorialStage)
        {
            case tutMoveToVase:
                // Speech UI
                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "Let's move to the vase.";
                movementUI.SetActive(true);

                // Arrow logic is in Update()

                // Paw path 
                task2PawPath.setActive(true); 

                break;
            case tutBreakVase:
                // Deactivate stuff from prev case 
                arrow.SetActive(false);
                task2PawPath.setActive(false);
                movementUI.SetActive(false);

                // Speech UI
                tutorialText.text = "Hmm... can we break the vase?";
                jumpUI.SetActive(true);

                break;
            case tutSplit:
                jumpUI.SetActive(false);
                task2Tutorial.enabled = false;

                tutorialText.text = "Chaos! Now, let's split apart.";
                splitUI.SetActive(true);

                break;
            case 3:
                splitUI.SetActive(false);

                speechBubbleTwoTails.SetActive(false);
                speechBubbleLeft.SetActive(true);
                
                tutorialText.text = "Creepy...";
                pressEnterToContinueUI.SetActive(true);

                // emote = playerManager.startEmote(playerManager.getBackHalf(), "sad");
                // play sad dog sound
                break;
            // case 4:
            //     UnityEngine.Debug.Log("case 4");
            //     playerManager.cancelEmote(emote);
            //     tutorialText.text = "sorry, i take that back.";
                
            //     emote = playerManager.startEmote(playerManager.getBackHalf(), "happy");
            //     // play happy dog sound
            //     break;              
            case tutScatterBoxes:
                // Cancel prev case
                pressEnterToContinueUI.SetActive(false);
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);
                // playerManager.cancelEmote(emote); 

                // Speech UI
                tutorialText.text = "Let's scatter the coloured boxes around."; 

                // Arrow logic in Update()
                
                // Paw
                task1PawPath.setActive(true); 

                break;
            case tutReconnect:
                arrow.SetActive(false);
                arrow2.SetActive(false);
                task1PawPath.setActive(false);

                tutorialText.text = "Yay! let's sow ourselves back together."; // speech bubble text
                reconnectUI.SetActive(true); // small text

                break;
            case tutMoveToRug:
                reconnectUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleLeft.SetActive(true);
                tutorialText.text = "Hey, what's under that purple rug?"; // speech bubble text

                break;
            case tutSwitch:
                task3Tutorial.enabled = false;

                tutorialText.text = "I can't grab this, can you help?";
                switchUI.SetActive(true);
                
                break;
            case tutDragRug:
                switchUI.SetActive(false);
                speechBubbleLeft.SetActive(false);

                speechBubbleRight.SetActive(true);
                tutorialText.text = "Woah, I'm at the front now!";
                grabUI.SetActive(true);

                break;
            case tutComplete:
                grabUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "Let's wreck this house!";
                tutorialSmallText.text = "Leave the attic, or take a look around";
                messageManager.startPressEnterToHideTutorial();

                // next case activated in AtticPrevention.cs
                break;
            
            case scaredDog:
                messageManager.cancelPressEnterToHideTutorial();
                speechBubbleLeft.SetActive(false);
                speechBubbleTwoTails.SetActive(false);
                tutorialSmallText.text = "";

                speechBubbleRight.SetActive(true);
                tutorialText.text = "The drop's too high... I'm scared!";
                pressEnterToContinueUI.SetActive(true);
                break;
            case annoyedCat:
                pressEnterToContinueUI.SetActive(false);
                speechBubbleRight.SetActive(false);

                speechBubbleLeft.SetActive(true);
                tutorialText.text = "Fine, I'll jump. Switch with me.";
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

    public bool checkForReturn()
    {
        return Input.GetKeyDown(KeyCode.Return) || 
               player1Input.GetSwitchPressed() || player1Input.GetReconnectPressed() ||
               player2Input.GetSwitchPressed() || player2Input.GetReconnectPressed();
    }


    // Coroutines
    private IEnumerator waitForSeconds(float seconds)
    {
        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < seconds) 
        {
            yield return null;
        }

        stopwatch.Reset();
        advanceTutorialStage();
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
        playerIcons.SetActive(true); 
    }
    
}
