using UnityEngine;

public class PlayAudioOnDrop : MonoBehaviour
{
    [Header("References")]
    public GameObject obj;
    public float force = 1f;
    public AudioClip audioClip;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the vase hits the ground with enough force
        if (collision.relativeVelocity.magnitude > force)
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
