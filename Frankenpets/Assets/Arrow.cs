using UnityEngine;

public class Arrow : MonoBehaviour
{
    void Update()
    {
        transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.001f, 0);
        transform.Rotate (0, 0, Time.time * 0.001f);
    }
}
