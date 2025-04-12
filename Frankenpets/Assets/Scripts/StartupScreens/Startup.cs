using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Startup : MonoBehaviour
{
    [Header("Controller Input")]
    public ControllerAssignment controllerAssignment;
    public InputHandler player1Input; // Cat (Player 1 controller)
    public InputHandler player2Input; // Dog (Player 2 controller)

    [Header("UI Panels")]
    public GameObject splashPanel;

    [Header("Scene Transition")]
    public GameObject startText;
    public LevelLoader levelLoader;

    [Header("Intro Comic")]
    public ComicManager comicManager;
    public GameObject comicPanel;
    public GameObject comicCanvas;
    private bool comicPlayed = false;

    private bool splashDone = false;

    void Start()
    {
        Screen.SetResolution(1920, 1080, true); // force their resolution to be 1920x1080

        // Show splash screen initially
        splashPanel.SetActive(true);
        
        // Set up initial UI in splash screen
        if (InputHelper.IsKeyboardActive() || controllerAssignment.IsKeyboard())
        {
            startText.transform.GetChild(0).gameObject.SetActive(true); // Keyboard prompt
            startText.transform.GetChild(2).gameObject.SetActive(true); // Additional keyboard info
        }
        else {
            startText.transform.GetChild(1).gameObject.SetActive(true); // Controller prompt
            startText.transform.GetChild(3).gameObject.SetActive(true); // Additional controller info
        }

        // Make sure comic elements are disabled initially
        if (comicCanvas != null)
        {
            comicCanvas.SetActive(false);
        }
        if (comicPanel != null)
        {
            comicPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Handle splash screen transition
        if (!splashDone)
        {
            HandleSplashScreen();
        }
    }

    void HandleSplashScreen()
    {
        bool startPressed = false;
    
        if (controllerAssignment.IsKeyboard() && Input.GetKey(KeyCode.Space))
        {
            startPressed = true;
        } 
        else if (!controllerAssignment.IsKeyboard() && (player1Input.GetJumpPressed() || player2Input.GetJumpPressed()))
        {
            startPressed = true;
        }
        
        if (startPressed)
        {
            // Transition directly to comic/game instead of character selection
            TransitionToComic();
            splashPanel.SetActive(false);
            splashDone = true;
        }
    }

    void TransitionToComic()
    {
        Debug.Log("Transitioning to comic");
        
        // Activate comic canvas and panel
        if (comicCanvas != null)
        {
            comicCanvas.SetActive(true);
        }
        if (comicPanel != null)
        {
            comicPanel.SetActive(true);
        }

        // Start the comic sequence with the input handlers
        // Player 1 (with Player 1 light) is always Cat
        // Player 2 (with Player 2 light) is always Dog
        comicManager.StartComic(player1Input, player2Input, this);
    }

    // Add method for comic to call when finished
    public void OnComicComplete()
    {
        
        //load the next level
        if (levelLoader != null)
        {
            levelLoader.LoadNextLevel();
        }
        else
        {
            Debug.LogError("Level Loader not assigned!");
        }
    }
}