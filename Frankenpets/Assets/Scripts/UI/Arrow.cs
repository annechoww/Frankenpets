using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float rotationSpeed = 70.0f; // degrees per second
    public float bounceSpeed = 3.0f;
    public float bounceDistance = 0.05f;
    
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * bounceSpeed) * bounceDistance;
        transform.position = startPos + new Vector3(0, newY, 0);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
