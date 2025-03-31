using UnityEngine;
using UnityEngine.UI;

public class MirrorShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenMirror;
    public float shatterForce = 1f;
    public AudioClip shatterSound;
    private bool isShattered = false;
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if the mirror hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterMirror();
        }
    }
    

    void ShatterMirror()
    {
        isShattered = true;

        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
        }

        // Instantiate the broken mirror at the mirror's position and rotation
        Instantiate(brokenMirror, transform.position, transform.rotation);

        // Destroy the intact mirror after shattering
        Destroy(gameObject);
    }
}
