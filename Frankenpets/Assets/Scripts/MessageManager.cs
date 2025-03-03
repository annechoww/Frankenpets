using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public GameObject reconnectFail;
    public GameObject cameraIndicator;
    public GameObject switchFail;
    public GameObject switchSuccess;
    public GameObject fellOver;
    public GameObject buttonHoldFail;
    public GameObject pressEnterToHideTutorial;
    public GameObject pressEnterToContinue;

    public GameObject custom;

    public void reconnectFailMessage()
    {
        StartCoroutine(disableAfterSeconds(reconnectFail, 3.0f));
    }

    public void cameraIndicatorMessage()
    {
        StartCoroutine(disableAfterSeconds(cameraIndicator, 3.0f));
    }

    public void switchFailMessage()
    {
        StartCoroutine(disableAfterSeconds(switchFail, 3.0f));
    }

    public void switchSuccessMessage()
    {
        StartCoroutine(disableAfterSeconds(switchSuccess, 3.0f));
    }

    public void fellOverMessage()
    {
        StartCoroutine(disableAfterSeconds(fellOver, 3.0f));
    }

    public void buttonHoldFailMessage()
    {
        StartCoroutine(disableAfterSeconds(buttonHoldFail, 3.0f));
    }

    public void startPressEnterToHideTutorial()
    {
        pressEnterToHideTutorial.SetActive(true);
    }

    public void cancelPressEnterToHideTutorial()
    {
        pressEnterToHideTutorial.SetActive(false);
    }

    public void startPressEnterToContinue()
    {
        pressEnterToContinue.SetActive(true);
    }

    public void cancelPressEnterToContinue()
    {
        pressEnterToContinue.SetActive(false);
    }

    public void autoCustomMessage(string message, float seconds = 3.0f)
    {
        custom.GetComponent<TextMeshProUGUI>().text = message;
        StartCoroutine(disableAfterSeconds(custom, seconds));
    }

    public void startCustomMessage(string message)
    {
        custom.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void cancelCustomMessage(string message)
    {
        custom.GetComponent<TextMeshProUGUI>().text = "";
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
