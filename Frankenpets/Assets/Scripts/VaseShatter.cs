using UnityEngine;

public class VaseShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenVase;
    public float shatterForce = 1f;
    public AudioClip shatterSound;
    private bool isShattered = false;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the vase hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterVase();
        }
    }

    void ShatterVase()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
        }

        // Instantiate the broken vase at the vase's position and rotation
        Instantiate(brokenVase, transform.position, transform.rotation);

        // Destroy the intact vase after shattering
        Destroy(gameObject);
    }
}
