using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint;
    
    [Header("Boundary Settings")]
    public Vector3 minBoundary = new Vector3(-10f, -4f, -10f);
    public Vector3 maxBoundary = new Vector3(10f, 10f, 10f);
    
    [Header("Debug")]
    public bool showBoundaryGizmos = true;
    public Color boundaryGizmoColor = new Color(1f, 0f, 0f, 0.3f);
    
    private PlayerManager playerManager; 
    private Player P1;
    private Player P2;
    private bool playersInitialized = false;

    void Start()
    {
        // Initialize players with a slight delay to ensure PlayerManager is available
        Invoke("InitializePlayers", 0.1f);
    }

    void Update()
    {
        if (!playersInitialized)
            return;
            
        // Check if players are out of bounds
        CheckAndRespawnIfNeeded(P1);
        CheckAndRespawnIfNeeded(P2);
    }
    
    void InitializePlayers()
    {
        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager != null)
        {
            P1 = playerManager.P1;
            P2 = playerManager.P2;
            
            if (P1 != null && P2 != null)
            {
                playersInitialized = true;
                Debug.Log("Players initialized successfully.");
                Debug.Log("P1 Species: " + P1.Species);
                Debug.Log("P2 Species: " + P2.Species);
            }
            else
            {
                Debug.LogWarning("Player references in PlayerManager are null.");
            }
        }
        else
        {
            Debug.LogError("PlayerManager not found in the scene!");
        }
    }
    
    void CheckAndRespawnIfNeeded(Player player)
    {
        if (player == null || player.Half == null)
            return;
            
        if (IsOutOfBounds(player.Half.transform.position))
        {
            Respawn(player);
        }
    }

    bool IsOutOfBounds(Vector3 position)
    {
        return position.x < minBoundary.x || position.x > maxBoundary.x ||
               position.y < minBoundary.y || position.y > maxBoundary.y ||
               position.z < minBoundary.z || position.z > maxBoundary.z;
    }

    public void Respawn(Player player)
    {
        if (respawnPoint != null)
        {
            player.Half.transform.position = respawnPoint.position;
            Debug.Log($"Respawned player ({player.Species}) at designated respawn point.");
        }
        else
        {
            Debug.LogWarning("Respawn point is not set!");
        }
    }
    
    // Visualize the boundary box in the editor
    void OnDrawGizmos()
    {
        if (!showBoundaryGizmos)
            return;
            
        Gizmos.color = boundaryGizmoColor;
        
        // Calculate the center and size of the boundary box
        Vector3 center = (minBoundary + maxBoundary) * 0.5f;
        Vector3 size = maxBoundary - minBoundary;
        
        Gizmos.DrawCube(center, size);
    }
}