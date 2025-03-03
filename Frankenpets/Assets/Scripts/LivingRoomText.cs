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
    public GameObject pressEnterUI;
    public GameObject glowUI;
    public GameObject accessControlsUI;

    [Header("Icons")]
    public GameObject catIcon;
    public GameObject dogIcon;
    public GameObject playerIcons;
    
    private int currStage = 0;
    private Stopwatch stopwatch = new Stopwatch(); // might need to make new stopwatch every time coroutine is called
    private MessageManager messageManager;

    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateLivingRoomText();
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
                StartCoroutine(waitForKey(KeyCode.Return)); // or controller
                pressEnterUI.SetActive(true);
                break;
            case 1:
                tutorialText.text = "explore the house and...";
                StartCoroutine(waitForKey(KeyCode.Return)); // or controller

                break;

            case 2:
                tutorialText.text = "complete the to-do list to advance to the next level";
                StartCoroutine(waitForKey(KeyCode.Return)); // or controller

                break;
            case 3:
                tutorialText.text = "don't know how to do something?";
                StartCoroutine(waitForKey(KeyCode.Return)); // or controller
                pressEnterUI.SetActive(true);
                break;
            case 4:
                pressEnterUI.SetActive(false);
                tutorialText.text = "you can make interactable objects glow";
                glowUI.SetActive(true);
                StartCoroutine(waitForKey(KeyCode.Return)); // or controller // CHANGE THE CODE
                
                break;
            case 5:
                glowUI.SetActive(false);
                pressEnterUI.SetActive(true);
                tutorialText.text = "take a look at the controls menu, too";
                accessControlsUI.SetActive(true);
                StartCoroutine(waitForKey(KeyCode.Return));
                StartCoroutine(waitForKey(KeyCode.Return));
                break;
            case 6:
                accessControlsUI.SetActive(false);
                pressEnterUI.SetActive(true);
                tutorialText.text = "we're all set! what's that near the stairs?";
                messageManager.startPressEnterToHideTutorial();
                break;
            case dockingStation:
                tutorialText.text = "are these... other halves?!";
                StartCoroutine(waitForSeconds(4.0f));
                break;
            case 8:
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

    private IEnumerator leaveTutorial()
    {
        while (!Input.GetKeyDown(KeyCode.Return)) // or controller 
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
