using UnityEngine;

public class YarnBall : MonoBehaviour
{
    public PlayerManager playerManager; 
    public PlayerActions playerActions; 
    public GameObject dogFront;
    private ConfigurableJoint joint;
    private Rigidbody yarnRigidBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yarnRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        joint = dogFront.GetComponent<ConfigurableJoint>();

        if (joint != null)
        {
            // if (playerManager.getJoint() == null && joint.connectedBody == yarnRigidBody)
            // {
            //     playerActions.tryChargedJump(playerActions.chargedJumpForce, playerActions.chargedJumpCooldown);
            // }
            if (joint.connectedBody == yarnRigidBody)
            {
                if (playerManager.getJoint() != null) playerManager.DestroyJoint();
          

                playerActions.tryChargedJump(playerActions.chargedJumpForce, playerActions.chargedJumpCooldown);
            }
        }
    }
}
