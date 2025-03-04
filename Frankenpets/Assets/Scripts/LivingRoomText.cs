using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class LivingRoomText : MonoBehaviour
{
    [Header("Text/Instruction Variables")]
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialSmallText;
    public GameObject speechBubbleTwoTails;
    public GameObject speechBubbleLeft;
    public GameObject speechBubbleRight;
    public GameObject pressEnterToContinueUI;
    public GameObject glowUI;
    public GameObject accessControlsUI;

    [Header("Icons")]
    public GameObject catIcon;
    public GameObject dogIcon;
    public GameObject playerIcons;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    
    private int currStage = 0;
    private Stopwatch stopwatch = new Stopwatch(); // might need to make new stopwatch every time coroutine is called
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;


    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateLivingRoomText();

        // Set the instructions UI according to keycaps or gamepad
        if (controllerAssignment.IsKeyboard())
        {
            glowUI.transform.GetChild(0).gameObject.SetActive(true);
            accessControlsUI.transform.GetChild(0).gameObject.SetActive(true);
            pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(true);

            glowUI.transform.GetChild(1).gameObject.SetActive(false);
            accessControlsUI.transform.GetChild(1).gameObject.SetActive(false);
            pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            glowUI.transform.GetChild(0).gameObject.SetActive(false);
            accessControlsUI.transform.GetChild(0).gameObject.SetActive(false);
            pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(false);

            glowUI.transform.GetChild(1).gameObject.SetActive(true);
            accessControlsUI.transform.GetChild(1).gameObject.SetActive(true);
            pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public const int dockingStation = 7;
    public void updateLivingRoomText()
    {
        switch (currStage)
        {
            case 0:
                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "we made it level 2: the living room!";
                StartCoroutine(waitForSkip());
                pressEnterToContinueUI.SetActive(true);
                break;
            case 1:
                tutorialText.text = "explore the house and...";
                StartCoroutine(waitForSkip());
                break;
            case 2:
                tutorialText.text = "complete the to-do list to advance to the next level";
                StartCoroutine(waitForSkip());
                break;
            case 3:
                tutorialText.text = "don't know how to do something?";
               StartCoroutine(waitForSkip());
                pressEnterToContinueUI.SetActive(true);
                break;
            case 4:
                pressEnterToContinueUI.SetActive(false);
                tutorialText.text = "you can make interactable objects glow";
                glowUI.SetActive(true);
                StartCoroutine(waitForGlow());
                
                break;
            case 5:
                glowUI.SetActive(false);
                pressEnterToContinueUI.SetActive(true);
                tutorialText.text = "take a look at the controls menu, too";
                accessControlsUI.SetActive(true);
                StartCoroutine(waitForMenu());
                break;
            case 6:
                accessControlsUI.SetActive(false);
                pressEnterToContinueUI.SetActive(true);
                tutorialText.text = "we're all set!"; // what's that near the stairs?
                messageManager.startPressEnterToHideTutorial();
                break;
            // case dockingStation:
            //     tutorialText.text = "are these... other halves?!";
            //     StartCoroutine(waitForSeconds(4.0f));
            //     break;
            case 7:
                leaveTutorial();
                break;
        }
    }

    public void advanceLivingRoomStage()
    {
        currStage++;
        updateLivingRoomText();
    }

    public int getCurrLivingRoomStage()
    {
        return currStage;
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
        advanceLivingRoomStage();
    }

    private IEnumerator waitForKey(KeyCode key)
    {
        while (!Input.GetKeyDown(key)) 
        {
            yield return null;
        }

        advanceLivingRoomStage();
    }

    private IEnumerator waitForSkip()
    {
        while (!Input.GetKeyDown(KeyCode.Return) && 
                !player1Input.GetSwitchPressed() && !player1Input.GetReconnectPressed() &&
                !player2Input.GetSwitchPressed() && !player2Input.GetReconnectPressed()) 
        {
            yield return null;
        }

        advanceLivingRoomStage();
    }

    private IEnumerator waitForGlow()
    {
        while (!Input.GetKeyDown(KeyCode.V) && !Input.GetKeyDown(KeyCode.M) && 
                !player1Input.GetGlowPressed() && !player2Input.GetGlowPressed()) 
        {
            yield return null;
        }

        advanceLivingRoomStage();
    }

    private IEnumerator waitForMenu()
    {
        while (!Input.GetKeyDown(KeyCode.I) && 
                !player1Input.GetControlsMenuPressed() && !player2Input.GetControlsMenuPressed())
        {
            yield return null;
        }

        advanceLivingRoomStage();
    }

    private IEnumerator leaveTutorial()
    {
        while (!Input.GetKeyDown(KeyCode.Return) && 
                !player1Input.GetSwitchPressed() && !player1Input.GetReconnectPressed() &&
                !player2Input.GetSwitchPressed() && !player2Input.GetReconnectPressed())
        {
            yield return null;
        }

        // hide speech bubble stuff 
        speechBubbleTwoTails.SetActive(false);
        tutorialText.text = "";
        tutorialSmallText.text = "";
        messageManager.cancelPressEnterToHideTutorial();
        catIcon.SetActive(false);
        dogIcon.SetActive(false);

        // show the player icons
        playerIcons.transform.GetChild(0).gameObject.SetActive(true); 
        playerIcons.transform.GetChild(1).gameObject.SetActive(true);
    }
}
