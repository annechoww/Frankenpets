using System.Collections;
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
    public GameObject P1SpeechIcons;
    public GameObject P2SpeechIcons;
    public GameObject miniPlayerIcons;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    
    private int currStage = 0;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;

    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
    }

    void Start()
    {
        SetupControlsUI(); // Configure UI based on input method
        StartCoroutine(TutorialSequence()); // Start tutorial progression
    }

    private void SetupControlsUI()
    {
        bool isKeyboard = controllerAssignment.IsKeyboard();
        glowUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        accessControlsUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);

        glowUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        accessControlsUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
    }

    private IEnumerator TutorialSequence()
    {
        speechBubbleTwoTails.SetActive(true);
        pressEnterToContinueUI.SetActive(true);

        yield return ShowMessage("we made it level 2: the living room!");
        yield return ShowMessage("explore the house and...");
        yield return ShowMessage("complete the to-do list to advance to the next level");
        yield return ShowMessage("if you don't know how to do something...");

        pressEnterToContinueUI.SetActive(false);
        yield return ShowMessage("...you can make interactable objects glow", "glow");
        yield return ShowMessage("take a look at the controls menu, too", "menu");

        messageManager.startPressEnterToHideTutorial();
        yield return ShowMessage("we're all set!");

        EndTutorial();
    }

    private IEnumerator ShowMessage(string message, string special = "")
    {
        tutorialText.text = message;

        if (special == "glow") 
        {
            glowUI.SetActive(true);
            yield return WaitForGlow();
            glowUI.SetActive(false);
        }
        else if (special == "menu") 
        {
            accessControlsUI.SetActive(true);
            yield return WaitForMenu();
            accessControlsUI.SetActive(false);
        }
        else yield return WaitForKey();
    }

    private IEnumerator WaitForKey()
    {
        // delay to prevent instant skipping if key was already down
        yield return new WaitForSeconds(0.1f);

        // wait for the key to be released first to prevent skipping the message
        while (Input.GetKey(KeyCode.Return) || 
            player1Input.GetSwitchPressed() || player1Input.GetReconnectPressed() ||
            player2Input.GetSwitchPressed() || player2Input.GetReconnectPressed())
        {
            yield return null;
        }
        
        while (!Input.GetKeyDown(KeyCode.Return) &&
               !player1Input.GetSwitchPressed() && !player1Input.GetReconnectPressed() &&
               !player2Input.GetSwitchPressed() && !player2Input.GetReconnectPressed())
        {
            yield return null;
        }
    }

    private IEnumerator WaitForGlow()
    {
        while (!Input.GetKeyDown(KeyCode.V) && !Input.GetKeyDown(KeyCode.M) &&
               !player1Input.GetGlowPressed() && !player2Input.GetGlowPressed())
        {
            yield return null;
        }
    }

    private IEnumerator WaitForMenu()
    {
        while (!Input.GetKeyDown(KeyCode.I) &&
               !player1Input.GetControlsMenuPressed() && !player2Input.GetControlsMenuPressed())
        {
            yield return null;
        }
    }

    private void EndTutorial()
    {
        speechBubbleTwoTails.SetActive(false);
        tutorialText.text = "";
        tutorialSmallText.text = "";
        messageManager.cancelPressEnterToHideTutorial();
        P1SpeechIcons.SetActive(false);
        P2SpeechIcons.SetActive(false);

        // Show player icons
        miniPlayerIcons.transform.GetChild(0).gameObject.SetActive(true);
        miniPlayerIcons.transform.GetChild(1).gameObject.SetActive(true);
    }
}
