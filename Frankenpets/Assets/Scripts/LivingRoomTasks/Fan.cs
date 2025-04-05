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
    private bool fanHasFallen = false;

    public AudioClip fanFallingClip;

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

        if (joint != null && !fanHasFallen)
        {
            if (joint.connectedBody == boneRigidbody)
            {
                if (!stopwatch.IsRunning) stopwatch.Start();

                // Pet should follow the bone's transform 
                dogRb.MovePosition(Vector3.Lerp(dogRb.position, bone.transform.position, Time.deltaTime / 10));

                dogRb.constraints = RigidbodyConstraints.FreezePositionY;
                catRb.constraints = RigidbodyConstraints.FreezePositionY;
            }
        } 
        else
        {
            ResetFanState();
        }

        // Condition to drop the ceiling fan 
        if (stopwatch.Elapsed.TotalSeconds >= 3.0f)
        {
            fanHasFallen = true;

            stopCeilingFan();

            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            boneRigidbody.constraints = RigidbodyConstraints.None;
            ropeRigidbody.constraints = RigidbodyConstraints.None;

            rope.GetComponent<MeshCollider>().enabled = true;
            Destroy(GetComponent<BoxCollider>());

            StartCoroutine(PlayFanFallingNoise());
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

        // Reset pet Rigidbody constraints
        dogRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        dogRb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        catRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        // DON'T FREEZE CAT BACK X Z ROTATIONS OR ANY BACK ROTATIIONS FOR THAT MATTER
        // catRb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }

    private IEnumerator PlayFanFallingNoise()
    {
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.PlaySFX(fanFallingClip);

        yield return new WaitForSeconds(3.0f);

        Destroy(this); // Remove this script from fan gameobject
    }
}
