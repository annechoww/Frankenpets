using UnityEngine;

public class RollingPin : MonoBehaviour
{
    public GameObject arrow;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("dog front") || other.GetComponent<Collider>().CompareTag("cat front") ||
            other.GetComponent<Collider>().CompareTag("dog back") || other.GetComponent<Collider>().CompareTag("cat back"))
        {
            arrow.SetActive(false);
        }
    }
}
