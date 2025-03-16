using UnityEngine;

public class BouncingSprite : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    public float speed = 4.0f;
    public float amplitude = 40.0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
