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

    // Tags to detect
    private string catFrontTag = "cat front";
    private string catBackTag = "cat back";
    private string dogBackTag = "dog back";
    private string dogFrontTag = "dog front";


    private HashSet<GameObject> halvesInTrigger = new HashSet<GameObject>();

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

    void Update()
    {
        // Check if both specific tagged objects are inside
        bool frontTagPresent = false;
        bool backTagPresent = false;

        foreach (var obj in halvesInTrigger)
        {
            if (obj != null)
            {
                if (obj.CompareTag(catFrontTag) || obj.CompareTag(dogFrontTag)) frontTagPresent = true;
                if (obj.CompareTag(catBackTag) || obj.CompareTag(dogBackTag)) backTagPresent = true;
            }
        }

        if (frontTagPresent && backTagPresent)
        {
            UnityEngine.Debug.Log("Trigger: Both halves are inside the no-hint trigger.");
            startStopwatch = false;
            showHint = false;
        }
        else
        {
            startStopwatch = true;
        }

        if (startStopwatch)
        {
            stopwatchElapsedTime += Time.deltaTime;
            showHint = false;

            if (stopwatchElapsedTime >= stopwatchInterval)
            {
                UnityEngine.Debug.Log($"Stopwatch: {stopwatchInterval} seconds have passed.");
                showHint = true;
                stopwatchElapsedTime = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        halvesInTrigger.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        halvesInTrigger.Remove(other.gameObject);
    }

    public bool ShouldShowHint()
    {
        return showHint;
    }
}
