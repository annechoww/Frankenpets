using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float bounceSpeed = 2.0f;
    public float bounceDistance = 0.001f;
    public float rotationSpeed = 0.001f;

    void Update()
    {
        transform.position += new Vector3(0, Mathf.Sin(Time.time * bounceSpeed) * bounceDistance, 0);
        transform.Rotate (0, 0, Time.time * rotationSpeed);
    }
}
