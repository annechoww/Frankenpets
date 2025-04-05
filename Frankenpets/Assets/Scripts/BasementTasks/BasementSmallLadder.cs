using UnityEngine;

public class BasementSmallLadder : MonoBehaviour
{
    public GameObject BasementLargeLadder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("cat front")) 
        {
            gameObject.SetActive(false);
            BasementLargeLadder.SetActive(true);
        }
    }
}
