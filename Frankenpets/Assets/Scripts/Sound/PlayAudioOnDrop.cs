using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayAudioOnDrop : MonoBehaviour
{
    [Header("References")]
    public float force = 1f;
    public AudioClip audioClip;

    private Stopwatch stopwatch = new Stopwatch();
    private bool shouldMute = true;
    private float muteForSeconds = 5.0f;

    void Start()
    {
        stopwatch.Start();
    }

    void Update()
    {
        if (stopwatch.Elapsed.TotalSeconds > muteForSeconds) 
        {
            shouldMute = false;
            stopwatch.Stop();
        }
    }

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
        if ((audioClip != null) && !shouldMute)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }
}
