using UnityEngine;
using TMPro;
using System.Collections;

public class TextBounce : MonoBehaviour
{
    public float bounceHeight = 100f;      // How high the bounce goes
    public float bounceDuration = 0.6f;   // How long each bounce lasts
    public int numberOfBounces = 4;       // How many bounces total

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float timeElapsed;
    private int currentBounce;
    private bool isBouncing = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        StartCoroutine(wait()); // Start the coroutine to wait before bouncing
        // StartBounce(); // You can trigger this elsewhere too
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(2.4f);
        StartBounce();
    }

    public void StartBounce()
    {
        timeElapsed = 0f;
        currentBounce = 0;
        isBouncing = true;
    }

    void Update()
    {
        if (!isBouncing) return;

        timeElapsed += Time.deltaTime;

        float singleBounceTime = bounceDuration;
        float totalTime = numberOfBounces * singleBounceTime;

        if (timeElapsed >= totalTime)
        {
            // Restart the bounce loop
            timeElapsed = 0f;
            currentBounce = 0;
            return;
        }

        // Current bounce
        float t = timeElapsed % singleBounceTime;
        float normalizedTime = t / singleBounceTime;
        int bounceIndex = Mathf.FloorToInt(timeElapsed / singleBounceTime);

        // Damping factor (decreases each bounce)
        float damping = Mathf.Pow(0.5f, bounceIndex % numberOfBounces);
        float offsetY = Mathf.Sin(normalizedTime * Mathf.PI) * bounceHeight * damping;

        rectTransform.anchoredPosition = originalPosition + Vector2.up * offsetY;
    }

}
