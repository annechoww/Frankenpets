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
    private FixedJoint fixedJoint;

    // Other variables 
    private Stopwatch stopwatch = new Stopwatch();
    private GameObject emote;

    // Script references
    private Task2Tutorial task2Tutorial;
    private Task3Tutorial task3Tutorial;
    private PlayerManager playerManager;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;
    

    void Awake()
    {
        task2Tutorial = GameObject.Find("Task 2").GetComponent<Task2Tutorial>();
        task3Tutorial = GameObject.Find("Task 3").GetComponent<Task3Tutorial>();
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
        if (controllerAssignment.IsKeyboard())
        {
            movementUI.transform.GetChild(0).gameObject.SetActive(true);
            jumpUI.transform.GetChild(0).gameObject.SetActive(true);
            splitUI.transform.GetChild(0).gameObject.SetActive(true);
            reconnectUI.transform.GetChild(0).gameObject.SetActive(true);
            switchUI.transform.GetChild(0).gameObject.SetActive(true);
            grabUI.transform.GetChild(0).gameObject.SetActive(true);
            pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(true);

            movementUI.transform.GetChild(1).gameObject.SetActive(false);
            jumpUI.transform.GetChild(1).gameObject.SetActive(false);
            splitUI.transform.GetChild(1).gameObject.SetActive(false);
            reconnectUI.transform.GetChild(1).gameObject.SetActive(false);
            switchUI.transform.GetChild(1).gameObject.SetActive(false);
            grabUI.transform.GetChild(1).gameObject.SetActive(false);
            pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            movementUI.transform.GetChild(0).gameObject.SetActive(false);
            jumpUI.transform.GetChild(0).gameObject.SetActive(false);
            splitUI.transform.GetChild(0).gameObject.SetActive(false);
            reconnectUI.transform.GetChild(0).gameObject.SetActive(false);
            switchUI.transform.GetChild(0).gameObject.SetActive(false);
            grabUI.transform.GetChild(0).gameObject.SetActive(false);
            pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(false);

            movementUI.transform.GetChild(1).gameObject.SetActive(true);
            jumpUI.transform.GetChild(1).gameObject.SetActive(true);
            splitUI.transform.GetChild(1).gameObject.SetActive(true);
            reconnectUI.transform.GetChild(1).gameObject.SetActive(true);
            switchUI.transform.GetChild(1).gameObject.SetActive(true);
            grabUI.transform.GetChild(1).gameObject.SetActive(true);
            pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (getCurrTutorialStage() == tutMoveToVase)
        {
            arrow.SetActive(true);
            arrow.transform.position = vaseTask.position + (Vector3.up * 0.30f);
            arrow.transform.LookAt(frontHalf);
            arrow.transform.rotation = Quaternion.Euler(180, 0, 0);
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
                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "let's move to the vase"; // speech bubble text
                movementUI.SetActive(true); // small text

                // todo: vase arrow

                break;
            case tutBreakVase:
                arrow.SetActive(false);
                movementUI.SetActive(false);

                tutorialText.text = "hmm... can we break the vase?";
                jumpUI.SetActive(true);

                break;
            case tutSplit:
                jumpUI.SetActive(false);

                task2Tutorial.enabled = false;

                tutorialText.text = "chaos! now, let's split apart";
                splitUI.SetActive(true);

                break;
            case 3:
                splitUI.SetActive(false);

                speechBubbleTwoTails.SetActive(false);
                speechBubbleLeft.SetActive(true);
                
                tutorialText.text = "creepy...";
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
                pressEnterToContinueUI.SetActive(false);
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);
                // playerManager.cancelEmote(emote); 

                tutorialText.text = "let's scatter the coloured boxes around"; 

                // todo: show arrows

                break;
            case tutReconnect:
                arrow.SetActive(false);
                arrow2.SetActive(false);
                tutorialText.text = "yay! let's sow ourselves back together"; // speech bubble text
                reconnectUI.SetActive(true); // small text

                break;
            case tutMoveToRug:
                reconnectUI.SetActive(false);

                speechBubbleRight.SetActive(false);
                speechBubbleLeft.SetActive(true);

                tutorialText.text = "hey, what's under that purple rug?"; // speech bubble text

                break;
            case tutSwitch:
                task3Tutorial.enabled = false;

                tutorialText.text = "i can't grab this, can you help?";
                switchUI.SetActive(true);
                
                break;
            case tutDragRug:
                switchUI.SetActive(false);
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);

                tutorialText.text = "woah, i'm at the front now!";
                grabUI.SetActive(true);

                break;
            case tutComplete:
                grabUI.SetActive(false);
                speechBubbleRight.SetActive(false);
                speechBubbleTwoTails.SetActive(true);
                
                tutorialText.text = "let's wreck this house!";
                tutorialSmallText.text = "leave the attic, or take a look around";
                messageManager.startPressEnterToHideTutorial();

                // go to next stage when touch attic door again during case tut complete

                break;

            // not used yet
            case scaredDog:
                speechBubbleLeft.SetActive(true);
                speechBubbleRight.SetActive(false);
                tutorialText.text = "the drop's too high, i'm scared!";
                // StartCoroutine(waitForSkip(scaredDog));
                break;
            case annoyedCat:
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);
                tutorialText.text = "fine, i'll jump. switch with me.";
                // StartCoroutine(waitForSkip(annoyedCat));
                break;
        }
    }

    public void advanceTutorialStage()
    {
        // Debug.Log("advanced to next tutorial stage");
        currTutorialStage++;
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

    private IEnumerator waitForKey(KeyCode key)
    {
        while (!Input.GetKeyDown(key)) 
        {
            yield return null;
        }

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
