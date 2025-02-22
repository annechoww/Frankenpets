using UnityEngine;

/**
This class handles all actions that do not require collision detection 
    - Jump
    - Make Noise
    - Stand on Hind Legs
    - Charged Jump

Actions NOT implemented in this script:
    - Climb
    - Grab
    - Tail Usage
    - Front Paw Usage
*/
public class PlayerController : MonoBehaviour
{
    [Header("Jumping Variables")]
    public float jumpForce = 15f;
    public float jumpCooldown = 0.5f;
    public float chargedJumpForce = 25f;
    public float chargedJumpCooldown = 0.8f;
    private float lastJumpTime = -10f;

    private PlayerManager playerManager; 
    private Player P1;
    private Player P2;
    private GameObject frontHalf;
    private GameObject backHalf;
    private Rigidbody frontRb;
    private Rigidbody backRb;

    private void Start()
    {
        // Find the PlayerManager in the scene (assuming it's attached to a GameObject)
        playerManager = FindObjectOfType<PlayerManager>();

        // Now you can access Player details like P1.Species
        if (playerManager != null)
        {
            Debug.Log("P1 Species: " + playerManager.P1.Species);
            Debug.Log("P2 Species: " + playerManager.P2.Species);

            P1 = playerManager.P1;
            P2 = playerManager.P2;

            frontHalf = playerManager.getFrontHalf();
            backHalf = playerManager.getBackHalf();

            frontRb = frontHalf.GetComponent<Rigidbody>();
            backRb = backHalf.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("Error in fetching playerManager");
        }

    }

    private void Update()
    {
        runJumpLogic();
    }


////////////////////////////////////// Jump Logic /////////////////////////////////////
    private void runJumpLogic()
    {
        // Basic Jump
        if (((Input.GetKey(KeyCode.Z) && !P1.IsFront)) || ((Input.GetKey(KeyCode.M)) && !P2.IsFront))
        {
            tryStartJump(jumpForce, jumpCooldown);
        }
        // Charged Jump
        else if ((Input.GetKey(KeyCode.V) && !P1.IsFront && P1.Species == "cat") || 
                (Input.GetKey(KeyCode.Slash) && !P2.IsFront && P2.Species == "dog"))
        {
            tryStartJump(chargedJumpForce, chargedJumpCooldown);
        }
    }

    private void tryStartJump(float jumpForce, float jumpCooldown)
    {
        if (Time.time - lastJumpTime > jumpCooldown)
        {
            frontRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            backRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
        }
    }


}