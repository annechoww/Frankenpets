using System.Collections;
using TMPro;
using UnityEngine;

public class LivingRoomText : MonoBehaviour
{
    [Header("Text/Instruction Variables")]
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialSmallText;
    public RectTransform bottomUIParent;
    public GameObject speechBubbleTwoTails;
    public GameObject speechBubbleLeft;
    public GameObject speechBubbleRight;
    public GameObject pressEnterToContinueUI;
    public GameObject glowUI;
    public GameObject accessControlsUI;
    public GameObject overlay;
    public GameObject overlayBG;

    [Header("Icons")]
    public GameObject P1SpeechIcons;
    public GameObject P2SpeechIcons;
    // public GameObject miniPlayerIcons;

    [Header("Sounds")]
    public AudioClip barkSound;
    public AudioClip mewSound;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    
    private int currStage = 0;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;
    private ControlsCornerUI cornerControlsUI;
    private int tutOverlayStage = 1;

    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = GameObject.Find("Pet").GetComponent<ControllerAssignment>();
        cornerControlsUI = GameObject.Find("MiniControlsUI").GetComponent<ControlsCornerUI>();

        Screen.SetResolution(1920, 1080, true);
    }

    void Start()
    {
        SetupControlsUI(); // Configure UI based on input method
        StartCoroutine(OverlaySequence());
        
        //StartCoroutine(TutorialSequence()); // Start tutorial progression
    }

    private IEnumerator OverlaySequence()
    {
        
        yield return WaitForKey();
        overlay.transform.GetChild(1).gameObject.SetActive(false);
        overlay.transform.GetChild(2).gameObject.SetActive(true);

        yield return WaitForKey();
        overlay.transform.GetChild(2).gameObject.SetActive(false);
        overlay.transform.GetChild(3).gameObject.SetActive(true);
        
        yield return WaitForKey();
        overlay.transform.GetChild(3).gameObject.SetActive(false);
        overlay.transform.GetChild(4).gameObject.SetActive(true);

        yield return WaitForKey();
        overlay.transform.GetChild(4).gameObject.SetActive(false);
        overlay.transform.GetChild(5).gameObject.SetActive(true);
        
        yield return WaitForKey();
        overlay.transform.GetChild(5).gameObject.SetActive(false);
        overlayBG.SetActive(false);
        overlay.SetActive(false);
        P1SpeechIcons.SetActive(true);
        P2SpeechIcons.SetActive(true);

        cornerControlsUI.setShow(true);
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

        if (isKeyboard){
            overlay.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        } else if (!isKeyboard){
            overlay.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    private IEnumerator TutorialSequence()
    {   
        // tutorial overlay starts first

        // speech bubbles start now
        yield return ShowMessage("You can make to-do list items glow.", "glow");
        yield return ShowMessage("Take a look at the controls menu, too.", "menu");

        // messageManager.startPressEnterToHideTutorial();
        yield return ShowMessage("Let's play!", "end");

        EndTutorial();
    }

    private IEnumerator ShowMessage(string message, string special = "")
    {
        // tutorialText.text = message;

        if (special == "glow") 
        {
            // glowUI.SetActive(true);
            yield return ShowBottomUI(glowUI, speechBubbleTwoTails, message);
            yield return WaitForGlow();
            yield return HideEffect(glowUI, speechBubbleTwoTails);
            // glowUI.SetActive(false);
        }
        else if (special == "menu") 
        {
            // accessControlsUI.SetActive(true);
            yield return ShowBottomUI(accessControlsUI, speechBubbleTwoTails, message);
            yield return WaitForMenu();
            yield return HideEffect(accessControlsUI, speechBubbleTwoTails);
            // accessControlsUI.SetActive(false);
        }
        else if (special == "end")
        {
            yield return ShowBottomUI(pressEnterToContinueUI, speechBubbleTwoTails, message);
            yield return WaitForKey();
            yield return HideEffect(pressEnterToContinueUI, speechBubbleTwoTails);
        }
    }

    private IEnumerator WaitForKey() // KEY IS SPACE FOR KEYBOARD
    {
        // delay to prevent instant skipping if key was already down
        // yield return new WaitForSeconds(0.1f);

        // wait for the key to be released first to prevent skipping the message
        while (Input.GetKey(KeyCode.Space) || 
            player1Input.GetSwitchPressed() || player1Input.GetReconnectPressed() ||
            player2Input.GetSwitchPressed() || player2Input.GetReconnectPressed())
        {
            yield return null;
        }

        while (!Input.GetKeyDown(KeyCode.Space) &&
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
        // miniPlayerIcons.transform.GetChild(0).gameObject.SetActive(true);
        // miniPlayerIcons.transform.GetChild(1).gameObject.SetActive(true);
    }

    // BOTTOM SPEECH UI ///////////////////////////////////////////////
    private bool isCoroutineRunning = false;
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
        Vector2 targetPosition = new Vector2(-10, -650);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        isCoroutineRunning = false;
    }

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

    }
}
