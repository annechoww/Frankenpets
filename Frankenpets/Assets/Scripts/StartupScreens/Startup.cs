using UnityEngine;
using UnityEngine.UI;


public class Startup : MonoBehaviour
{
    [Header("Controller Input")]
    public ControllerAssignment controllerAssignment;
    public InputHandler player1Input;
    public InputHandler player2Input;

    [Header("UI Panels")]
    public GameObject splashPanel;
    public GameObject characterSelectionPanel;

    [Header("Character Selection UI - Player 1")]
    public Image player1CatIcon;
    public Image player1DogIcon;
    public GameObject player1LockedIndicator;
    public GameObject player1Text;

    [Header("Character Selection UI - Player 2")]
    public Image player2CatIcon;
    public Image player2DogIcon;
    public GameObject player2LockedIndicator;
    public GameObject player2Text;

    [Header("Player 1 Icon Outlines")]
    public Image player1CatOutline;
    public Image player1DogOutline;

    [Header("Player 2 Icon Outlines")]
    public Image player2CatOutline;
    public Image player2DogOutline;

    [Header("UI Feedback Colors")]
    public Color highlightColor = Color.yellow;
    public Color player1LockedColor = Color.red;
    public Color player2LockedColor = Color.blue;
    public Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color normalColor = Color.white;

    [Header("Ready UI")]
    public GameObject readyIndicator;

    [Header("Scene Transition")]
    public GameObject startText;
    public LevelLoader levelLoader;

    private bool splashDone = false;
    private bool inCharacterSelect = false;
    
    // Character selection state
    private bool player1SelectingDog = false; // false = selecting cat (default)
    private bool player2SelectingCat = false; // false = selecting dog (default)
    private bool player1Locked = false;
    private bool player2Locked = false;


    // Selection input cooldown (prevent rapid toggling)
    private float player1InputCooldown = 0f;
    private float player2InputCooldown = 0f;
    private const float INPUT_COOLDOWN_TIME = 0f;

    // Selection finalization state
    private bool selectionFinalized = false;
    private bool player1ChoseDog = false;

    void Start()
    {
        Screen.SetResolution(1920, 1080, true); // force their resolution to be 1920x1080

        // Show splash screen initially
        splashPanel.SetActive(true);
        characterSelectionPanel.SetActive(false);
        readyIndicator.SetActive(false);
        
        // Set up initial UI in splash screen
        if (controllerAssignment.IsKeyboard())
        {
            startText.transform.GetChild(0).gameObject.SetActive(true);
            startText.transform.GetChild(2).gameObject.SetActive(true);
        }
        else {
            startText.transform.GetChild(1).gameObject.SetActive(true);
            startText.transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    
    void Update()
    {

        // Update input cooldowns
        if (player1InputCooldown > 0)
            player1InputCooldown -= Time.deltaTime;
            
        if (player2InputCooldown > 0)
            player2InputCooldown -= Time.deltaTime;
        
        // Handle game state transitions
        if (!splashDone)
        {
            HandleSplashScreen();
        }
        else if (inCharacterSelect && !selectionFinalized)
        {
            HandleCharacterSelection();
        }
    }

    void HandleSplashScreen()
    {
        bool startPressed = false;
        
        if (controllerAssignment.IsKeyboard() && Input.GetKey(KeyCode.Space))
        {
            startPressed = true;
        } 
        else if (!controllerAssignment.IsKeyboard() && (player1Input.GetJumpJustPressed() || player2Input.GetJumpJustPressed()))
        {
            startPressed = true;
        }
        
        if (startPressed)
        {
            // Transition to character selection
            splashPanel.SetActive(false);
            characterSelectionPanel.SetActive(true);
            splashDone = true;
            inCharacterSelect = true;
            
            // Initialize character selection UI
            InitializeCharacterSelectionUI();
        }
    }

    void InitializeCharacterSelectionUI()
    {
        // Set default selections
        player1SelectingDog = false; // Player 1 starts with cat selected
        player2SelectingCat = false; // Player 2 starts with dog selected
        player1Locked = false;
        player2Locked = false;
        
        // Reset icons to normal state
        player1CatIcon.color = normalColor;
        player1DogIcon.color = normalColor;
        player2CatIcon.color = normalColor;
        player2DogIcon.color = normalColor;
        
        // Hide lock indicators
        player1LockedIndicator.SetActive(false);
        player2LockedIndicator.SetActive(false);
        readyIndicator.SetActive(false);
        
        // Set initial highlights
        UpdateCharacterSelectionUI();
    }

    void HandleCharacterSelection()
    {
        // Handle Player 1 selection
        HandlePlayer1Selection();
        
        // Handle Player 2 selection
        HandlePlayer2Selection();
        
        // Check if both players have confirmed and it's time to proceed
        CheckSelectionConfirmed();
    }

    void HandlePlayer1Selection()
    {
        if (!player1Locked)
        {
            // Handle directional input for selection
            if (player1InputCooldown <= 0)
            {
                float horizontalInput = player1Input.GetMoveInput().x;

                Debug.Log($"Player 1 horizontal input: {horizontalInput}");
                
                if (horizontalInput > 0.5f && !player1SelectingDog)
                {
                    // Moving right to select dog, if not already locked by Player 2
                    if (!(player2Locked && !player2SelectingCat))
                    {
                        player1SelectingDog = true;
                        player1InputCooldown = INPUT_COOLDOWN_TIME;
                        UpdateCharacterSelectionUI();
                    }
                }
                else if (horizontalInput < -0.5f && player1SelectingDog)
                {
                    // Moving left to select cat, if not already locked by Player 2
                    if (!(player2Locked && player2SelectingCat))
                    {
                        player1SelectingDog = false;
                        player1InputCooldown = INPUT_COOLDOWN_TIME;
                        UpdateCharacterSelectionUI();
                    }
                }
            }
        }

        if (player1Input.GetJumpJustPressed())
        {
            if (player1Locked)
            {
                // Only allow unlocking if both players aren't locked
                if (!player2Locked)
                {
                    player1Locked = false;
                    player1Text.SetActive(true);
                    player1LockedIndicator.SetActive(false);
                    UpdateCharacterSelectionUI();
                }
            }
            else
            {
                // Lock in current selection if it doesn't conflict with Player 2's locked selection
                if (!(player2Locked && ((player1SelectingDog && !player2SelectingCat) || (!player1SelectingDog && player2SelectingCat))))
                {
                    player1Locked = true;

                    if (player1SelectingDog && !player2Locked && !player2SelectingCat) {
                        // If Player 1 locked dog, automatically move Player 2 to cat
                        player2SelectingCat = true;
                    }
                    else if (!player1SelectingDog && !player2Locked && player2SelectingCat) {
                        // If Player 1 locked cat, automatically move Player 2 to dog
                        player2SelectingCat = false;
                    }

                    player1LockedIndicator.SetActive(true);
                    player1Text.SetActive(false);
                    UpdateCharacterSelectionUI();
                }
            }
        }
    }

    void HandlePlayer2Selection()
    {
        if (!player2Locked)
        {
            // Handle directional input for selection
            if (player2InputCooldown <= 0)
            {
                float horizontalInput = player2Input.GetMoveInput().x;
                print($"Player 2 horizontal input: {horizontalInput}");
                
                if (horizontalInput > 0.5f && player2SelectingCat)
                {
                    // Moving right to select dog, if not already locked by Player 1
                    if (!(player1Locked && player1SelectingDog))
                    {
                        player2SelectingCat = false;
                        player2InputCooldown = INPUT_COOLDOWN_TIME;
                        UpdateCharacterSelectionUI();
                    }
                }
                else if (horizontalInput < -0.5f && !player2SelectingCat)
                {
                    // Moving left to select cat, if not already locked by Player 1
                    if (!(player1Locked && !player1SelectingDog))
                    {
                        player2SelectingCat = true;
                        player2InputCooldown = INPUT_COOLDOWN_TIME;
                        UpdateCharacterSelectionUI();
                    }
                }
            }
        }
        
        // Handle jump button for locking/unlocking
        if (player2Input.GetJumpJustPressed())
        {
            if (player2Locked)
            {
                // Only allow unlocking if both players aren't locked
                if (!player1Locked)
                {
                    player2Locked = false;
                    player2LockedIndicator.SetActive(false);
                    player2Text.SetActive(true);
                    UpdateCharacterSelectionUI();
                }
            }
            else
            {
                // Lock in current selection if it doesn't conflict with Player 1's locked selection
                if (!(player1Locked && ((player2SelectingCat && !player1SelectingDog) || (!player2SelectingCat && player1SelectingDog))))
                {
                    player2Locked = true;

                    if (player2SelectingCat && !player1Locked && !player1SelectingDog) {
                        // If Player 2 locked cat, automatically move Player 1 to dog
                        player1SelectingDog = true;
                    }
                    else if (!player2SelectingCat && !player1Locked && player1SelectingDog) {
                        // If Player 2 locked dog, automatically move Player 1 to cat
                        player1SelectingDog = false;
                    }

                    player2LockedIndicator.SetActive(true);
                    player2Text.SetActive(false);
                    UpdateCharacterSelectionUI();
                }
            }
        }
    }

    void UpdateCharacterSelectionUI()
    {

        // Reset all outline states
        player1CatOutline.enabled = false;
        player1DogOutline.enabled = false;
        player2CatOutline.enabled = false;
        player2DogOutline.enabled = false;

        // Reset icon colors
        player1CatIcon.color = normalColor;
        player1DogIcon.color = normalColor;
        player2CatIcon.color = normalColor;
        player2DogIcon.color = normalColor;
        
        // ----- Player 1 Icons -----
        
        // Handle Player 1's current selection
        if (player1Locked)
        {
            // Locked selection
            if (player1SelectingDog)
            {
                player1DogOutline.enabled = true;
                player1DogOutline.color = player1LockedColor;

                // Make dog unavailable to Player 2
                player2DogIcon.color = unavailableColor;
                player1CatIcon.color = unavailableColor;
            }
            else
            {
                player1CatOutline.enabled = true;
                player1CatOutline.color = player1LockedColor;

                // Make cat unavailable to Player 2
                player2CatIcon.color = unavailableColor;
                player1DogIcon.color = unavailableColor;
            }
        }
        else
        {
            // Current highlighted selection
            if (player1SelectingDog)
            {
                player1DogOutline.enabled = true;
                player1DogOutline.color = highlightColor;
            }
            else
            {
                player1CatOutline.enabled = true;
                player1CatOutline.color = highlightColor;
            }
        }
        
        // ----- Player 2 Icons -----
        
        // Handle Player 2's current selection
        if (player2Locked)
        {
            // Locked selection
            if (player2SelectingCat)
            {
                player2CatOutline.enabled = true;
                player2CatOutline.color = player2LockedColor;

                // Make cat unavailable to Player 1
                player1CatIcon.color = unavailableColor;
                player2DogIcon.color = unavailableColor;
            }
            else
            {
                player2DogOutline.enabled = true;
                player2DogOutline.color = player2LockedColor;

                // Make dog unavailable to Player 1
                player1DogIcon.color = unavailableColor;
                player2CatIcon.color = unavailableColor;
            }
        }
        else
        {
            // Current highlighted selection
            if (player2SelectingCat)
            {
                player2CatOutline.enabled = true;
                player2CatOutline.color = highlightColor;
            }
            else
            {
                player2DogOutline.enabled = true;
                player2DogOutline.color = highlightColor;
            }
        }
        
        // Check if both players have valid selections locked in
        CheckSelectionReady();
    }

    void CheckSelectionReady()
    {
        // Both players have locked in and have different characters selected
        bool bothLockedWithValidSelections = player1Locked && player2Locked && 
            ((player1SelectingDog && !player2SelectingCat) || (!player1SelectingDog && player2SelectingCat));
            
        readyIndicator.SetActive(bothLockedWithValidSelections);
    }
    
    void CheckSelectionConfirmed()
    {
        // Both players have valid selections and are locked in
        bool bothLockedWithValidSelections = player1Locked && player2Locked && 
            ((player1SelectingDog && player2SelectingCat) || (!player1SelectingDog && !player2SelectingCat));
    
        print("Player1 Selecting Dog: " + player1SelectingDog + ", Player2 Selecting Cat: " + player2SelectingCat);
            
        print("bothLockedWithValidSelections: " + bothLockedWithValidSelections + ", selectionFinalized: " + selectionFinalized);
        if (bothLockedWithValidSelections && !selectionFinalized)
        {
            print("Both players have confirmed selections!");
            // Prevent further input processing
            selectionFinalized = true;
            
            // Record final choice for Player 1
            player1ChoseDog = player1SelectingDog;
            
            // Finalize controller assignments and proceed to next level
            FinalizeSelections();
        }
    }
    
    void FinalizeSelections()
    {
        Debug.Log($"Finalizing character selection: Player 1 chose {(player1ChoseDog ? "dog" : "cat")}");
        
        // Call controller assignment with the appropriate parameter
        // If player 1 selected dog, we need to swap controllers so that player 1 is always cat internally
        controllerAssignment.FinalizeAssignment(!player1ChoseDog);
        
        // Short delay before loading next level (for visual feedback)
        print("Loading next level...");
        Invoke("LoadNextLevel", 1.0f);
    }
    
    void LoadNextLevel()
    {
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