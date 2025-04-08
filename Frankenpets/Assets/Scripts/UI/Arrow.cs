using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float bounceSpeed = 3.0f;
    public float bounceDistance = 0.002f;
    public float rotationSpeed = 0.002f;

    void Update()
    {
        transform.position += new Vector3(0, Mathf.Sin(Time.time * bounceSpeed) * bounceDistance, 0);
        transform.Rotate (0, 0, Time.time * rotationSpeed);
    }
}
