using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CeilingFan : MonoBehaviour
{
    Stopwatch stopwatch = new Stopwatch();
    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatch.Elapsed.TotalSeconds >= 3.0f)
        {
            stopCeilingFan();
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("dog front"))
        {
            stopwatch.Start();
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dog front"))
        {
            stopCeilingFan();
        }
    }

    private void stopCeilingFan()
    {
        stopwatch.Stop();
        stopwatch.Reset();
        transform.Rotate(Vector3.zero);
    }
}
