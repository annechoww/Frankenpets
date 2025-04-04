using UnityEngine;
using UnityEngine.UI;

public class JoystickVisualizer : MonoBehaviour
{

    [Header("Player References")]
    public PlayerManager playerManager;
    
    [Header("UI Elements")]
    public RectTransform player1Background;
    public RectTransform player1Nub;
    public RectTransform player2Background;
    public RectTransform player2Nub;
    
    [Header("Visualization Settings")]
    public float maxNubDistance = 20f; // Maximum distance the nub can move from center
    public float deadzone = 0.1f; // Minimum input magnitude to show movement
    public bool hideWhenInactive = true; // Whether to hide nubs when no input
    public float inactiveAlpha = 0.3f; // Alpha when inactive
    public float activeAlpha = 1.0f; // Alpha when active
    
    private Image p1NubImage;
    private Image p2NubImage;

    private Vector2 player1InitialNubPosition;
    private Vector2 player2InitialNubPosition;

    void Start()
    {
        // Cache references to the nub images
        if (player1Nub) {
            p1NubImage = player1Nub.GetComponent<Image>();
            player1InitialNubPosition = player1Nub.anchoredPosition;
        }
        
        if (player2Nub) {
            p2NubImage = player2Nub.GetComponent<Image>();
            player2InitialNubPosition = player2Nub.anchoredPosition;
        }
        
        // Set initial transparency if hideWhenInactive is true
        if (hideWhenInactive)
        {
            SetNubAlpha(p1NubImage, inactiveAlpha);
            SetNubAlpha(p2NubImage, inactiveAlpha);
        }
    }

    void UpdateJoystickMiniature(RectTransform nub, Image nubImage, Vector2 input, Vector2 initialPosition)
    {
        if (nub == null) return;
        
        bool hasSignificantInput = input.magnitude > deadzone;
        
        // Calculate position: initial position + input offset
        Vector2 targetPosition = initialPosition;
        if (hasSignificantInput) {
            targetPosition += input * maxNubDistance;
        }
        
        // Apply the position
        nub.anchoredPosition = targetPosition;
        
        // Handle visibility based on input
        if (hideWhenInactive && nubImage != null)
        {
            float targetAlpha = hasSignificantInput ? activeAlpha : inactiveAlpha;
            SetNubAlpha(nubImage, targetAlpha);
        }
    }
    
    void Update()
    {
        if (playerManager == null) return;
        
        // Get joystick inputs from both players
        Vector2 p1Input = playerManager.player1Input.GetMoveInput();
        Vector2 p2Input = playerManager.player2Input.GetMoveInput();
        
        // Update the joystick visualizations
        UpdateJoystickMiniature(player1Nub, p1NubImage, p1Input, player1InitialNubPosition);
        UpdateJoystickMiniature(player2Nub, p2NubImage, p2Input, player2InitialNubPosition);
    }
    
    void SetNubAlpha(Image image, float alpha)
    {
        if (image == null) return;
        
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}

