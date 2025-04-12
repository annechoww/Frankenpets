using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasementText : MonoBehaviour
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

    [Header("Intro Overlay Variables")]
    public GameObject overlay;
    public GameObject overlayBG;
    public GameObject leftTutIcon;
    public GameObject rightTutIcon;
    private Animator leftTutAnimator;
    private Animator rightTutAnimator;
    private Animator continueTutAnimator;
    private Animator continueParentTutAnimator;

    [Header("End Overlay Variables")]
    public GameObject gameOverOverlay;
    public GameObject leftEndIcon;
    public GameObject rightEndIcon;
    private Animator leftEndAnimator;
    private Animator rightEndAnimator;
    private Animator restartGameAnimator;

    [Header("Icons")]
    public GameObject P1SpeechIcons;
    public GameObject P2SpeechIcons;
    // public GameObject miniPlayerIcons;

    [Header("Black Overlays and Highlight Variables")]
    public GameObject bottomUIParentHighlight;
    public Canvas todoListCanvas;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    
    public OutroComicManager outroComicManager;
    public GameObject outroComicCanvas;

    private int currStage = 0;
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;
    private ControlsCornerUI cornerControlsUI;
    private int tutOverlayStage = 1;

    private List<Task> tasks;
    private bool taskComplete = false;
    private bool resetGame = false;
    private GameObject singleOverlay;
    private GameObject doubleOverlay;

    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        
        
        cornerControlsUI = GameObject.Find("MiniControlsUI").GetComponent<ControlsCornerUI>();

        Screen.SetResolution(1920, 1080, true);
    }

    void Start()
    {
        controllerAssignment = FindFirstObjectByType<ControllerAssignment>();
        SetupControlsUI(); // Configure UI based on input method
        overlayUI();
        singleOverlay = overlay.transform.GetChild(0).gameObject;
        doubleOverlay = overlay.transform.GetChild(1).gameObject;
        StartCoroutine(OverlaySequence());
        
        //StartCoroutine(TutorialSequence()); // Start tutorial progression
    }

    private void Update()
    {
        tasks = TaskManager.GetAllTasksOfLevel(2);
        if (TaskManager.CheckTaskCompletion(tasks) && !taskComplete)
        {
            outroComicCanvas.SetActive(true);
            outroComicManager.StartComic(player1Input, player2Input);
            outroComicCanvas.SetActive(false);
            StartCoroutine(EndOverlaySequence());
            taskComplete = true;

        }
    }

    private IEnumerator OverlaySequence()
    {

       yield return WaitForKeyBoth();

       yield return tutOverlayAdvance(0.8f); // prev 2.5f
       
        
        overlayBG.SetActive(false);
        overlay.SetActive(false);
        yield return null;
    }

    private IEnumerator EndOverlaySequence()
    {
        yield return new WaitForSeconds(8.0f);
        gameOverOverlay.SetActive(true);
        yield return WaitForSelectionBoth();
        yield return endOverlayAdvance(0.8f);

        gameOverOverlay.SetActive(false);
        if (resetGame)
        {
            StartCoroutine(PerformRestart());
        }
        yield return null;
    }

    private void overlayUI()
    {
        leftTutAnimator = leftTutIcon.GetComponent<Animator>();
        rightTutAnimator = rightTutIcon.GetComponent<Animator>();
        continueTutAnimator = overlay.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
        continueParentTutAnimator = overlay.transform.GetChild(2).gameObject.GetComponent<Animator>();
        leftTutAnimator.Play("P1 Tut icon", 0, 0f);
        rightTutAnimator.Play("P2 Tut icon", 0, 0f);
        continueTutAnimator.Play("Instruction continue animation", 0, 0f);

        leftEndAnimator = leftTutIcon.GetComponent<Animator>();
        rightEndAnimator = rightTutIcon.GetComponent<Animator>();
        restartGameAnimator = overlay.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
        leftEndAnimator.Play("P1 Tut icon", 0, 0f);
        rightEndAnimator.Play("P2 Tut icon", 0, 0f);
        restartGameAnimator.Play("Instruction continue animation", 0, 0f);
        
        todoListCanvas.sortingOrder = 0;
    }

    private IEnumerator tutOverlayAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        leftTutAnimator.Play("P1 Tut icon", 0, 0f);
        rightTutAnimator.Play("P2 Tut icon", 0, 0f);
        continueTutAnimator.Play("Instruction continue animation", 0, 0f);
        leftTutAnimator.SetBool("pressed", false);
        rightTutAnimator.SetBool("pressed", false);
    }

    private IEnumerator endOverlayAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        leftEndAnimator.Play("P1 Tut icon", 0, 0f);
        rightEndAnimator.Play("P2 Tut icon", 0, 0f);
        restartGameAnimator.Play("Instruction continue animation", 0, 0f);
        leftEndAnimator.SetBool("pressed", false);
        rightEndAnimator.SetBool("pressed", false);
    }

    private void SetupControlsUI()
    {
        bool isKeyboard = controllerAssignment.IsKeyboard();
        //bool isKeyboard = false;
        glowUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        accessControlsUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        pressEnterToContinueUI.transform.GetChild(0).gameObject.SetActive(isKeyboard);

        glowUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        accessControlsUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);
        pressEnterToContinueUI.transform.GetChild(1).gameObject.SetActive(!isKeyboard);

        if (isKeyboard){
            overlay.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
            gameOverOverlay.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
            gameOverOverlay.transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(true);
        } else if (!isKeyboard){
            overlay.transform.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(true);
            gameOverOverlay.transform.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(true);
            gameOverOverlay.transform.GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(true);
        }
    }

    private IEnumerator TutorialSequence()
    {   
        // tutorial overlay starts first

        // speech bubbles start now
        yield return ShowMessage("Try <u>locating</u> to-do list tasks.", "glow");
        yield return ShowMessage("Check out the <u>controls menu</u>, too.", "menu");

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
            StartCoroutine(Highlight(bottomUIParentHighlight));
            yield return ShowBottomUI(glowUI, speechBubbleTwoTails, message);
            yield return WaitForGlow(0.7f);
            yield return HideEffect(glowUI, speechBubbleTwoTails);
            // glowUI.SetActive(false);
        }
        else if (special == "menu") 
        {
            // accessControlsUI.SetActive(true);
            yield return ShowBottomUI(accessControlsUI, speechBubbleTwoTails, message);
            yield return WaitForMenu(); // Wait to open menu
            // yield return new WaitForSeconds(0.5f);
            yield return WaitForMenu(); // Wait to close menu
            yield return HideEffect(accessControlsUI, speechBubbleTwoTails);
            // accessControlsUI.SetActive(false);
        }
        else if (special == "end")
        {
            yield return ShowBottomUI(null, speechBubbleTwoTails, message);
            yield return new WaitForSeconds(3.0f);
            yield return HideEffect(null, speechBubbleTwoTails);
        }
    }

    private IEnumerator WaitForKey() // KEY IS SPACE FOR KEYBOARD
    {
        // delay to prevent instant skipping if key was already down
        // yield return new WaitForSeconds(0.1f);

        // // wait for the key to be released first to prevent skipping the message
        // while (Input.GetKey(KeyCode.Space) || 
        //     player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
        // {
        //     yield return null;
        // }

        // Now wait until either Space is pressed down or glow is just pressed
        while (!Input.GetKeyDown(KeyCode.Space) &&
            !player1Input.GetGlowJustPressed() && !player2Input.GetGlowJustPressed())
        {
            yield return null;
        }
    }

    private IEnumerator WaitForKeyBoth() {
        bool player1Pressed = false;
        bool player2Pressed = false;

        while ((!player1Pressed || !player2Pressed) && !Input.GetKeyDown(KeyCode.Space)) {
            if (player1Input.GetGlowJustPressed()){
                player1Pressed = true;
                AudioManager.Instance.playUIClickSFX();
                leftTutAnimator.SetBool("pressed", true);
            }
            if (player2Input.GetGlowJustPressed()){
                player2Pressed = true;
                AudioManager.Instance.playUIClickSFX();
                rightTutAnimator.SetBool("pressed", true);
            }
            yield return null;  // Allow waiting until the next frame.
        }
        UnityEngine.Debug.Log("pressed space");
        yield return null;
    }

    private IEnumerator WaitForSelectionBoth() {
        bool player1Pressed = false;
        bool player2Pressed = false;

        while (!player1Pressed || !player2Pressed) {
            // Play again
            if (Input.GetKeyDown(KeyCode.Space)) {
                AudioManager.Instance.playUIClickSFX();
                resetGame = true;
                player1Pressed = true;
                player2Pressed = true;
            }
            if (player1Input.GetGlowJustPressed()){
                AudioManager.Instance.playUIClickSFX();
                player1Pressed = true;
                resetGame = true;
            }
            if (player2Input.GetGlowJustPressed()){
                AudioManager.Instance.playUIClickSFX();
                player2Pressed = true;
                resetGame = true;
            }
            
            // Exploring basement
            if (Input.GetKeyDown(KeyCode.Slash)) {
                AudioManager.Instance.playUIClickSFX();
                resetGame = false;
                player1Pressed = true;
                player2Pressed = true;
            }
            if (player1Input.GetSpecialActionJustPressed()){
                AudioManager.Instance.playUIClickSFX();
                player1Pressed = true;
                resetGame = false;
            }
            if (player2Input.GetSpecialActionJustPressed()){
                AudioManager.Instance.playUIClickSFX();
                player2Pressed = true;
                resetGame = false;
            }
            yield return null;  // Allow waiting until the next frame.
        }
        yield return null;
    }

    private IEnumerator WaitForGlow(float keyDownDuration)
    {
        while (true) // Keep running indefinitely
        {
            float elapsedTime = 0f;

            // Wait until the key is held down for the required duration
            while (elapsedTime < keyDownDuration)
            {
                if (player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
                {
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    elapsedTime = 0f; // Reset if the key is released
                }

                yield return null; // Wait for the next frame
            }

            // Required key down time is satisfied.
            // But, if player is still holding key down, continue the coroutine
            while (!player1Input.GetGlowPressed() && !player2Input.GetGlowPressed())
            {
                yield return null;
            }

            // Stop coroutine when player lifts key
            yield break;
        }
    }

    private IEnumerator WaitForMenu()
    {
        while (!player1Input.GetControlsMenuPressed() && !player2Input.GetControlsMenuPressed())
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

    private IEnumerator PerformRestart()
    {
        
        // // Show a loading message or transition effect
        // if (confirmPromptText != null)
        // {
        //     confirmPromptText.text = "Restarting game...";
        // }
        
        // Wait a short time for feedback
        yield return new WaitForSeconds(1.0f);
        
        // Load the initial scene
        Destroy(AudioManager.Instance.gameObject);
        SceneManager.LoadScene("Splash Screen");
    }

    // BOTTOM SPEECH UI ///////////////////////////////////////////////
    private bool isCoroutineRunning = false;
    private IEnumerator SlideUpEffect(RectTransform rectTransform)
    {
        while (isCoroutineRunning)
            yield return null;

        isCoroutineRunning = true;
        
        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(0, -465);
    
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
        Vector2 targetPosition = new Vector2(0, -650);
    
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
        Vector2 targetPosition = new Vector2(0, -1086);
    
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
            if (bubble == speechBubbleLeft) AudioManager.Instance.PlayUIMeowSFX();
            else if (bubble == speechBubbleRight) AudioManager.Instance.PlayUIBarkSFX();
            else
            {
                AudioManager.Instance.PlayUIMeowBarkSFX();
            }
        }

        bottomUIParent.anchoredPosition = new Vector2(0, -1086);

        tutorialSmallText.text = smallText;
        tutorialText.text = largeText;

        yield return StartCoroutine(SlideUpEffect(bottomUIParent));
        yield return StartCoroutine(SlideDownEffect(bottomUIParent));

    }


    // BLACK OVERLAY ///////////////////////////////////////////////////////////
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
}
