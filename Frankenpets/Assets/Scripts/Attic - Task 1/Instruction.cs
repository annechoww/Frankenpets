using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
    public GameObject floatingTextPrefab;
    public KeyCode toggleKey = KeyCode.Space;

    private bool isShown = false;
    private GameObject floatingTextInstance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cat front") && !isShown)
        {
            Debug.Log($"enter collider.");
            
            floatingTextInstance = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            floatingTextInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

            // Make sure it's visible
            floatingTextInstance.SetActive(true);
            isShown = true;
        }
    } 

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && floatingTextInstance != null)
        {
            Destroy(floatingTextInstance);  // Destroy the floating text instance
        }
    }

    // void showFloatingText(){
    //     Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
    // }

}
