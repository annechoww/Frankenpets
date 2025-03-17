using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CatBoxes : MonoBehaviour
{
    private bool coroutineIsDone = false;
    private RectTransform messageRectTransform;
    private TutorialText tutorialText;
    public GameObject message;
    public AudioClip woofSound;

    private Coroutine notMyBoxCoroutine;
    private Coroutine bounceCoroutine;

    void Start()
    {
        messageRectTransform = message.GetComponent<RectTransform>();
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("dog back") && notMyBoxCoroutine == null && tutorialText.getCurrTutorialStage() == TutorialText.tutScatterBoxes)
        {      
            UnityEngine.Debug.Log("dog touched cat boxes");      
            AudioSource.PlayClipAtPoint(woofSound, transform.position);
            notMyBoxCoroutine = StartCoroutine(StartMessage(messageRectTransform));
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("dog back"))
        {   
            if (coroutineIsDone)
            {
                coroutineIsDone = false;
                StopCoroutine(notMyBoxCoroutine);
                notMyBoxCoroutine = null;
            }         
        }
    }

    private IEnumerator StartMessage(RectTransform rectTransform)
    {
        message.SetActive(true);
        yield return SlideUpEffect(rectTransform);
        yield return new WaitForSeconds(1.5f);
        yield return HideEffect(rectTransform);
        message.SetActive(false);
    }

    private IEnumerator SlideUpEffect(RectTransform rectTransform)
    {
        float moveSpeed = 5.0f;
        Vector2 targetPosition = new Vector2(-10, -405);
    
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    private IEnumerator HideEffect(RectTransform rectTransform)
    {
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
