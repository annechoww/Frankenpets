using UnityEngine;

public class BouncingSprite : MonoBehaviour
{
    private RectTransform arrowTransform;
    private Vector3 initialPosition;
    public float speed = 4.0f;
    public float amplitude = 40.0f;

    void Start()
    {
        arrowTransform = GetComponent<RectTransform>();
        initialPosition = arrowTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        arrowTransform.anchoredPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
