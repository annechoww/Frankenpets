using UnityEngine;

public class BasementSmallLadder : MonoBehaviour
{
    public GameObject BasementLargeLadder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("cat front") || other.gameObject.CompareTag("dog front")) 
        {
            gameObject.SetActive(false);
            BasementLargeLadder.SetActive(true);
        }
    }
}
