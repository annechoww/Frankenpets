using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialSmallText;
    public GameObject speechBubbleTwoTails;
    public GameObject speechBubbleLeft;
    public GameObject speechBubbleRight;
    public GameObject catIcon;
    public GameObject dogIcon;
    public GameObject movementUI;
    public GameObject splitUI;
    public GameObject grabUI;


    private int currTutorialStage = 0;
    private bool hasSplit = false;
    private bool hasReconnected = false;

    private bool hasSwitched = false;

    private Task2Tutorial task2Tutorial;
    private Task3Tutorial task3Tutorial;
    private FixedJoint fixedJoint;
    private PlayerManager playerManager;
    private MessageManager messageManager;
    private Stopwatch stopwatch = new Stopwatch();
    private GameObject emote;

    void Awake()
    {
        task2Tutorial = GameObject.Find("Task 2").GetComponent<Task2Tutorial>();
        task3Tutorial = GameObject.Find("Task 3").GetComponent<Task3Tutorial>();
        playerManager = GameObject.Find("Pet").GetComponent<PlayerManager>();
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateTutorialText();
        fixedJoint = playerManager.getJoint();
    }

    void Update()
    {
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
    }

    public const int tutMoveToVase = 0;
    public const int tutBreakVase = 1;
    public const int tutSplit = 2;
    public const int tutScatterBoxes = 5;
    public const int tutReconnect = 6;
    public const int tutMoveToRug = 7;
    public const int tutSwitch = 8;
    public const int tutDragRug = 9;
    public const int tutComplete = 10;
    // Living room
    public const int lrEnter = 11;

    // Update is called once per frame
    private void updateTutorialText()
    {
        switch (currTutorialStage)
        {
            case tutMoveToVase:
                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "let's move to the vase";
                // tutorialSmallText.text = "P1 WASD                     P2 arrows";
                movementUI.SetActive(true);
                break;
            case tutBreakVase:
                movementUI.SetActive(false);
                tutorialText.text = "hmm... can we break the vase?";
                tutorialSmallText.text = "";
                break;
            case tutSplit:
                task2Tutorial.enabled = false;
                tutorialText.text = "chaos! now, let's split apart";
                // tutorialSmallText.text = "P1 hold W                     P2 hold dArrow";
                splitUI.SetActive(true);
                break;
            case 3:
                speechBubbleTwoTails.SetActive(false);
                speechBubbleRight.SetActive(true);
                splitUI.SetActive(false);
                tutorialText.text = "creepy...";
                tutorialSmallText.text = "";

                emote = playerManager.startEmote(playerManager.getFrontHalf(), "sad");
                // play sad dog sound

                StartCoroutine(waitForSeconds(4.0f));
                // StartCoroutine(waitForKeypress(KeyCode.Return));
                break;
            case 4:
                playerManager.cancelEmote(emote);
                tutorialText.text = "sorry, i take that back.";
                
                emote = playerManager.startEmote(playerManager.getFrontHalf(), "happy");
                // play happy dog sound

                StartCoroutine(waitForSeconds(4.0f));
                // StartCoroutine(waitForKeypress(KeyCode.Return));
                break;              
            case tutScatterBoxes:
                speechBubbleRight.SetActive(false);
                speechBubbleLeft.SetActive(false);
                playerManager.cancelEmote(emote);   
                tutorialText.text = "let's scatter the coloured boxes around!";
                tutorialSmallText.text = ""; // "move the boxes while split apart";
                break;
            case tutReconnect:
                tutorialText.text = "yay! let's sow ourselves back together";
                tutorialSmallText.text = "align your halves and press space";
                break;
            case tutMoveToRug:
                speechBubbleRight.SetActive(true);
                speechBubbleLeft.SetActive(false);
                tutorialText.text = "hey, what's under that pink rug?";
                tutorialSmallText.text = "";
                break;
            case tutSwitch:
                task3Tutorial.enabled = false;
                tutorialText.text = "P1 and P2, switch sides with each other";
                tutorialSmallText.text = "P1 hold LShift                     P2 hold RShift";
                break;
            case tutDragRug:
                tutorialText.text = "that felt weird...";
                // tutorialSmallText.text = "P2 press '/' to grab";
                tutorialSmallText.text = "";
                grabUI.SetActive(true);
                break;
            case tutComplete:
                speechBubbleLeft.SetActive(false);
                speechBubbleTwoTails.SetActive(true);
                grabUI.SetActive(false);
                tutorialText.text = "let's wreck this house!";
                tutorialSmallText.text = "leave the attic, or take a look around first";
                messageManager.customMessage("press return to hide the tutorial");
                if (Input.GetKeyDown(KeyCode.Return)) 
                {
                    catIcon.SetActive(false);
                    dogIcon.SetActive(false);
                    advanceTutorialStage();
                }
                break;
            // case lrEnter:

            //     catIcon.SetActive(false);
            //     dogIcon.SetActive(false);
            //     break;
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

    private IEnumerator waitForKeypress(KeyCode key)
    {
        while (!Input.GetKeyDown(key)) 
        {
            yield return null;
        }

        advanceTutorialStage();
    }
    
}
