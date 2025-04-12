using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OutroComicManager : MonoBehaviour
{
    [Header("Comic Panels")]
    public List<GameObject> comicPanels;
    
    [Header("Transition Settings")]
    public List<float> panelDurations; // Time each panel stays on screen
    public List<bool> useFadeTransition; // Whether to fade between panels
    public float fadeTime = 0.5f; // How long fade transitions take
    public Image fadeOverlay;
    public Color fadeColor = Color.black; // Color of the fade overlay
    
    [Header("Skip Settings")]
    public Image skipProgressIndicator;
    public float timeToSkip = 1.5f; // How long both players need to hold B
    public GameObject skipPromptContainer; // Container for the skip UI elements
    
    [Header("Audio")]
    public AudioClip comicBackgroundMusic; // Background music for the comic
    public List<AudioClip> panelSoundEffects; // Sound effects for each panel
    public AudioClip skipSound; // Sound played when skipping the comic
    
    // Private variables
    private int currentPanelIndex = -1;
    private bool comicActive = false;
    private float skipProgress = 0f;
    private bool skipping = false;
    
    // References to other components
    private InputHandler player1Input;
    private InputHandler player2Input;
    private Startup startupScript;

    // Coroutine references
    private Coroutine panelSequenceCoroutine;

    /// <summary>
    /// Starts playing the comic sequence
    /// </summary>
    /// <param name="p1Input">Player 1 input handler</param>
    /// <param name="p2Input">Player 2 input handler</param>
    /// <param name="startup">Reference to the startup script</param>
    public void StartComic(InputHandler p1Input, InputHandler p2Input, Startup startup)
    {
        // Store references
        player1Input = p1Input;
        player2Input = p2Input;
        startupScript = startup;
        
        // Reset state
        currentPanelIndex = -1;
        comicActive = true;
        skipProgress = 0f;
        skipping = false;
        
        // Hide all panels initially
        foreach (GameObject panel in comicPanels)
        {
            panel.SetActive(false);
        }
        
        // Show skip prompt
        skipPromptContainer.SetActive(true);
        skipProgressIndicator.fillAmount = 0f;
        
        // Start playing music if available
        if (comicBackgroundMusic != null)
        {
            AudioManager.Instance.PlayMusic(comicBackgroundMusic);

        }
        
        // Start the panel sequence
        panelSequenceCoroutine = StartCoroutine(PlayComicSequence());
    }

    /// <summary>
    /// Coroutine that plays through the comic panels in sequence
    /// </summary>
    private IEnumerator PlayComicSequence()
    {
        // Validate panel counts
        if (comicPanels.Count == 0)
        {
            Debug.LogError("No comic panels assigned!");
            EndComic();
            yield break;
        }
        
        // Ensure we have enough durations and transition settings
        while (panelDurations.Count < comicPanels.Count)
        {
            panelDurations.Add(3f); // Default duration
        }
        
        while (useFadeTransition.Count < comicPanels.Count)
        {
            useFadeTransition.Add(true); // Default to fade
        }
        
        // Go through each panel
        for (int i = 0; i < comicPanels.Count; i++)
        {
            // Show the next panel
            yield return ShowPanel(i);
            
            // Wait for the panel duration
            float timer = 0f;
            while (timer < panelDurations[i] && !skipping)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Don't transition after the last panel
            if (i < comicPanels.Count - 1 && !skipping)
            {
                // Transition to the next panel
                if (useFadeTransition[i])
                {
                    yield return FadeOutPanel(i);
                }
                else
                {
                    // Just hide the current panel
                    comicPanels[i].SetActive(false);
                }
            }
            else if (i == comicPanels.Count - 1)
            {
                // This is the last panel - fade to black
                yield return FadeToBlack();
            }
        }
        
        // End the comic sequence
        EndComic();
    }

    private IEnumerator FadeToBlack()
    {
        float fadeToBlackTime = 3f;
        // Set the fade overlay color to black
        fadeOverlay.color = Color.black;
        CanvasGroup fadeGroup = fadeOverlay.GetComponent<CanvasGroup>();
        
        // Start with overlay invisible
        fadeGroup.alpha = 0f;
        
        // Fade to black
        float timer = 0f;
        while (timer < fadeToBlackTime && !skipping)
        {
            fadeGroup.alpha = timer / fadeToBlackTime;
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Ensure overlay is fully black
        fadeGroup.alpha = 1f;
        
        // Hide the last panel - but keep the black overlay
        comicPanels[comicPanels.Count - 1].SetActive(false);
    }
    
    /// <summary>
    /// Shows a specific panel with proper effects
    /// </summary>
    private IEnumerator ShowPanel(int panelIndex)
    {
        currentPanelIndex = panelIndex;
    
        // Play sound effect if available
        if (panelIndex < panelSoundEffects.Count && panelSoundEffects[panelIndex] != null)
        {
            AudioManager.Instance.PlaySFX(panelSoundEffects[panelIndex]);
        }
        
        // Get the panel and prepare for transition
        GameObject panel = comicPanels[panelIndex];
        
        // If using fade transition
        if (useFadeTransition[panelIndex])
        {
            fadeOverlay.gameObject.SetActive(true);
            // Set the fade overlay color
            fadeOverlay.color = fadeColor;
            CanvasGroup fadeGroup = fadeOverlay.GetComponent<CanvasGroup>();
            
            // Start with overlay visible
            fadeGroup.alpha = 1f;
            
            // Show the panel (it will be behind the overlay)
            panel.SetActive(true);
            
            // Fade out the overlay to reveal the panel
            float timer = 0f;
            while (timer < fadeTime && !skipping)
            {
                fadeGroup.alpha = 1f - (timer / fadeTime);
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Ensure overlay is invisible at the end
            if (!skipping)
            {
                fadeGroup.alpha = 0f;
            }
        }
        else
        {
            fadeOverlay.gameObject.SetActive(false);
            // Simple cut, no fade
            panel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Fades out a specific panel
    /// </summary>
    private IEnumerator FadeOutPanel(int panelIndex)
    {
        GameObject panel = comicPanels[panelIndex];
    
        // If using fade transition
        if (useFadeTransition[panelIndex])
        {
            // Set the fade overlay color
            fadeOverlay.color = fadeColor;
            CanvasGroup fadeGroup = fadeOverlay.GetComponent<CanvasGroup>();
            
            // Start with overlay invisible
            fadeGroup.alpha = 0f;
            
            // Fade in the overlay to hide the current panel
            float timer = 0f;
            while (timer < fadeTime && !skipping)
            {
                fadeGroup.alpha = timer / fadeTime;
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Ensure overlay is fully visible
            if (!skipping)
            {
                fadeGroup.alpha = 1f;
            }
        }
        
        // Hide the panel
        panel.SetActive(false);
    }
    
    /// <summary>
    /// Ends the comic sequence and transitions to character selection
    /// </summary>
    private void EndComic()
    {
        // Hide all panels
        foreach (GameObject panel in comicPanels)
        {
            panel.SetActive(false);
        }
        
        // NOTE: We don't hide the fade overlay - it stays black
        // until the next level loads
        
        // Hide skip prompt
        skipPromptContainer.SetActive(false);
        
        // Set state
        comicActive = false;
        
        // Tell the startup script we're done
        if (startupScript != null)
        {
            startupScript.OnComicComplete();
        }
    }
    
    /// <summary>
    /// Skips the comic sequence
    /// </summary>
    private void SkipComic()
    {
        if (!comicActive || skipping)
            return;
            
        skipping = true;
        
        // Play skip sound if available
        if (skipSound != null)
        {
            AudioManager.Instance.PlaySFX(skipSound);
        }
        
        // Stop the current sequence
        if (panelSequenceCoroutine != null)
        {
            StopCoroutine(panelSequenceCoroutine);
        }
        
        // End the comic
        EndComic();
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (!comicActive)
            return;
                
        // Get all gamepad devices
        var gamepads = Gamepad.all.ToArray();
        
        // Direct check for button presses on the physical controllers
        bool p1ButtonPressed = false;
        bool p2ButtonPressed = false;
        
        if (gamepads.Length > 0)
        {
            // Assuming the first gamepad is Player 1's controller after assignment
            p1ButtonPressed = gamepads[0].buttonEast.isPressed; 
        }
        
        if (gamepads.Length > 1)
        {
            // Assuming the second gamepad is Player 2's controller after assignment
            p2ButtonPressed = gamepads[1].buttonEast.isPressed; 
        }
        
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Direct gamepad check - P1 Button: {p1ButtonPressed}, P2 Button: {p2ButtonPressed}");
            Debug.Log($"Gamepad count: {gamepads.Length}");
            for (int i = 0; i < gamepads.Length && i < 2; i++)
            {
                Debug.Log($"Gamepad {i}: {gamepads[i].name}, East: {gamepads[i].buttonEast.isPressed}, West: {gamepads[i].buttonWest.isPressed}");
            }
        }
        
        // Use direct button presses for skipping
        if (p1ButtonPressed && p2ButtonPressed)
        {
            skipProgress += Time.deltaTime;
            
            // Update fill amount
            skipProgressIndicator.fillAmount = Mathf.Clamp01(skipProgress / timeToSkip);
            
            // Skip if held long enough
            if (skipProgress >= timeToSkip)
            {
                SkipComic();
            }
        }
        else
        {
            // Reset progress if not both holding
            skipProgress = 0f;
            skipProgressIndicator.fillAmount = 0f;
        }
    }
    
    /// <summary>
    /// Add necessary components to each panel
    /// </summary>
    private void OnValidate()
    {
        // Ensure each panel has a CanvasGroup for fading
        foreach (GameObject panel in comicPanels)
        {
            if (panel != null && panel.GetComponent<CanvasGroup>() == null)
            {
                Debug.LogWarning($"Adding CanvasGroup to panel {panel.name} for fade transitions");
                panel.AddComponent<CanvasGroup>();
            }
        }
    }

}
