using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayAudioOnDrop : MonoBehaviour
{
    [Header("References")]
    public float forceThreshold = 1f;  // Minimum force needed to trigger sound
    public AudioClip dropSoundClip;

    private bool isMuted = true;
    private float muteDuration = 5.0f;

    private void Start()
    {
        StartCoroutine(UnmuteAfterDelay());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object hit the ground with enough force
        if (collision.relativeVelocity.magnitude > forceThreshold && !isMuted)
        {
            PlaySoundAtPosition();
        }
    }

    private void PlaySoundAtPosition()
    {
        if (dropSoundClip != null)
        {
            // Send the drop sound to AudioManager for volume control
            AudioManager.Instance.Play3DSFX(dropSoundClip, transform.position);
        }
    }

    private IEnumerator UnmuteAfterDelay()
    {
        yield return new WaitForSeconds(muteDuration);
        isMuted = false;
    }
}
