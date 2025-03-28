using UnityEngine;
using UnityEngine.UI;

public class JugShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenJug;
    public float shatterForce = 1f;
    public AudioClip shatterSound;
    private bool isShattered = false;

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;

    void OnCollisionEnter(Collision collision)
    {

        // Check if the jug hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterJug();
        }
    }
    

    void ShatterJug()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
            AudioManager.Instance.PlaySFX(shatterSound);
        }

        // Instantiate the broken jug at the jug's position and rotation
        Instantiate(brokenJug, transform.position, transform.rotation);

        // Destroy the intact jug after shattering
        Destroy(gameObject);
    }

}
