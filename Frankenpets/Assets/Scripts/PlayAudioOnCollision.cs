using UnityEngine;

public class PlayAudioOnCollision : MonoBehaviour
{
    [Header("References")]
    public float force = 0.1f;
    public AudioClip audioClip;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision force: " + collision.relativeVelocity.magnitude);
        // Check the force in which pet hits object
        if (!collision.transform.CompareTag("Floor") && (collision.relativeVelocity.magnitude > force))
        {
            playSound();
        }
    }

    void playSound()
    {   
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }
}
