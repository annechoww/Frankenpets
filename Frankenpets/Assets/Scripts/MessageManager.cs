using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public GameObject reconnectFail;
    public GameObject cameraIndicator;
    public GameObject switchFail;
    public GameObject buttonHoldFail;
    public GameObject pressEnterToHideTutorial;
    public GameObject pressEnterToContinue;
    public GameObject custom;

    public GameObject P1WantsToSwitch;
    public GameObject P2WantsToSwitch;
    public GameObject P1WantsToReconnect;
    public GameObject P2WantsToReconnect;

    private RectTransform P1WantsToSwitchRect;
    private RectTransform P2WantsToSwitchRect;
    private RectTransform P1WantsToReconnectRect;
    private RectTransform P2WantsToReconnectRect;

    private Coroutine P1WantsToSwitchCoroutine;
    private Coroutine P2WantsToSwitchCoroutine;
    private Coroutine P1WantsToReconnectCoroutine;
    private Coroutine P2WantsToReconnectCoroutine;

    private ControllerAssignment controllerAssignment;

    void Awake()
    {
        controllerAssignment = FindObjectOfType<ControllerAssignment>();
        UnityEngine.Debug.Log(controllerAssignment);
    }

    void Start()
    {
        bool isKeyboard = controllerAssignment.IsKeyboard();
        pressEnterToHideTutorial.transform.GetChild(0).gameObject.SetActive(isKeyboard);
        pressEnterToHideTutorial.transform.GetChild(1).gameObject.SetActive(!isKeyboard);

        P1WantsToSwitchRect = P1WantsToSwitch.GetComponent<RectTransform>();
        P2WantsToSwitchRect = P2WantsToSwitch.GetComponent<RectTransform>();
        P1WantsToReconnectRect = P1WantsToReconnect.GetComponent<RectTransform>();
        P2WantsToReconnectRect = P2WantsToReconnect.GetComponent<RectTransform>();
    }

    public void reconnectFailMessage()
    {
        StartCoroutine(disableAfterSeconds(reconnectFail, 3.0f));
    }

    public void cameraIndicatorMessage()
    {
        StartCoroutine(disableAfterSeconds(cameraIndicator, 3.0f));
    }

    public void buttonHoldFailMessage()
    {
        StartCoroutine(disableAfterSeconds(buttonHoldFail, 3.0f));
    }

    public void switchFailMessageActivate()
    {
        switchFail.SetActive(true);
    }

    public void switchFailMessageDeactivate()
    {
        switchFail.SetActive(false);
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

    private Coroutine P1SwitchCoroutine;
    private Coroutine P2SwitchCoroutine;
    private Coroutine P1ReconnectCoroutine;
    private Coroutine P2ReconnectCoroutine;

    public void stopSwitchReconnectCoroutines()
    {
        if (P1SwitchCoroutine != null) StopCoroutine(P1SwitchCoroutine);
        if (P2SwitchCoroutine != null) StopCoroutine(P2SwitchCoroutine);
        if (P1ReconnectCoroutine != null) StopCoroutine(P1ReconnectCoroutine);
        if (P2ReconnectCoroutine != null) StopCoroutine(P2ReconnectCoroutine);
    }

    public void startP1WantsToSwitch()
    {
        P1WantsToSwitch.SetActive(true);
        // StartCoroutine(disableAfterSeconds(P1WantsToSwitch, 1.5f));
        stopSwitchReconnectCoroutines();
        P1SwitchCoroutine = StartCoroutine(SlideUpEffect(P1WantsToSwitchRect));
    }
    public void endP1WantsToSwitch()
    {
        
        stopSwitchReconnectCoroutines();
        P1SwitchCoroutine = StartCoroutine(HideEffect(P1WantsToSwitchRect));
        P1WantsToSwitch.SetActive(false);
    }
    public void startP2WantsToSwitch()
    {
        P2WantsToSwitch.SetActive(true);
        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(SlideUpEffect(P2WantsToSwitchRect));
    }
    public void endP2WantsToSwitch()
    {
        
        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(HideEffect(P2WantsToSwitchRect));
        P2WantsToSwitch.SetActive(false);
    }
    public void startP1WantsToReconnect()
    {
        P1WantsToReconnect.SetActive(true);
        // StartCoroutine(disableAfterSeconds(P1WantsToReconnect, 1.5f));

        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(SlideUpEffect(P1WantsToReconnectRect));
    }
    public void endP1WantsToReconnect()
    {
        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(HideEffect(P1WantsToReconnectRect));
        P1WantsToReconnect.SetActive(false);
    }
    public void startP2WantsToReconnect()
    {
        P2WantsToReconnect.SetActive(true);
        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(SlideUpEffect(P2WantsToReconnectRect));
        
    }
    public void endP2WantsToReconnect()
    {
        stopSwitchReconnectCoroutines();
        P2SwitchCoroutine = StartCoroutine(HideEffect(P2WantsToReconnectRect));
        P2WantsToReconnect.SetActive(false);
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

    private Coroutine bounceCoroutine;
    private IEnumerator SlideUpEffect(RectTransform rectTransform)
    {
        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -405);
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // Stop any ongoing bounce before starting a new one
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }

        bounceCoroutine = StartCoroutine(BounceAfterDelay(rectTransform, 1.5f));
    }

    private IEnumerator HideEffect(RectTransform rectTransform)
    {
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }
        
        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -1086);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    private IEnumerator BounceAfterDelay(RectTransform rectTransform, float delay)
    {
        yield return new WaitForSeconds(delay); 

        Vector3 initialPosition = rectTransform.anchoredPosition;

        while (true)
        {
            float newY = initialPosition.y + Mathf.Sin(Time.time * 4.0f) * 20.0f; // speed, amplitude
            rectTransform.anchoredPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
            yield return null; // Wait for next frame
        }
    }
}
