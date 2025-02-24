// NOT BEING USED

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Instruction : MonoBehaviour
{
    public GameObject floatingTextPrefab;
    public KeyCode toggleKey = KeyCode.Space;

    private bool isShown = false;
    private GameObject floatingTextInstance;
    //private TMPro.TextMeshProUGUI floatingTextComponent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cat front") && !isShown)
        {
            Debug.Log($"enter collider.");
            
            floatingTextInstance = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            floatingTextInstance.transform.SetParent(GameObject.Find("Task Manager Canvas").transform, false);
            floatingTextInstance.SetActive(true);

            // floatingTextComponent = floatingTextInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            // if (floatingTextComponent != null)
            // {
            //     floatingTextComponent.text = "Click 'space' to split";
            // }
            isShown = true;
        }
    } 

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && floatingTextInstance != null)
        {
            //floatingTextComponent.text = "P1 scatter red glowing boxes";
            Destroy(floatingTextInstance); 
        }
    }

    // void showFloatingText(){
    //     Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
    // }

}
