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
    public GameObject catIconP1Left;
    public GameObject dogIconP1Left;
    public GameObject catIconP2Right;
    public GameObject dogIconP2Right;
    public GameObject playerIcons;
    public GameObject movementUI;
    public GameObject jumpUI;
    public GameObject splitUI;
    public GameObject reconnectUI;
    public GameObject switchUI;
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
    public const int scaredDog = 11;
    public const int annoyedCat = 12;

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

                emote = playerManager.startEmote(playerManager.getBackHalf(), "sad");
                // play sad dog sound

                StartCoroutine(waitForSeconds(3.0f));
                // StartCoroutine(waitForKeypress(KeyCode.Return));
                
                break;
            case 4:
                playerManager.cancelEmote(emote);
                tutorialText.text = "sorry, i take that back.";
                
                emote = playerManager.startEmote(playerManager.getBackHalf(), "happy");
                // play happy dog sound

                StartCoroutine(waitForSeconds(3.0f));
                // StartCoroutine(waitForKeypress(KeyCode.Return));
                break;              
            case tutScatterBoxes:
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);
                playerManager.cancelEmote(emote); 

                tutorialText.text = "let's scatter the coloured boxes around"; 

                // todo: show arrows

                break;
            case tutReconnect:
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

                StartCoroutine(waitToLeaveTutorial());

                // go to next stage when touch attic door again during case tut complete

                break;
            case scaredDog:
                speechBubbleLeft.SetActive(true);
                speechBubbleRight.SetActive(false);
                tutorialText.text = "the drop's too high, i'm scared!";
                StartCoroutine(waitForSeconds(4.0f));
                break;
            case annoyedCat:
                speechBubbleLeft.SetActive(false);
                speechBubbleRight.SetActive(true);
                tutorialText.text = "fine, i'll jump. switch with me.";
                StartCoroutine(waitForSeconds(4.0f));
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
    }

    private IEnumerator waitToLeaveTutorial()
    {
        while (!Input.GetKeyDown(KeyCode.Return)) 
        {
            yield return null;
        }

        // hide speech bubble stuff 
        speechBubbleTwoTails.SetActive(false);
        tutorialText.text = "";
        tutorialSmallText.text = "";
        messageManager.cancelPressEnterToHideTutorial();
        catIconP1Left.SetActive(false);
        dogIconP2Right.SetActive(false);

        // show the player icons
        playerIcons.transform.GetChild(0).gameObject.SetActive(true); 
        playerIcons.transform.GetChild(1).gameObject.SetActive(true);
    }
    
}
