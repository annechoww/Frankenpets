using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float riseSpeed = 0.5f;
    public float balloonDestroyHeight = 15.0f;
    public GameObject bone;
    public GameObject balloon;
    public GameObject dogFront;
    public GameObject catBack;

    private Rigidbody boneRb;
    private Rigidbody balloonRb;
    private Rigidbody dogRb;
    private Rigidbody catRb;
    private ConfigurableJoint joint;
    private FixedJoint petJoint;
    private PlayerManager playerManager;
    private bool balloonStartedRising = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        balloonRb = balloon.GetComponent<Rigidbody>();
        boneRb = bone.GetComponent<Rigidbody>();
        dogRb = dogFront.GetComponent<Rigidbody>();
        catRb = catBack.GetComponent<Rigidbody>();
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("PlayerManager not found in the scene.");
        }
        balloonRb.useGravity = false;
    }

    void FixedUpdate()
    {
        petJoint = playerManager.getJoint();
        // Check if dog grabbed bone 
        joint = dogFront.GetComponent<ConfigurableJoint>();

        if (joint != null && joint.connectedBody == boneRb && !balloonStartedRising)
        {
            // Balloon rises
            balloonStartedRising = true;
            balloonRb.MovePosition(balloonRb.position + Vector3.up * riseSpeed * Time.fixedDeltaTime);
            boneRb.constraints = RigidbodyConstraints.None;

            // Pet and bone should float with balloon: disable gravity and freeze sideways rotations
            boneRb.transform.SetParent(balloonRb.transform, true);
            dogRb.isKinematic = true;
            dogRb.useGravity = false;
            boneRb.useGravity = false;
            dogRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            // Pet and bone should follow the balloon's transform 
            // dogRb.MovePosition(Vector3.Lerp(dogRb.position, bone.transform.position, Time.deltaTime / 10f)); // cat back will follow dog front if they're connected
            // boneRb.MovePosition(Vector3.Lerp(boneRb.position, balloonRb.position, Time.deltaTime * 2f));

            if (petJoint)
            {
                catRb.useGravity = false;
                // catRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }

            // Player respawn is handled in PlayerRespawn.cs
        }
        else if (balloonStartedRising && joint == null)
        {
            // Reset pet Rigidbody constraints
            dogRb.isKinematic = false;
            dogRb.useGravity = true;
            boneRb.useGravity = true;
            boneRb.constraints = RigidbodyConstraints.None;
            dogRb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
            boneRb.transform.SetParent(null);

            // Freeze the rigidbody’s rotation around the X-axis and Z-axis
            dogRb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


            if (petJoint)
            {
                catRb.useGravity = true;
                // catRb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
            }
        }

        // Balloon continues to rise if it has started rising
        if (balloonRb && balloonStartedRising)
        {
            balloonRb.MovePosition(balloonRb.position + Vector3.up * riseSpeed * Time.fixedDeltaTime);
        }

        // Destroy balloon if it goes too high
        if (balloon.transform.position.y >= balloonDestroyHeight)
        {
            Destroy(balloon);
        }
    }
}
