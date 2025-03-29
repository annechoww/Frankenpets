using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Fan : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    public GameObject bone;
    public GameObject rope;
    public GameObject dogFront;
    public GameObject catBack;
    public GameObject pet;
    public PlayerManager playerManager; 
    private ConfigurableJoint joint;

    private Rigidbody dogRb;
    private Rigidbody catRb;

    private Rigidbody boneRigidbody;
    private Rigidbody ropeRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boneRigidbody = bone.GetComponent<Rigidbody>();
        ropeRigidbody = rope.GetComponent<Rigidbody>();
        dogRb = dogFront.GetComponent<Rigidbody>();
        catRb = catBack.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if dog grabbed bone 
        joint = dogFront.GetComponent<ConfigurableJoint>();

        if (joint != null)
        {
            if (joint.connectedBody == boneRigidbody)
            {
                if (!stopwatch.IsRunning) stopwatch.Start();
                UnityEngine.Debug.Log("pet is rotating");

                // Pet should follow the bone's transform 
                dogRb.MovePosition(Vector3.Lerp(dogRb.position, bone.transform.position, Time.deltaTime / 10));

                dogRb.constraints = RigidbodyConstraints.FreezePositionY;
                catRb.constraints = RigidbodyConstraints.FreezePositionY;
            }
        } 
        else
        {
            UnityEngine.Debug.Log("let go; pet is rotating");
            ResetFanState();
        }

        // Condition to drop the ceiling fan 
        if (stopwatch.Elapsed.TotalSeconds >= 3.0f)
        {
            stopCeilingFan();

            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            boneRigidbody.constraints = RigidbodyConstraints.None;
            ropeRigidbody.constraints = RigidbodyConstraints.None;

            rope.GetComponent<MeshCollider>().enabled = true;
            Destroy(GetComponent<BoxCollider>());
            Destroy(this); // Remove this script from fan gameobject
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dog front"))
        {
            // Rotate ceiling fan
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dog front"))
        {
            stopCeilingFan();
        }
    }

    private void stopCeilingFan()
    {
        ResetFanState();
        transform.Rotate(Vector3.zero);
    }

    private void ResetFanState()
    {
        stopwatch.Stop();
        stopwatch.Reset();

        // Reset Rigidbody constraints and stop rotation
        dogRb.constraints = RigidbodyConstraints.None;
        catRb.constraints = RigidbodyConstraints.None;
        // dogRb.angularVelocity = Vector3.zero;
        // catRb.angularVelocity = Vector3.zero;
    }
}