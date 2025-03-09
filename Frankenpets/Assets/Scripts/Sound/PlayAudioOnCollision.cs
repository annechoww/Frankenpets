using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayAudioOnCollision : MonoBehaviour
{
    [Header("References")]
    public float force = 0.05f;
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
        // Debug.Log("collision force: " + collision.relativeVelocity.magnitude);
        // Check the force in which pet hits object
        if (!collision.transform.CompareTag("Floor") && (collision.relativeVelocity.magnitude > force))
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
