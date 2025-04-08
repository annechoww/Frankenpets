using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class TutorialOverlayStep
{
    public GameObject overlay;
    public string description;
    public bool useDoubleOverlay;
    public bool adjustTodoListSorting;
    public int todoListSortingOrder;
    public bool adjustCornerControlsSorting;
    public int cornerControlsSortingOrder;
    public bool hideOverlayAfter;

    public bool moveContinueTextDown;
}

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
    public GameObject switchUI;

    public GameObject leftTutIcon;
    public GameObject rightTutIcon;
    private Animator leftTutAnimator;
    private Animator rightTutAnimator;
    private Animator continueTutAnimator;
    private Animator continueParentTutAnimator;

    private List<Task> livingRoomTasks;
    private bool finishedTasks = false;
    private bool hasShownBackyardMessage = false;
    private bool dogTryingToGetIn;
    private bool hasShownAntiDogMessage = false;


    [Header("Tutorial Overlay Sequence")]
    public List<TutorialOverlayStep> tutorialSteps = new List<TutorialOverlayStep>();
    public GameObject overlay;
    public GameObject overlayBG;
    public GameObject singleOverlay;
    public GameObject doubleOverlay;

    [Header("Icons")]
    public GameObject P1SpeechIcons;
    public GameObject P2SpeechIcons;
    // public GameObject miniPlayerIcons;

    [Header("Black Overlays and Highlight Variables")]
    public GameObject bottomUIParentHighlight;
    public Canvas todoListCanvas;
    public Canvas cornerControlsCanvas;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;
    
    private MessageManager messageManager;
    private ControllerAssignment controllerAssignment;
    private ControlsCornerUI cornerControls;

    void Awake()
    {
        messageManager = GameObject.Find("Messages").GetComponent<MessageManager>();
        controllerAssignment = ControllerAssignment.Instance;
        cornerControls = GameObject.Find("MiniControlsUI").GetComponent<ControlsCornerUI>();
        
        Screen.SetResolution(1920, 1080, true);

        StartCoroutine(WaitForControllerAssignment());
    }

    void Start()
    {
        livingRoomTasks = TaskManager.GetAllTasksOfLevel(1);
    }

    void Update()
    {
        
        finishedTasks = TaskManager.CheckTaskCompletion(livingRoomTasks);
        dogTryingToGetIn = AdvanceToBasement.Instance.GetAntiDogClub();
        
        if (finishedTasks && !hasShownBackyardMessage)
        {
            StartCoroutine(AdvanceToBasementSequence());
            hasShownBackyardMessage = true;
        }

        // Display locate tasks hint
        if (!finishedTasks && HintsManager.Instance.ShouldShowHint())
        {
            StartCoroutine(ShowMessage("HINT: <u>Locate</u> to-do list tasks.", "glow"));
        }

        // if (finishedTasks && dogTryingToGetIn)
        // {
        //     StartCoroutine(DelayFirstAntiDog());
        // } 
        // else if (finishedTasks && !dogTryingToGetIn)
        // {
        //     SetCurrAdvBasementStage(2);
        // }

        // if (finishedTasks && dogTryingToGetIn && !hasShownAntiDogMessage)
        // {
        //     StartCoroutine(ShowMessage("It's too dark; I can't see!", "antiDogClub"));
        //     hasShownAntiDogMessage = true;
        // }

        // if (finishedTasks && !dogTryingToGetIn)
        // {
        //     StartCoroutine(HideEffect(null, speechBubbleRight));
        //     hasShownAntiDogMessage = false;
        // }
        
    }

    private IEnumerator WaitForControllerAssignment()
    {
        // Wait for the ControllerAssignment singleton to exist
        while (ControllerAssignment.Instance == null)
            yield return null;
            
        // Wait for it to be fully initialized
        while (!ControllerAssignment.Instance.IsInitialized())
            yield return null;
            
        // Now it's safe to use
        controllerAssignment = ControllerAssignment.Instance;
        
        // Continue with initialization
        SetupControlsUI();
        overlayUI();
        singleOverlay = overlay.transform.GetChild(0).gameObject;
        doubleOverlay = overlay.transform.GetChild(1).gameObject;
        StartCoroutine(OverlaySequence());
    }

    private IEnumerator OverlaySequence()
    {
        // Initialize overlay references (do this in Start or Awake instead if preferred)
        singleOverlay = overlay.transform.GetChild(0).gameObject;
        doubleOverlay = overlay.transform.GetChild(1).gameObject;
        
        // Make sure all overlays start inactive
        foreach (var step in tutorialSteps)
        {
            if (step.overlay != null)
                step.overlay.SetActive(false);
        }
        
        // Go through each tutorial step
        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            var step = tutorialSteps[i];
            
            // Set up single/double overlay display
            singleOverlay.SetActive(!step.useDoubleOverlay);
            doubleOverlay.SetActive(step.useDoubleOverlay);

            if (step.moveContinueTextDown) {
                continueParentTutAnimator.SetBool("moveDown", true);
            }
            else {
                continueParentTutAnimator.SetBool("moveDown", false);
            }
            
            // Show current overlay
            if (step.overlay != null)
                step.overlay.SetActive(true);
            
            // Apply todo list sorting order if needed
            if (step.adjustTodoListSorting)
                todoListCanvas.sortingOrder = step.todoListSortingOrder;

            // Apply corner controls sorting order if needed
            if (step.adjustCornerControlsSorting)
                cornerControlsCanvas.sortingOrder = step.cornerControlsSortingOrder;
            
            // Wait for player input
            yield return WaitForKeyBoth();
            
            // Transition animation
            yield return tutOverlayAdvance(2f);
            
            // Hide current overlay if specified
            if (step.hideOverlayAfter && step.overlay != null)
                step.overlay.SetActive(false);
            
            // Special handling for final step if needed
            if (i == tutorialSteps.Count - 1)
            {
                // Final cleanup
                overlayBG.SetActive(false);
                overlay.SetActive(false);
                P1SpeechIcons.SetActive(true);
                P2SpeechIcons.SetActive(true);
                cornerControls.setShow(true);
            }
        }
        
        // Start the next phase of the tutorial
        StartCoroutine(TutorialSequence());
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
            overlay.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
        } else if (!isKeyboard){
            overlay.transform.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    private IEnumerator TutorialSequence()
    {   
        // tutorial overlay starts first

        // speech bubbles start now
        StartCoroutine(Highlight(bottomUIParentHighlight));
        yield return ShowMessage("HINT: <u>Locate</u> to-do list tasks.", "glow");
        yield return ShowMessage("HINT: Check out the <u>controls menu</u>.", "menu");

        // messageManager.startPressEnterToHideTutorial();
        // yield return ShowMessage("Let's play!", "end");

        // EndTutorial();
    }

    public int currAdvBasementStage = 0;
    private IEnumerator AdvanceToBasementSequence()
    {
        switch (currAdvBasementStage)
        {
            case 0:
                yield return ShowMessage("There's something in the <u>backyard</u>!", "basement");
                break;
            case 1:
                UnityEngine.Debug.Log("case 1");
                yield return new WaitForSeconds(1f);
                yield return ShowBottomUI(null, speechBubbleRight, "It's too dark; I can't see!");
                yield return WaitForSwitch();
                break;
            case 2:
             UnityEngine.Debug.Log("case 2");
                yield return HideEffect(null, speechBubbleRight);
                break;
        }
    }

    public void SetCurrAdvBasementStage(int stage)
    {
        currAdvBasementStage = stage;
        UnityEngine.Debug.Log($"Current Stage: {currAdvBasementStage}");
    }

    private IEnumerator WaitForSwitch()
    {
        while (dogTryingToGetIn)
        {
            yield return null;
        }

        SetCurrAdvBasementStage(2);
    }

    private IEnumerator DelayFirstAntiDog()
    {
        yield return new WaitForSeconds(1f);
        SetCurrAdvBasementStage(1);
    }

    private IEnumerator ShowMessage(string message, string special = "")
    {
        if (special == "glow") 
        {
            yield return ShowBottomUI(glowUI, speechBubbleTwoTails, message);
            yield return WaitForGlow(0.7f);
            yield return HideEffect(glowUI, speechBubbleTwoTails);
        }
        else if (special == "menu") 
        {
            yield return ShowBottomUI(accessControlsUI, speechBubbleTwoTails, message);
            yield return WaitForMenu(); // Wait to open menu
            // yield return new WaitForSeconds(0.5f);
            yield return WaitForMenu(); // Wait to close menu
            yield return HideEffect(accessControlsUI, speechBubbleTwoTails);
        }
        else if (special == "end")
        {
            yield return ShowBottomUI(null, speechBubbleTwoTails, message);
            yield return new WaitForSeconds(3.0f);
            yield return HideEffect(null, speechBubbleTwoTails);
        }
        else if (special == "basement")
        {
            yield return new WaitForSeconds(5.5f);
            yield return ShowBottomUI(null, speechBubbleTwoTails, message);
            yield return WaitForBasementDoor();
            yield return HideEffect(null, speechBubbleTwoTails);

            // if (dogTryingToGetIn)
            // {
            //     SetCurrAdvBasementStage(1);
            // } 
            // else
            // {
            //     SetCurrAdvBasementStage(2);
            // }
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
                leftTutAnimator.SetBool("pressed", true);
            }
            if (player2Input.GetGlowJustPressed()){
                player2Pressed = true;
                rightTutAnimator.SetBool("pressed", true);
            }
            yield return null;  // Allow waiting until the next frame.
        }
        yield return null;
    }

    private IEnumerator WaitForGlow(float keyDownDuration)
    {
        float timeoutDuration = 20f; // Timeout after 20 seconds
        float elapsedTime = 0f;
        
        while (elapsedTime < timeoutDuration) 
        {
            float keyHoldTime = 0f;

            // Wait until the key is held down for the required duration
            while (keyHoldTime < keyDownDuration)
            {
                if (player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
                {
                    keyHoldTime += Time.deltaTime;
                }
                else
                {
                    keyHoldTime = 0f; // Reset if the key is released
                }

                elapsedTime += Time.deltaTime; // Track amount of time coroutine has been running

                // Break out if exceeded the timeout duration
                if (elapsedTime >= timeoutDuration)
                {
                    yield break;
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
        float timeoutDuration = 10f;
        float elapsedTime = 0f;

        // Wait until either player presses the menu button or timeout occurs
        while (elapsedTime < timeoutDuration)
        {
            if (player1Input.GetControlsMenuPressed() || player2Input.GetControlsMenuPressed())
            {
                yield break; // Exit coroutine if either player presses the menu button
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator WaitForBasementDoor()
    {
        while (true)
        {
            if (AdvanceToBasement.Instance.GetIsAtBasementDoor())
            {
                yield break;
            }

            yield return null;
        }
    }

    private void EndTutorial()
    {
        speechBubbleTwoTails.SetActive(false);
        tutorialText.text = "";
        tutorialSmallText.text = "";
        // messageManager.cancelPressEnterToHideTutorial();
        // P1SpeechIcons.SetActive(false);
        // P2SpeechIcons.SetActive(false);

        // Show player icons
        // miniPlayerIcons.transform.GetChild(0).gameObject.SetActive(true);
        // miniPlayerIcons.transform.GetChild(1).gameObject.SetActive(true);
    }

    // BOTTOM SPEECH UI ///////////////////////////////////////////////
    private bool isCoroutineRunning = false;
    private IEnumerator SlideUpEffect(RectTransform rectTransform)
    {
        Debug.Log("SlideUpEffect started");
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
        Debug.Log("SlideUpEffect ended");
    }

    private IEnumerator SlideDownEffect(RectTransform rectTransform)
    {
        Debug.Log("SlidedownEffect started");
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
        Debug.Log("SlidedownEffect ended");
    }

    private IEnumerator HideEffect(GameObject uiComponent = null, GameObject bubble = null)
    {
        Debug.Log("hideeffect Coroutine started");
        while (isCoroutineRunning)
            yield return null;

        isCoroutineRunning = true;

        RectTransform rectTransform = bottomUIParent;

        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -1086);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        tutorialSmallText.text = "";
        tutorialText.text = "";

        if (uiComponent != null) uiComponent.SetActive(false);
        if (bubble != null) bubble.SetActive(false);
        
        isCoroutineRunning = false;
                Debug.Log("hideeffect Coroutine ednded");

        yield return null;
        
    }

    private IEnumerator ShowBottomUI(GameObject uiComponent = null, GameObject bubble = null, string largeText = "", string smallText = "", bool playSound = true)
    {
        Debug.Log("ShowBottomUI Coroutine started");
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

        bottomUIParent.anchoredPosition = new Vector2(-10, -1086);

        tutorialSmallText.text = smallText;
        tutorialText.text = largeText;

        yield return StartCoroutine(SlideUpEffect(bottomUIParent));
        yield return StartCoroutine(SlideDownEffect(bottomUIParent));
Debug.Log("ShowBottomUI Coroutine ended");
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
