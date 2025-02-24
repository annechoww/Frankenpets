using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public GameObject reconnectFail;
    public GameObject cameraIndicator;
    public GameObject switchFail;

    public void reconnectFailMessage()
    {
        StartCoroutine(disableAfterSeconds(reconnectFail, 3.0f));
    }

    public void cameraIndicatorMessage()
    {
        StartCoroutine(disableAfterSeconds(cameraIndicator, 4.0f));
    }

    public void switchFailMessage()
    {
        StartCoroutine(disableAfterSeconds(switchFail, 3.0f));
    }

    // Coroutines
    private IEnumerator disableAfterSeconds(GameObject obj, float seconds)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        obj.SetActive(true);

        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < seconds) 
        {
            yield return null;
        }

        // stopwatch.Reset();

        obj.SetActive(false);
    }
}
