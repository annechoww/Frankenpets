using UnityEngine;

public class Pulse : MonoBehaviour
{
    public RectTransform task;
    public float pulseSpeed = 2f;
    public float scaleAmount = 0.1f;

    private Vector3 initialScale;

    void Start()
    {
        if (task == null)
            task = GetComponent<RectTransform>();

        initialScale = task.localScale;
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        task.localScale = initialScale * scaleFactor;
    }
}
