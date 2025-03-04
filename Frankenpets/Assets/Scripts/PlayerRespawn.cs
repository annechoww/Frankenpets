using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float respawnValue = -4.0f;

    private PlayerManager playerManager; 
    private Player P1;
    private Player P2;

    void Start()
    {
        Invoke("getPlayers", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (P1.Half.transform.position.y < respawnValue)
        {
            Respawn(P1);
        }
        
        if (P2.Half.transform.position.y < respawnValue)
        {
            Respawn(P2);
        }
    }

    void getPlayers()
    {
        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();

        // Now you can access Player details like P1.Species
        if (playerManager != null)
        {
            Debug.Log("P1 Species: " + playerManager.P1.Species);
            Debug.Log("P2 Species: " + playerManager.P2.Species);

            P1 = playerManager.P1;
            P2 = playerManager.P2;
        }
    }

    void Respawn(Player player)
    {
        player.Half.transform.position = respawnPoint.position;
    }
}
