using UnityEngine;

public class AtticDoorTrigger : MonoBehaviour
{
    public LevelLoader levelLoader;
    private float triggerEnterTime = 0.0f;
    private bool timerStarted = false;

    private void OnTriggerStay(Collider other)
    {
        if (!timerStarted)
        {
            triggerEnterTime = Time.time;
            timerStarted = true;
        }

        if (Time.time - triggerEnterTime >= 1.0f && (other.CompareTag("dog front") || other.CompareTag("cat front")))
        {
            if (levelLoader != null)
            {
                levelLoader.LoadNextLevel();
            }

            timerStarted = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dog front") || other.CompareTag("cat front"))
        {
            timerStarted = false;
        }
    }
}
