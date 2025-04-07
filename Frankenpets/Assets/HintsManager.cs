using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
    HintsManager does not check whether all tasks in the current level are done.
    It also does not check if the player is "attempting" a task.

    HintsManager.Instance.ShouldShowHint() returns true if the player is inside the box collider trigger(s).
*/


public class HintsManager : MonoBehaviour
{
    public static HintsManager Instance { get; private set; }

    private bool startStopwatch = false;
    public bool showHint = false;
    private float stopwatchElapsedTime = 0f;
    public float stopwatchInterval = 60f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(DelayStart(20f));
    }

    private IEnumerator DelayStart(float x) // Delay the start of the script for x seconds
    {
        this.enabled = false;
        yield return new WaitForSeconds(x);
        this.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startStopwatch)
        {
            stopwatchElapsedTime += Time.deltaTime;
            showHint = false;

            if (stopwatchElapsedTime >= stopwatchInterval)
            {
                UnityEngine.Debug.Log($"Stopwatch: {stopwatchInterval} seconds have passed.");
                // StartCoroutine(ShowMessage("HINT: <u>Locate</u> to-do list tasks.", "glow"));
                showHint = true;
                stopwatchElapsedTime = 0f;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.CompareTag("cat front") && other.gameObject.CompareTag("dog back")) || 
            (other.gameObject.CompareTag("cat back") && other.gameObject.CompareTag("dog front")))
        {
            startStopwatch = false;
            showHint = false;
        } else
        {
            startStopwatch = true;
        }
    }

    public bool ShouldShowHint()
    {
        return showHint;
    }
}
