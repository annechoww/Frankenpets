using UnityEngine;

public class Startup : MonoBehaviour
{
    public ControllerAssignment controllerAssignment;
    public GameObject startText;

    private void showText()
    {
        if (controllerAssignment.IsKeyboard())
        {
            startText.transform.GetChild(0).gameObject.SetActive(true);
            startText.transform.GetChild(2).gameObject.SetActive(true);
        }
        else {
            startText.transform.GetChild(1).gameObject.SetActive(true);
            startText.transform.GetChild(3).gameObject.SetActive(true);
        }
        
        // Flip the text to unmirror it
        // climbText.transform.rotation = Quaternion.Euler(0, climbText.transform.rotation.eulerAngles.y + 180, 0);
    }
}